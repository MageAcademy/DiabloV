using UnityEngine;

public class Test : MonoBehaviour
{
    private void Start()
    {
    }


    private void Update()
    {
    }


    public void CreatePlayer()
    {
        PlayerIdentity.LocalPlayer.SpawnPlayerServerRPC(new Vector3(Random.Range(-3f, 3f), 0f, Random.Range(-3f, 3f)));
    }


    public void DestroyPlayer()
    {
        PlayerIdentity.LocalPlayer.UnSpawnPlayerServerRPC();
    }


    public void CreateMap()
    {
        MapManager.Instance.Initialize();
    }
}