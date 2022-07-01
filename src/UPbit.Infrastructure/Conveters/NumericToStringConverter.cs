using Newtonsoft.Json;
using System;

namespace Dreamrosia.Koin.UPbit.Infrastructure.Conveters
{
    public class NumericToStringConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return false;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            //writer.WriteValue(value.ToString());
            writer.WriteValue(NumericConverter.ToString((decimal)value));
        }
    }
}
