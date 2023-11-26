using System.Collections.Generic;
using Mirror;
using Steamworks;
using UnityEngine;

public class PlayerIdentity : NetworkBehaviour
{
    public static List<PlayerIdentity> InstanceList = new List<PlayerIdentity>();

    public static PlayerIdentity LocalPlayer = null;

    [SyncVar] public ulong networkSteamID = 0L;

    [SyncVar(hook = nameof(OnSteamNameValueChange))]
    public string networkSteamName = null;

    public Player player = null;


    private void Start()
    {
        Debug.LogError($"{isOwned},{isLocalPlayer},{isClient},{isClientOnly},{isServer},{isServerOnly}");
        InstanceList.Add(this);
        InitializeOnLocalPlayer();
    }


    private void Test1()
    {
        SpawnPlayerServerRPC(new Vector3(Random.Range(-3f, 3f), 0f, Random.Range(-3f, 3f)));
    }


    private void Test2()
    {
        UnSpawnPlayerServerRPC();
    }


    private void OnDestroy()
    {
        InstanceList.Remove(this);
    }


    private void OnSteamNameValueChange(string _, string newValue)
    {
        if (player == null)
        {
            return;
        }

        player.onSteamNameValueChange.Invoke(newValue);
    }


    [Client]
    private void InitializeOnLocalPlayer()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        LocalPlayer = this;
        InitializeServerRPC();
        //InvokeRepeating(nameof(Test1), 2f, 6f);
        //InvokeRepeating(nameof(Test2), 5f, 6f);
    }


    [Command]
    private void InitializeServerRPC()
    {
        networkSteamID = SteamUser.GetSteamID().m_SteamID;
        networkSteamName = SteamFriends.GetPersonaName();
    }


    [Command(requiresAuthority = false)]
    public void SpawnPlayerServerRPC(Vector3 position, NetworkConnectionToClient conn = null)
    {
        if (networkSteamID == 0L || player != null)
        {
            return;
        }

        player = Instantiate(UNetworkManager.Instance.prefabPlayer, position, Quaternion.identity);
        player.networkSteamID = networkSteamID;
        NetworkServer.Spawn(player.gameObject, conn);
    }


    [Command(requiresAuthority = false)]
    public void UnSpawnPlayerServerRPC()
    {
        if (player == null)
        {
            return;
        }

        NetworkServer.UnSpawn(player.gameObject);
        Destroy(player.gameObject);
    }
}