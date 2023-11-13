using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

public class AssetManager
{
    public static Dictionary<string, AssetBundle> AssetBundleDictionary = new Dictionary<string, AssetBundle>();


    public static void LoadAssetAsync(string bundleName, string assetName, Action<Object> onComplete)
    {
        GameManager.Instance.StartCoroutine(LoadAssetAsyncInternal(bundleName, assetName, onComplete));
    }


    private static IEnumerator LoadAssetAsyncInternal(string bundleName, string assetName, Action<Object> onComplete)
    {
        if (!AssetBundleDictionary.ContainsKey(bundleName))
        {
            string path = Path.Combine(Application.streamingAssetsPath, bundleName);
            AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(path);
            yield return request;
            if (request.assetBundle == null)
            {
                Debug.LogError($"没有Bundle: [{bundleName}]");
                yield break;
            }

            AssetBundleDictionary[bundleName] = request.assetBundle;
        }

        AssetBundle assetBundle = AssetBundleDictionary[bundleName];
        if (!assetBundle.Contains(assetName))
        {
            Debug.LogError($"Bundle: [{bundleName}]中没有Asset: [{assetName}]");
            yield break;
        }

        onComplete.Invoke(assetBundle.LoadAsset(assetName));
    }
}