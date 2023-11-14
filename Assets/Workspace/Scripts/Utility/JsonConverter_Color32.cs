using System;
using Newtonsoft.Json;
using UnityEngine;

public class JsonConverter_Color32 : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(Color32);
    }


    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        return JsonConvert.DeserializeObject<Color32>(serializer.Deserialize(reader).ToString());
    }


    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        Color32 color = (Color32)value;
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