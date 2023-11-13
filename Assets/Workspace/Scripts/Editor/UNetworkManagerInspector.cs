using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(UNetworkManager))]
public class UNetworkManagerInspector : Editor
{
    protected UNetworkManager networkManager = null;

    private ReorderableList spawnList = null;

    private SerializedProperty spawnListProperty = null;


    protected void Init()
    {
        if (spawnList == null)
        {
            networkManager = target as UNetworkManager;
            spawnListProperty = serializedObject.FindProperty("spawnPrefabs");
            spawnList = new ReorderableList(serializedObject, spawnListProperty)
            {
                drawHeaderCallback = DrawHeader,
                drawElementCallback = DrawChild,
                onReorderCallback = Changed,
                onRemoveCallback = RemoveButton,
                onChangedCallback = Changed,
                onAddCallback = AddButton,
                elementHeight = 16
            };
        }
    }


    public override void OnInspectorGUI()
    {
        Init();
        networkManager.isClient = EditorGUILayout.Toggle("Is Client", networkManager.isClient);
        networkManager.isLocal = EditorGUILayout.Toggle("Is Local", networkManager.isLocal);
        networkManager.isServer = EditorGUILayout.Toggle("Is Server", networkManager.isServer);
        DrawDefaultInspector();
        EditorGUI.BeginChangeCheck();
        spawnList.DoLayoutList();
        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
        }

        if (GUILayout.Button("Populate Spawnable Prefabs"))
        {
            ScanForNetworkIdentities();
        }
    }


    void ScanForNetworkIdentities()
    {
        List<GameObject> identities = new List<GameObject>();
        bool cancelled = false;
        try
        {
            string[] paths = EditorHelper.IterateOverProject("t:prefab").ToArray();
            int count = 0;
            foreach (string path in paths)
            {
                if (path.Contains("Mirror/Tests/") ||
                    path.Contains("Mirror/Examples/"))
                {
                    continue;
                }

                if (EditorUtility.DisplayCancelableProgressBar("Searching for NetworkIdentities..",
                        $"Scanned {count}/{paths.Length} prefabs. Found {identities.Count} new ones",
                        count / (float)paths.Length))
                {
                    cancelled = true;
                    break;
                }

                count++;
                NetworkIdentity ni = AssetDatabase.LoadAssetAtPath<NetworkIdentity>(path);
                if (!ni)
                {
                    continue;
                }

                if (!networkManager.spawnPrefabs.Contains(ni.gameObject))
                {
                    identities.Add(ni.gameObject);
                }
            }
        }
        finally
        {
            EditorUtility.ClearProgressBar();
            if (!cancelled)
            {
                Undo.RecordObject(networkManager, "NetworkManager: populated prefabs");
                networkManager.spawnPrefabs.AddRange(identities);
                networkManager.spawnPrefabs = networkManager.spawnPrefabs.OrderBy(go => go.name).ToList();
                EditorUtility.SetDirty(target);
            }

            Resources.UnloadUnusedAssets();
        }
    }


    static void DrawHeader(Rect headerRect)
    {
        GUI.Label(headerRect, "Registered Spawnable Prefabs:");
    }


    internal void DrawChild(Rect r, int index, bool isActive, bool isFocused)
    {
        SerializedProperty prefab = spawnListProperty.GetArrayElementAtIndex(index);
        GameObject go = (GameObject)prefab.objectReferenceValue;
        GUIContent label;
        if (go == null)
        {
            label = new GUIContent("Empty", "Drag a prefab with a NetworkIdentity here");
        }
        else
        {
            NetworkIdentity identity = go.GetComponent<NetworkIdentity>();
            label = new GUIContent(go.name,
                identity != null ? $"AssetId: [{identity.assetId}]" : "No Network Identity");
        }

        GameObject newGameObject = (GameObject)EditorGUI.ObjectField(r, label, go, typeof(GameObject), false);
        if (newGameObject != go)
        {
            if (newGameObject != null && !newGameObject.GetComponent<NetworkIdentity>())
            {
                Debug.LogError(
                    $"Prefab {newGameObject} cannot be added as spawnable as it doesn't have a NetworkIdentity.");
                return;
            }

            prefab.objectReferenceValue = newGameObject;
        }
    }


    internal void Changed(ReorderableList list)
    {
        EditorUtility.SetDirty(target);
    }


    internal void AddButton(ReorderableList list)
    {
        spawnListProperty.arraySize += 1;
        list.index = spawnListProperty.arraySize - 1;
        SerializedProperty obj = spawnListProperty.GetArrayElementAtIndex(spawnListProperty.arraySize - 1);
        obj.objectReferenceValue = null;
        spawnList.index = spawnList.count - 1;
        Changed(list);
    }


    internal void RemoveButton(ReorderableList list)
    {
        spawnListProperty.DeleteArrayElementAtIndex(spawnList.index);
        if (list.index >= spawnListProperty.arraySize)
        {
            list.index = spawnListProperty.arraySize - 1;
        }
    }
}