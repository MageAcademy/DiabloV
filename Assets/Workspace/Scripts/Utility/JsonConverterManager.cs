using Newtonsoft.Json;

public class JsonConverterManager
{
    public static JsonConverter[] InstanceList =
    {
        new JsonConverter_Color(),
        new JsonConverter_Vector3()
    };
}