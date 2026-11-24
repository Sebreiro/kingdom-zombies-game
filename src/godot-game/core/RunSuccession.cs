using System;
using System.Collections.Immutable;
using System.Linq;

namespace KingdomZombies.Core;

/// <summary>Orders explicit post-death requests and leases untouched existing survivors.</summary>
internal static class RunSuccession
{
    internal static State Start(string sessionId)
    {
        uint hash = 2166136261;
        foreach (char unit in sessionId)
        {
            hash = unchecked((hash ^ unit) * 16777619);
        }

        return new State(waiting: [], seed: unchecked((int)hash), drawPosition: 0);
    }

    internal static Batch Prepare(FoundationSession.RunPublication current, ImmutableArray<Request> requests,
            ImmutableDictionary<(string binding, long epoch), Ack> acknowledgments)
    {
        State state = current.Succession;
        ImmutableArray<Attempt>.Builder attempts = ImmutableArray.CreateBuilder<Attempt>(initialCapacity:
                requests.Length);
        foreach (Request request in requests)
        {
            (Attempt attempt, Wait? ticket) = Check(current: current, state: state, request: request,
                    acks: acknowledgments);
            attempts.Add(item: attempt);
            if (ticket is not null)
            {
                state = state.WithWaiting(waiting: state.Waiting.Add(item: ticket));
            }

            if (attempt.Stamp)
            {
                acknowledgments = acknowledgments.SetItem(key: (request.BindingId, request.DeathGeneration),
                        value: new Ack(sequence: request.Sequence, status: attempt.Status));
            }
        }

        return new Batch(state: state, attempts: attempts.MoveToImmutable(), acknowledgments: acknowledgments);
    }

    internal static Frame Settle(RunPopulation population, ImmutableArray<FoundationParticipantState> participants,
            RunDeathAndLoot.State loot, State state)
    {
        if (state.Waiting.IsEmpty)
        {
            return new Frame(population: population, participants: participants, state: state);
        }

        ImmutableArray<Wait>.Builder waiting = ImmutableArray.CreateBuilder<Wait>();
        IRandomInput? random = null;
        foreach (Wait ticket in state.Waiting.OrderBy(keySelector: ticket => (ticket.At, ticket.Slot)))
        {
            FoundationParticipantState? participant = Find(participants: participants, binding: ticket.BindingId);
            RunDeathAndLoot.Death? death = LatestDeath(loot: loot, binding: ticket.BindingId);
            if (participant is null || death?.Lease is not { } retired || retired.Generation != ticket.Epoch
                    || population.FindLivingControl(bindingId: ticket.BindingId) is not null
                    || Defeated(population: population))
            {
                continue;
            }

            ImmutableArray<RunPopulation.Survivor> candidates = population.Candidates();
            if (candidates.IsEmpty || population.LastControlGeneration == long.MaxValue)
            {
                waiting.Add(item: ticket);
                continue;
            }

            RunPopulation.Survivor chosen = Choose(candidates: candidates, state: state, random: ref random);
            population = population.AssignSuccessor(retired: retired, successor: chosen);
            participants = participants.Replace(oldValue: participant, newValue: participant.FollowControl(
                    survivor: chosen));
        }

        return new Frame(population: population, participants: participants, state: new State(
                waiting: waiting.ToImmutable(), seed: state.Seed, drawPosition: random?.DrawPosition
                        ?? state.DrawPosition));
    }

    internal static View? Read(FoundationSession.RunPublication current, string binding)
    {
        ImmutableArray<FoundationParticipantState> participants = current.Snapshot.Participants;
        if (Find(participants: participants, binding: binding) is null)
        {
            return null;
        }

        RunPopulation.Lease? lease = current.Population.FindLivingControl(bindingId: binding);
        RunDeathAndLoot.Death? death = LatestDeath(loot: current.Loot, binding: binding);
        decimal? opensAt = death?.At + current.Definitions.RespawnDelay;
        Status status = ViewStatus(current: current, binding: binding, lease: lease, opensAt: opensAt);
        return new View(status: status, survivorId: lease?.SurvivorId, generation: lease?.Generation,
                deathGeneration: death?.Lease?.Generation, opensAt: opensAt, deathContact: death?.Contact,
                teammates: Teammates(current: current, binding: binding));
    }

