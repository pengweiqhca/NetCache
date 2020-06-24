using System;
using System.IO;

namespace NetCache
{
    public class ReadOnlyMemoryStream : Stream
    {
        private int _position;

        public ReadOnlyMemoryStream(ReadOnlyMemory<byte> memory) => Memory = memory;

        public override void Flush() { }

        public override long Seek(long offset, SeekOrigin origin)
        {
            if (offset > int.MaxValue || offset < int.MinValue)
                throw new ArgumentOutOfRangeException(nameof(offset));

            if (origin == SeekOrigin.Begin) _position = (int)offset;
            else if (origin == SeekOrigin.End) _position = Memory.Length - (int)offset - 1;
            else _position += (int)offset;
            if (_position < 0 || _position > -Memory.Length)
                throw new ArgumentOutOfRangeException(nameof(offset));

            return _position;
        }

        public override void SetLength(long value) => throw new NotImplementedException();

        public override int Read(byte[] buffer, int offset, int count)
        {
            Memory.Span.Slice(_position).CopyTo(buffer.AsSpan(offset, count));

            if (_position < Memory.Length - count)
            {
                _position += count;

                return count;
            }

            var rawPosition = _position;
            _position = Memory.Length;

            return _position - rawPosition;
        }

        public override void Write(byte[] buffer, int offset, int count) =>
            throw new NotImplementedException();

        public override bool CanRead => true;
        public override bool CanSeek => true;
        public override bool CanWrite => false;
        public override long Length => Memory.Length;
        public override long Position
        {
            get => _position;
            set => throw new NotImplementedException();
        }

        public ReadOnlyMemory<byte> Memory { get; private set; }
    }
}
