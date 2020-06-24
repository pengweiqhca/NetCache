using Google.Protobuf;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace NetCache
{
    public class GoogleProtobufSerializer : ValueSerializer
    {
        private static class Wrapper<TV>
        {
            public static bool IsGoogleProtobuf { get; } =
                typeof(IMessage).IsAssignableFrom(typeof(TV)) &&
#if NET45
                typeof(TV).GetConstructor(new Type[0]) != null;
#else
                typeof(TV).GetConstructor(Array.Empty<Type>()) != null;
#endif
        }

        protected override TV InternalDeserialize<TV>(Stream stream)
        {
            if (!Wrapper<TV>.IsGoogleProtobuf)
                throw new InvalidOperationException($"{typeof(TV).FullName} must be implement IMessage");

            var value = (IMessage)Activator.CreateInstance<TV>()!;

            value.MergeFrom(stream);

            return (TV)value;
        }

        protected override void InternalSerialize<TV>([AllowNull] TV value, Stream stream)
        {
            if (!Wrapper<TV>.IsGoogleProtobuf)
                throw new InvalidOperationException($"{typeof(TV).FullName} must be implement IMessage");

            if (value is IMessage message) message.WriteTo(stream);
        }
    }
}
