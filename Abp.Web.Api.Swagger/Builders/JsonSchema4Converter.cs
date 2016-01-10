using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NJsonSchema;

namespace Abp.Builders
{
    internal class JsonSchema4Converter : JsonConverter
    {
        private readonly Type[] _types;
        public JsonSchema4Converter(params Type[] types)
        {
            _types = types;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var token = JToken.FromObject(value);
            if (token.Type != JTokenType.Object)
            {
                token.WriteTo(writer);
            }
            else
            {
                var obj = (JObject)token;
                var firstOrDefault = obj.Properties().FirstOrDefault(x => x.Name == "typeName");
                firstOrDefault?.Remove();

                obj.WriteTo(writer);
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException("Unnecessary because CanRead is false. The type will skip the converter.");
        }

        public override bool CanConvert(Type objectType)
        {
            return _types.Any(t => t == objectType);
        }

        public override bool CanRead => false;
    }
}
