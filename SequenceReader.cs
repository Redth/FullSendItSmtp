using System.Buffers;

public class SequenceReader : Stream
{
    private readonly ReadOnlySequence<byte> _sequence;
    private SequencePosition _position;
    private long _length;

    public SequenceReader(ReadOnlySequence<byte> sequence)
    {
        _sequence = sequence;
        _position = sequence.Start;
        _length = sequence.Length;
    }

    public override bool CanRead => true;
    public override bool CanSeek => false;
    public override bool CanWrite => false;
    public override long Length => _length;
    public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

    public override int Read(byte[] buffer, int offset, int count)
    {
        var remaining = _sequence.Slice(_position);
        var toCopy = (int)Math.Min(count, remaining.Length);
        
        if (toCopy == 0)
            return 0;

        remaining.Slice(0, toCopy).CopyTo(buffer.AsSpan(offset));
        _position = _sequence.GetPosition(toCopy, _position);
        
        return toCopy;
    }

    public override void Flush() => throw new NotSupportedException();
    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
    public override void SetLength(long value) => throw new NotSupportedException();
    public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
}
