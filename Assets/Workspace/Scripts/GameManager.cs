using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance = null;


    private void Awake()
    {
        Instance = this;
    }


    private void Start()
    {
        AssetManager.AssetBundleDictionary = new Dictionary<string, AssetBundle>();
        SettingManager.Load();
        UNetworkManager.Instance?.Initialize();
    }
}