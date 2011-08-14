using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace FireEngine.Library
{
    public abstract class DebugStreamBase : Stream
    {
        private StringBuilder sb = new StringBuilder();

        public DebugStreamBase()
            : base()
        {
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override abstract void Flush();

        public override long Length
        {
            get { return sb.Length; }
        }

        public override long Position
        {
            get
            {
                throw new NotSupportedException();
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            sb.Append(UTF8Encoding.UTF8.GetString(buffer, offset, count));
        }
    }
}
