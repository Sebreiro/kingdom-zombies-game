using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Godot;

namespace KingdomZombies.Proofs;
/// <summary>Drives bounded real scene cycles exclusively from native process-frame callbacks.</summary>
internal static class ProofSequence
{
    /// <summary>Gets the gateway captured before engine-facing proof work begins.</summary>
    internal static MainThreadGateway Gateway { get; } = new();
    /// <summary>Gets the generation selected before the engine instantiates its scripts.</summary>
    internal static LifetimeToken Current { get; private set; } = new();

    /// <summary>Advances one bounded step on the captured engine thread.</summary>
    internal static void Tick(Node owner, string nonce)
    {
        Gateway.RequireThread();
        try
        {
            Require(condition: _started == 0 || Time.GetTicksMsec() - _started < 5000, message: "CYCLE_TIMEOUT");
            Advance(owner: owner, nonce: nonce);
        }
        catch (Exception error)
        {
            GD.Print(what: ["PHASE1_FAILURE " + error]);
            Cleanup(owner: owner);
            owner.GetTree().Quit(exitCode: 1);
        }
    }

    private static readonly ManualResetEventSlim _barrier = new(initialState: false);
    private static readonly LifetimeToken _first = Current;
    private static LifecycleFixture? _fixture;
    private static Task? _worker;
    private static Task? _prohibited;
    private static string _rejection = "";
    private static ulong _started;
    private static int _step;
    private static int _unsafeOperations;

    private static void Advance(Node owner, string nonce)
    {
        switch (_step)
        {
            case 0:
                Load(owner: owner);
                _step++;
                break;
            case 1 when Current.Processes > 0:
                BeginWorkers();
                _step++;
                break;
            case 2 when _worker?.IsCompleted == true && _prohibited?.IsCompleted == true:
                Replace(owner: owner);
                _step++;
                break;
            case 3 when Current.Processes > 0:
                Release();
                _step++;
                break;
            case 4 when _worker?.IsCompleted == true:
                Finish(owner: owner, nonce: nonce);
                _step++;
                break;
        }
    }

    private static void Load(Node owner)
    {
        using PackedScene scene = GD.Load<PackedScene>(path: "res://proofs/ValidFixture.tscn");
        _fixture = scene.Instantiate<LifecycleFixture>();
        owner.AddChild(node: _fixture);
        _started = Time.GetTicksMsec();
    }

    private static void BeginWorkers()
    {
        Fixture().Use();
        Require(condition: Current.Handlers == 1 && Current.Value == 7, message: "SIGNAL_ONCE");
        _worker = Task.Run(action: () => Gateway.Enqueue(lifetime: _first, value: 99,
                apply: value => _first.Value = value, barrier: _barrier));
        _prohibited = Task.Run(action: () =>
        {
            try
            {
                Gateway.Run(operation: () => _unsafeOperations++);
                _rejection = "UNGUARDED";
            }
            catch (InvalidOperationException error)
            {
                _rejection = error.Message;
            }
        });
    }

    private static void Replace(Node owner)
    {
        Require(condition: _worker?.IsCompletedSuccessfully == true && _prohibited?.IsCompletedSuccessfully == true,
                message: "WORKERS_FAILED");
        Require(condition: StringComparer.Ordinal.Equals(x: _rejection,
                y: "THREAD_ACCESS") && _unsafeOperations == 0, message: "WORKER_ENGINE_OPERATION");
        Gateway.Drain();
        Require(condition: _first.Value == 7, message: "BARRIER_CLOSED");
        Cleanup(owner: owner);
        Validate(lifetime: _first);
        Current = new();
        Load(owner: owner);
    }

    private static void Release()
    {
        _barrier.Set();
        Gateway.Drain();
        Require(condition: _first.Value == 7 && _first.Handlers == 1 && Current.Value == 0
                && Current.Handlers == 0 && !_first.IsOpen && _first.Cancellation.IsCancellationRequested,
                message: "LATE_GENERATION_MUTATION");
        Fixture().Use();
        LifetimeToken live = Current;
        _worker = Task.Run(action: () => Gateway.Enqueue(lifetime: live, value: 41,
                apply: value => Apply(lifetime: live, value: value)));
    }

    private static void Apply(LifetimeToken lifetime, int value)
    {
        Gateway.RequireThread();
        lifetime.Value = value;
    }

    private static void Finish(Node owner, string nonce)
    {
        Require(condition: _worker?.IsCompletedSuccessfully == true, message: "WORKER_FAILED");
        Gateway.Drain();
        Require(condition: Current.Value == 41 && Current.Handlers == 1 && _first.Value == 7
                && _first.Handlers == 1, message: "MAIN_THREAD_RESULT");
        Cleanup(owner: owner);
        Validate(lifetime: Current);
        _barrier.Dispose();
        _first.Cancellation.Dispose();
        Current.Cancellation.Dispose();
        foreach (int cycle in new[] { 1, 2 })
        {
            foreach (string surface in new[] { "construction", "callbacks", "process", "signals", "teardown" })
            {
                Publish(nonce: nonce, surface: surface, cycle: cycle);
            }
        }

        Publish(nonce: nonce, surface: "late-work", cycle: 1);
        Publish(nonce: nonce, surface: "worker-rejected", cycle: 1);
        Publish(nonce: nonce, surface: "main-thread", cycle: 2);
        owner.GetTree().Quit();
    }

    private static void Cleanup(Node owner)
    {
        if (_fixture is null)
        {
            return;
        }

        owner.RemoveChild(node: _fixture);
        _fixture.Free();
        _fixture = null;
    }

    private static LifecycleFixture Fixture()
    {
        return _fixture ?? throw new InvalidOperationException(message: "FIXTURE_MISSING");
    }

    private static void Validate(LifetimeToken lifetime)
    {
        string[] expected = ["Parent:enter", "Child:enter", "Child:ready", "Parent:ready", "Child:exit", "Parent:exit"];
        Require(condition: lifetime.Events.SequenceEqual(second: expected, comparer: StringComparer.Ordinal)
                && lifetime.Constructions == 2 && lifetime.Processes > 0 && lifetime.Handlers == 1
                && !lifetime.IsOpen, message: "LIFECYCLE_ORDER");
    }

    private static void Publish(string nonce, string surface, int cycle)
    {
        GD.Print(what: ["PHASE1_SURFACE " + JsonSerializer.Serialize(value: new { nonce, surface, cycle })]);
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message: message);
        }
    }
}
