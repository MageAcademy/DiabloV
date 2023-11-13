using Newtonsoft.Json;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance = null;

    public MapData[] mapData = null;


    private void Awake()
    {
        Instance = this;
    }


    public void Initialize()
    {
        AssetManager.LoadAssetAsync("text assets", "MapData", asset =>
        {
            TextAsset textAsset = asset as TextAsset;
            mapData = JsonConvert.DeserializeObject<MapData[]>(textAsset.text, JsonConverterManager.InstanceList);
        });
    }
}