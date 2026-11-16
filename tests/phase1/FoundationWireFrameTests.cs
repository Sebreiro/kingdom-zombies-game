using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using KingdomZombies.Core;
using KingdomZombies.Foundation;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Exercises the native adapter's actual frame codec without an engine or authority mutation.</summary>
internal static class FoundationWireFrameTests
{
    internal static void Check()
    {
        CheckRaw();
        CheckBounds();
        foreach (bool unicode in new[] { false, true }) { CheckSnapshot(unicode: unicode); }
        byte[] payload = [.. Enumerable.Repeat(element: (byte)'A', count: 32768)];
        byte[] frame = FoundationWireFrame.Encode(payload: payload);
        frame[3].ShouldBe(expected: (byte)1);
        FoundationWireFrame.Decode(frame: frame).ShouldBe(expected: payload);
        CheckCompressed(frame: frame);
    }

    private static void CheckRaw()
    {
        byte[] payload = [(byte)'K', (byte)'Z'];
        byte[] frame = FoundationWireFrame.Encode(payload: payload);
        frame[3].ShouldBe(expected: (byte)0);
        FoundationWireFrame.Decode(frame: frame).ShouldBe(expected: payload);
        Reject(frame: [.. frame, 0]);
        Reject(frame: frame[..^1]);
        byte[] random = new byte[32768];
        new Random(Seed: 1701).NextBytes(buffer: random);
        byte[] raw = FoundationWireFrame.Encode(payload: random);
        raw.Length.ShouldBe(expected: FoundationWireFrame.MaximumEncodedBytes);
        raw[3].ShouldBe(expected: (byte)0);
        FoundationWireFrame.Decode(frame: raw).ShouldBe(expected: random);
        foreach (string kind in new[] { "hello", "ack", "intent", "ping", "bye", "end", "end-ack", "cancel" })
        {
            byte[] control = FoundationProtocol.Encode(envelope: new FoundationEnvelope(kind: kind,
                    sessionId: "session", playerId: "player", actorId: "actor", bindingId: "binding",
                    label: "Élodie 生存者", attemptId: "attempt"));
            FoundationWireFrame.Decode(frame: FoundationWireFrame.Encode(payload: control))
                    .ShouldBe(expected: control);
        }
    }

    private static void CheckBounds()
    {
        foreach (int length in new[] { 0, 1, 32769 })
        {
            Should.Throw<InvalidDataException>(actual: () => FoundationWireFrame.Encode(payload: new byte[length]));
        }

        Reject(frame: []);
        Reject(frame: new byte[32777]);
        byte[] valid = FoundationWireFrame.Encode(payload: new byte[] { 1, 2 });
        for (int length = 0; length < valid.Length; length++) { Reject(frame: valid[..length]); }
        foreach (int index in new[] { 0, 1, 2, 3 })
        {
            byte[] invalid = (byte[])valid.Clone();
            invalid[index] = 255;
            Reject(frame: invalid);
        }

        foreach (int length in new[] { -1, 0, 1, 3, 32769, int.MaxValue })
        {
            byte[] invalid = (byte[])valid.Clone();
            BinaryPrimitives.WriteInt32LittleEndian(destination: invalid.AsSpan(start: 4), value: length);
            Reject(frame: invalid);
        }
    }

    private static void CheckSnapshot(bool unicode)
    {
        var random = new Random(Seed: 13037);
        string session = Identity(random: random);
        IEnumerable<FoundationParticipantState> participants = Enumerable.Range(start: 1, count: 4).Select(
                selector: slot => new FoundationParticipantState(bindingId:
                        slot == 1 ? "local" : Identity(random: random),
                        label: unicode ? "Élodie 生存者 🎮 " + slot : "Player " + slot, slot: slot, ready: true,
                        actor: new FoundationActorState(sessionId: session,
                                playerId: Identity(random: random),
                                actorId: Identity(random: random), positionSubunits: 10000 + slot,
                                facing: 1, elapsedMilliseconds: 123456)));
        var snapshot = new FoundationSnapshot(sessionId: session, revision: 1234, participants: participants);
        byte[] payload = FoundationProtocol.Encode(envelope: new FoundationEnvelope(kind: "state",
                sessionId: session, revision: snapshot.Revision, snapshot: snapshot,
                attemptId: Identity(random: random)));
        FoundationProtocol.Decode(bytes: payload).Snapshot?.Participants.Length.ShouldBe(expected: 4);
        byte[] frame = FoundationWireFrame.Encode(payload: payload);
        frame.Length.ShouldBeLessThan(expected: 1392, customMessage: "Full four-player snapshot native MTU");
        FoundationWireFrame.Decode(frame: frame).ShouldBe(expected: payload);
        TestContext.Current.TestOutputHelper?.WriteLine(message: "WIRE_FRAME unicode=" + unicode
                + " application=" + payload.Length
                + " encoded=" + frame.Length);
    }

    private static void CheckCompressed(byte[] frame)
    {
        Reject(frame: frame[..^1]);
        Reject(frame: [.. frame, 0]);
        Reject(frame: [.. frame, .. frame[8..]]);
        byte[] invalid = (byte[])frame.Clone();
        invalid.AsSpan(start: 8).Fill(value: 255);
        Reject(frame: invalid);
        foreach (int length in new[] { 2, 32767 })
        {
            byte[] bomb = (byte[])frame.Clone();
            BinaryPrimitives.WriteInt32LittleEndian(destination: bomb.AsSpan(start: 4), value: length);
            Reject(frame: bomb);
        }

        byte[] shortFrame = FoundationWireFrame.Encode(payload: Enumerable.Repeat(element: (byte)'B', count: 256)
                .ToArray());
        BinaryPrimitives.WriteInt32LittleEndian(destination: shortFrame.AsSpan(start: 4), value: 32768);
        Reject(frame: shortFrame);
    }

    private static void Reject(byte[] frame)
    {
        Should.Throw<InvalidDataException>(actual: () => FoundationWireFrame.Decode(frame: frame));
    }

    private static string Identity(Random random)
    {
        byte[] bytes = new byte[16];
        random.NextBytes(buffer: bytes);
        return Convert.ToHexString(bytes: bytes);
    }
}
