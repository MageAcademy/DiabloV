using System;
using System.Collections;
using Mirror;

public class Player : NetworkBehaviour
{
    [SyncVar] public ulong networkSteamID = 0L;

    public PlayerAppearance playerAppearance = null;

    public PlayerIdentity playerIdentity = null;

    public PlayerMove playerMove = null;

    public Action<string> onSteamNameValueChange = null;


    private void Start()
    {
        StartCoroutine(Initialize());
    }


    private IEnumerator Initialize()
    {
        while (true)
        {
            foreach (PlayerIdentity playerIdentity in PlayerIdentity.InstanceList)
            {
                if (networkSteamID == playerIdentity.networkSteamID)
                {
                    this.playerIdentity = playerIdentity;
                    break;
                }
            }

            if (playerMove != null)
            {
                break;
            }

            yield return null;
        }

        playerAppearance.Initialize(this);
        playerMove.Initialize(this);
    }
}