    internal static ImmutableArray<Outcome> Complete(FoundationSession.RunPublication current,
            ImmutableArray<Request> requests, Batch batch, bool published,
            ref ImmutableDictionary<(string binding, long epoch), Ack> receipts)
    {
        if (published)
        {
            receipts = batch.Acknowledgments;
        }

        ImmutableArray<Outcome>.Builder results = ImmutableArray.CreateBuilder<Outcome>(initialCapacity:
                requests.Length);
        for (int index = 0; index < requests.Length; index++)
        {
            Request request = requests[index];
            Attempt attempt = batch.Attempts[index];
            View? view = attempt.Status == Status.AccessLost ? null : Read(current: current,
                    binding: request.BindingId);
            Status status = attempt.Status;
            if (status == Status.Waiting && !published)
            {
                status = Status.Rejected;
            }
            else if (status == Status.Waiting && view?.Status == Status.Active)
            {
                status = Status.Assigned;
                (string binding, long epoch) key = (request.BindingId, request.DeathGeneration);
                if (receipts.TryGetValue(key: key, value: out Ack? ack) && ack.Sequence == request.Sequence)
                {
                    receipts = receipts.SetItem(key: key, value: new Ack(sequence: ack.Sequence, status: status));
                }
            }

            results.Add(item: new Outcome(status: status, view: view, replayed: attempt.Replayed));
        }

        return results.MoveToImmutable();
    }

    private static (Attempt attempt, Wait? ticket) Check(FoundationSession.RunPublication current, State state,
            Request request, ImmutableDictionary<(string binding, long epoch), Ack> acks)
    {
        FoundationParticipantState? participant = Find(participants: current.Snapshot.Participants,
                binding: request.BindingId);
        if (participant is null || !StringComparer.Ordinal.Equals(x: current.Snapshot.SessionId, y: request.SessionId))
        {
            return (new Attempt(status: Status.AccessLost), null);
        }

        RunDeathAndLoot.Death? death = LatestDeath(loot: current.Loot, binding: request.BindingId);
        if (death is not { Lease: { } retired } || retired.Generation != request.DeathGeneration)
        {
            return (new Attempt(status: Status.StaleDeath), null);
        }

        if (request.Sequence <= 0)
        {
            return (new Attempt(status: Status.StaleRequest), null);
        }

        if (acks.TryGetValue(key: (request.BindingId, request.DeathGeneration), value: out Ack? old)
                && request.Sequence <= old.Sequence)
        {
            bool replay = request.Sequence == old.Sequence;
            return (new Attempt(status: replay ? old.Status : Status.StaleRequest, replayed: replay), null);
        }

        decimal opensAt = death.At + current.Definitions.RespawnDelay;
        RunPopulation.Lease? lease = current.Population.FindLivingControl(bindingId: request.BindingId);
        Status status = ViewStatus(current: current, binding: request.BindingId, lease: lease, opensAt: opensAt);
        if (status != Status.Ready)
        {
            return (new Attempt(status: status, stamp: true), null);
        }

        bool pending = state.Waiting.Any(predicate: ticket => StringComparer.Ordinal.Equals(
                x: ticket.BindingId, y: request.BindingId));
        var queued = new Wait(bindingId: request.BindingId, epoch: request.DeathGeneration,
                slot: participant.Slot, at: current.ElapsedMilliseconds);
        return (new Attempt(status: Status.Waiting, stamp: true), pending ? null : queued);
    }

    private static Status ViewStatus(FoundationSession.RunPublication current, string binding,
            RunPopulation.Lease? lease, decimal? opensAt)
    {
        if (lease is not null)
        {
            return Status.Active;
        }

        if (Defeated(population: current.Population))
        {
            return Status.Defeated;
        }

        if (current.Succession.Waiting.Any(predicate: wait => StringComparer.Ordinal.Equals(
                x: wait.BindingId, y: binding)))
        {
            return Status.Waiting;
        }

        return opensAt is null ? Status.StaleDeath : current.ElapsedMilliseconds < opensAt
                ? Status.Early : Status.Ready;
    }

    private static RunPopulation.Survivor Choose(ImmutableArray<RunPopulation.Survivor> candidates,
            State state, ref IRandomInput? random)
    {
        if (candidates.Length == 1)
        {
            return candidates[0];
        }

        random ??= new ReplayRandom(seed: state.Seed, runtimeContract: ReplayRandom.PinnedRuntimeContract,
                drawPosition: state.DrawPosition);
        long limit = long.MaxValue - long.MaxValue % candidates.Length;
        long draw;
        do
        {
            draw = random.Draw();
        }
        while (draw >= limit);
        return candidates[(int)(draw % candidates.Length)];
    }

    private static ImmutableArray<Teammate> Teammates(FoundationSession.RunPublication current, string binding)
    {
        ImmutableArray<Teammate>.Builder teammates = ImmutableArray.CreateBuilder<Teammate>();
        foreach (FoundationParticipantState participant in current.Snapshot.Participants.OrderBy(
                keySelector: participant => participant.Slot))
        {
            if (participant.Ready && !StringComparer.Ordinal.Equals(x: participant.BindingId, y: binding)
                    && current.Population.FindLivingControl(bindingId: participant.BindingId) is { } lease)
            {
                teammates.Add(item: new Teammate(bindingId: participant.BindingId, slot: participant.Slot,
                        survivorId: lease.SurvivorId));
            }
        }

        return teammates.ToImmutable();
    }

