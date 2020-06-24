using System;
using System.Runtime.Serialization;

namespace NetCache
{
    [Serializable]
    public class DistributedLockException : Exception
    {
        public DistributedLockException() : base(Res.Repeat_Wait) { }
        public DistributedLockException(string message) : base(message) { }
        public DistributedLockException(string message, Exception innerException)
            : base(message, innerException) { }

        protected DistributedLockException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext) { }
    }
}
