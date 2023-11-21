using System.Collections.Generic;
using Mirror;
using Steamworks;

public class PlayerIdentity : NetworkBehaviour
{
    public static List<PlayerIdentity> InstanceList = new List<PlayerIdentity>();

    [SyncVar] public ulong networkSteamID = 0L;

    [SyncVar(hook = nameof(OnSteamNameValueChange))]
    public string networkSteamName = null;

    public Player player = null;


    private void Start()
    {
        InitializeServerRPC();
        InstanceList.Add(this);
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


    [Command]
    private void InitializeServerRPC()
    {
        networkSteamID = SteamUser.GetSteamID().m_SteamID;
        networkSteamName = SteamFriends.GetPersonaName();
    }


    [Command(requiresAuthority = false)]
    public void SpawnPlayerServerRPC(NetworkConnectionToClient conn = null)
    {
        if (networkSteamID == 0L || player != null)
        {
            return;
        }

        player = Instantiate(UNetworkManager.Instance.prefabPlayer);
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