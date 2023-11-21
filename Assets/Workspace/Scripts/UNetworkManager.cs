using Mirror;
using UnityEngine;

public class UNetworkManager : NetworkManager
{
    public static UNetworkManager Instance = null;

    [Header("Custom")] public bool isClient = true;

    public bool isLocal = false;

    public bool isServer = true;

    public Player prefabPlayer = null;

    private string localAddress = "127.0.0.1";

    private ushort localPort = 9420;


    public override void Awake()
    {
        base.Awake();
        Instance = this;
        PlayerIdentity.InstanceList.Clear();
    }


    public override void ConfigureHeadlessFrameRate()
    {
    }


    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        NetworkServer.AddPlayerForConnection(conn, Instantiate(playerPrefab));
    }


    public void Initialize()
    {
        TelepathyTransport telepathyTransport = transport as TelepathyTransport;
        if (isServer)
        {
            networkAddress = localAddress;
            telepathyTransport.port = localPort;
            if (isClient)
            {
                Application.targetFrameRate = SettingManager.Data.targetFrameRate;
                StartHost();
            }
            else
            {
                Application.targetFrameRate = sendRate;
                StartServer();
            }
        }
        else
        {
            Application.targetFrameRate = SettingManager.Data.targetFrameRate;
            networkAddress = SettingManager.Data.remoteAddress;
            telepathyTransport.port = SettingManager.Data.remotePort;
            StartClient();
        }
    }
}