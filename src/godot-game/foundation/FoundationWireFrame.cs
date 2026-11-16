using System;
using System.Buffers;
using System.Buffers.Binary;
using System.IO;
using System.IO.Compression;

namespace KingdomZombies.Foundation;

/// <summary>Frames bounded native packets and restores their exact application bytes before dispatch.</summary>
internal static class FoundationWireFrame
{
    internal const int MaximumEncodedBytes = FoundationProtocol.MaximumBytes + HeaderBytes;

    internal static byte[] Encode(ReadOnlyMemory<byte> payload)
    {
        if (payload.Length is < 2 or > FoundationProtocol.MaximumBytes)
        {
            throw new InvalidDataException(message: "FRAME_APPLICATION_SIZE");
        }

        byte[] body = payload.ToArray();
        byte[] frame = new byte[HeaderBytes + body.Length];
        frame[0] = (byte)'K';
        frame[1] = (byte)'Z';
        frame[2] = 1;
        frame[3] = 0;
        BinaryPrimitives.WriteInt32LittleEndian(destination: frame.AsSpan(start: 4), value: payload.Length);
        body.CopyTo(array: frame, index: HeaderBytes);
        return frame;
    }

    internal static byte[] Decode(ReadOnlyMemory<byte> frame)
    {
        if (frame.Length is < HeaderBytes + 1 or > MaximumEncodedBytes || frame.Span[0] != (byte)'K'
                || frame.Span[1] != (byte)'Z' || frame.Span[2] != 1 || frame.Span[3] > 1)
        {
            throw new InvalidDataException(message: "FRAME_HEADER");
        }

        int length = BinaryPrimitives.ReadInt32LittleEndian(source: frame.Span[4..HeaderBytes]);
        if (length is < 2 or > FoundationProtocol.MaximumBytes)
        {
            throw new InvalidDataException(message: "FRAME_DECODED_SIZE");
        }

        ReadOnlySpan<byte> body = frame.Span[HeaderBytes..];
        if (frame.Span[3] == 1) { return Decompress(body: body, length: length); }
        if (body.Length != length) { throw new InvalidDataException(message: "FRAME_RAW_SIZE"); }
        return body.ToArray();
    }

    private const int HeaderBytes = 8;

    private static byte[] Decompress(ReadOnlySpan<byte> body, int length)
    {
        byte[] decoded = new byte[length];
        using var decoder = new BrotliDecoder();
        OperationStatus status = decoder.Decompress(source: body, destination: decoded,
                bytesConsumed: out int consumed, bytesWritten: out int written);
        if (status != OperationStatus.Done || consumed != body.Length || written != length)
        {
            throw new InvalidDataException(message: "FRAME_BROTLI");
        }

        return decoded;
    }
}
