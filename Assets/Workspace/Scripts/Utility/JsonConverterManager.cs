using Newtonsoft.Json;

public class JsonConverterManager
{
    public static JsonConverter[] InstanceList =
    {
        new JsonConverter_Color32(),
        new JsonConverter_Vector3()
    };
}