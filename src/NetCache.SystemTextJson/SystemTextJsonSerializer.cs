using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.Json;

namespace NetCache
{
    public class SystemTextJsonSerializer : ValueSerializer
    {
        private readonly JsonSerializerOptions? _options;

        public SystemTextJsonSerializer(JsonSerializerOptions? options = null) => _options = options;

        protected override TV InternalDeserialize<TV>(Stream stream)
        {
            var task = JsonSerializer.DeserializeAsync<TV>(stream, _options);

            return task.IsCompleted ? task.GetAwaiter().GetResult() : task.AsTask().GetAwaiter().GetResult();
        }

        protected override void InternalSerialize<TV>([AllowNull]TV value, Stream stream)
        {
            using var writer = new Utf8JsonWriter(stream);

            JsonSerializer.Serialize(writer, value!, _options);
        }
    }
}
