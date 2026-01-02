using System;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Unity.UOS.Networking
{
    public class TimestampContractResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);

            if (property.PropertyType == typeof(Google.Protobuf.WellKnownTypes.Timestamp))
            {
                property.Converter = new TimestampConverter();
            }

            return property;
        }

        public class TimestampConverter : DateTimeConverterBase
        {
            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                if (reader.Value == null)
                {
                    return null;
                }
                var date = DateTime.Parse(reader.Value.ToString());
                date = DateTime.SpecifyKind(date, DateTimeKind.Utc);
                return Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(date);
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                if (value != null)
                {
                    writer.WriteValue(value.ToString());
                }
            }
        }
    }
}