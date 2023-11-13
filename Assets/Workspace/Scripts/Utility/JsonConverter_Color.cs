using System;
using Newtonsoft.Json;
using UnityEngine;

public class JsonConverter_Color : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(Color);
    }


    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        return JsonConvert.DeserializeObject<Color>(serializer.Deserialize(reader).ToString());
    }


    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        Color color = (Color)value;
        writer.WriteStartObject();
        writer.WritePropertyName("r");
        writer.WriteValue(color.r);
        writer.WritePropertyName("g");
        writer.WriteValue(color.g);
        writer.WritePropertyName("b");
        writer.WriteValue(color.b);
        writer.WritePropertyName("a");
        writer.WriteValue(color.a);
        writer.WriteEndObject();
    }
}