    private static FoundationParticipantState? Find(ImmutableArray<FoundationParticipantState> participants,
            string binding)
    {
        return participants.FirstOrDefault(predicate: participant => participant.Ready
                && StringComparer.Ordinal.Equals(x: participant.BindingId, y: binding));
    }

    private static RunDeathAndLoot.Death? LatestDeath(RunDeathAndLoot.State loot, string binding)
    {
        return loot.Deaths.Values.Where(predicate: death => death.Lease is { } lease
                && StringComparer.Ordinal.Equals(x: lease.BindingId, y: binding))
                .MaxBy(keySelector: death => death.Lease?.Generation ?? 0);
    }

    private static bool Defeated(RunPopulation population)
    {
        return !population.Survivors.Any(predicate: survivor => survivor.EligibleForControl);
    }

    internal enum Status
    {
        AccessLost,
        StaleDeath,
        StaleRequest,
        Active,
        Early,
        Ready,
        Waiting,
        Assigned,
        Defeated,
        Rejected,
    }

    internal sealed record Request
    {
        internal Request(string sessionId, string bindingId, long deathGeneration, long sequence)
        {
            SessionId = sessionId;
            BindingId = bindingId;
            DeathGeneration = deathGeneration;
            Sequence = sequence;
        }

        internal string SessionId { get; }
        internal string BindingId { get; }
        internal long DeathGeneration { get; }
        internal long Sequence { get; }
    }

    internal sealed record Ack
    {
        internal Ack(long sequence, Status status)
        {
            Sequence = sequence;
            Status = status;
        }

        internal long Sequence { get; }
        internal Status Status { get; }
    }

    internal sealed record Attempt
    {
        internal Attempt(Status status, bool stamp = false, bool replayed = false)
        {
            Status = status;
            Stamp = stamp;
            Replayed = replayed;
        }

        internal Status Status { get; }
        internal bool Stamp { get; }
        internal bool Replayed { get; }
    }

    internal sealed record Batch
    {
        internal Batch(State state, ImmutableArray<Attempt> attempts,
                ImmutableDictionary<(string binding, long epoch), Ack> acknowledgments)
        {
            State = state;
            Attempts = attempts;
            Acknowledgments = acknowledgments;
        }

        internal State State { get; }
        internal ImmutableArray<Attempt> Attempts { get; }
        internal ImmutableDictionary<(string binding, long epoch), Ack> Acknowledgments { get; }
    }

    internal sealed record Outcome
    {
        internal Outcome(Status status, View? view, bool replayed = false)
        {
            Status = status;
            View = view;
            Replayed = replayed;
        }

        internal Status Status { get; }
        internal View? View { get; }
        internal bool Replayed { get; }
    }

    internal sealed record View
    {
        internal View(Status status, string? survivorId, long? generation, long? deathGeneration, decimal? opensAt,
                RunWorld.Point? deathContact, ImmutableArray<Teammate> teammates)
        {
            Status = status;
            SurvivorId = survivorId;
            Generation = generation;
            DeathGeneration = deathGeneration;
            OpensAt = opensAt;
            DeathContact = deathContact;
            Teammates = teammates;
        }

        internal Status Status { get; }
        internal string? SurvivorId { get; }
        internal long? Generation { get; }
        internal long? DeathGeneration { get; }
        internal decimal? OpensAt { get; }
        internal RunWorld.Point? DeathContact { get; }
        internal ImmutableArray<Teammate> Teammates { get; }
    }

    internal sealed record Teammate
    {
        internal Teammate(string bindingId, int slot, string survivorId)
        {
            BindingId = bindingId;
            Slot = slot;
            SurvivorId = survivorId;
        }

        internal string BindingId { get; }
        internal int Slot { get; }
        internal string SurvivorId { get; }
    }

    internal sealed record Wait
    {
        internal Wait(string bindingId, long epoch, int slot, long at)
        {
            BindingId = bindingId;
            Epoch = epoch;
            Slot = slot;
            At = at;
        }

        internal string BindingId { get; }
        internal long Epoch { get; }
        internal int Slot { get; }
        internal long At { get; }
    }

    internal sealed record State
    {
        internal State(ImmutableArray<Wait> waiting, int seed, long drawPosition)
        {
            Waiting = waiting;
            Seed = seed;
            DrawPosition = drawPosition;
        }

        internal ImmutableArray<Wait> Waiting { get; }
        internal int Seed { get; }
        internal long DrawPosition { get; }

        internal State WithWaiting(ImmutableArray<Wait> waiting)
        {
            return new State(waiting: waiting, seed: Seed, drawPosition: DrawPosition);
        }
    }

    internal sealed record Frame
    {
        internal Frame(RunPopulation population, ImmutableArray<FoundationParticipantState> participants, State state)
        {
            Population = population;
            Participants = participants;
            State = state;
        }

        internal RunPopulation Population { get; }
        internal ImmutableArray<FoundationParticipantState> Participants { get; }
        internal State State { get; }
    }
}
