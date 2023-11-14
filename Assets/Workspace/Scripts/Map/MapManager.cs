using System.Collections;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.ProBuilder;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance = null;

    public MapData[] mapData = null;

    public Transform parentMap = null;


    private void Awake()
    {
        Instance = this;
    }


    public void Initialize()
    {
        StartCoroutine(Generate());
    }


    private IEnumerator Generate()
    {
        yield return AssetManager.LoadAssetAsyncInternal("text assets", "MapData", asset =>
        {
            TextAsset textAsset = asset as TextAsset;
            mapData = JsonConvert.DeserializeObject<MapData[]>(textAsset.text, JsonConverterManager.InstanceList);
        });

        int mapBlockSize = 13;
        Vector2Int mapBlockLength = new Vector2Int(3, 3);
        int mapBlockCount = mapBlockLength.x * mapBlockLength.y;
        int mapDataLength = mapData.Length;
        if (mapBlockCount > mapDataLength)
        {
            yield break;
        }

        yield return AssetManager.LoadAssetAsyncInternal("prefabs", "Soil Base", asset =>
        {
            GameObject prefabSoilBase = asset as GameObject;

            // IL2CPP无法通过反射机制手动加载程序集，但可以通过引用来触发自动加载
            prefabSoilBase.GetComponent<ProBuilderMesh>();
            ////////////

            Instantiate(prefabSoilBase, parentMap).transform.localPosition = new Vector3(
                mapBlockLength.x * mapBlockSize % 2 == 0 ? 0f : 0.5f, -1.01f,
                mapBlockLength.y * mapBlockSize % 2 == 0 ? 0f : 0.5f);
        });

        yield return AssetManager.LoadAssetAsyncInternal("prefabs", "Soil", asset =>
        {
            GameObject prefabSoil = asset as GameObject;
            int[] mapDataIndices = new int[mapDataLength];
            for (int a = 0; a < mapDataLength; ++a)
            {
                mapDataIndices[a] = a;
            }

            for (int y = 0; y < mapBlockLength.y; ++y)
            {
                for (int x = 0; x < mapBlockLength.x; ++x)
                {
                    int mapBlockIndex = y * mapBlockLength.x + x;
                    int randomIndex = Random.Range(mapBlockIndex, mapDataLength);
                    (mapDataIndices[mapBlockIndex], mapDataIndices[randomIndex]) =
                        (mapDataIndices[randomIndex], mapDataIndices[mapBlockIndex]);
                    MapData data = mapData[mapDataIndices[mapBlockIndex]];
                    GameObject mapBlock = new GameObject(data.blockName);
                    Transform transform = mapBlock.transform;
                    transform.SetParent(parentMap);
                    transform.localPosition = new Vector3((x - (mapBlockLength.x - 1f) / 2f) * mapBlockSize, 0f,
                        (y - (mapBlockLength.y - 1f) / 2f) * mapBlockSize);
                    GameObject soil = Instantiate(prefabSoil, transform);
                    transform = soil.transform;
                    transform.localPosition = Vector3.down;
                    transform.localScale = new Vector3(mapBlockSize, 2f, mapBlockSize);
                    soil.GetComponent<MeshRenderer>().material.color = (Color)data.soilColor;
                }
            }
        });
    }
}