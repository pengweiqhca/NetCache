using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;

namespace NetCache
{
    public class NewtonsoftJsonSerializer : ValueSerializer
    {
        private readonly JsonSerializerSettings? _settings;

        public NewtonsoftJsonSerializer(JsonSerializerSettings? settings = null) => _settings = settings;

        protected override TV InternalDeserialize<TV>(Stream stream)
        {
            var serializer = JsonSerializer.CreateDefault(_settings);

            using var sr = new StreamReader(stream, Encoding.UTF8);
            using var jr = new JsonTextReader(sr);

            return serializer.Deserialize<TV>(jr);
        }

        protected override void InternalSerialize<TV>([AllowNull] TV value, Stream stream)
        {
            var serializer = JsonSerializer.CreateDefault(_settings);

            using var sr = new StreamWriter(stream, Encoding.UTF8);
            using var jr = new JsonTextWriter(sr);

            serializer.Serialize(jr, value);
        }
    }
}
