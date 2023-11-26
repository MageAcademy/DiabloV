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

    private bool checkNull = false;


    private void Start()
    {
        StartCoroutine(Initialize());
    }


    private void Update()
    {
        CheckNull();
    }


    private void CheckNull()
    {
        if (checkNull && playerIdentity == null)
        {
            Destroy(gameObject);
        }
    }


    private IEnumerator Initialize()
    {
        while (true)
        {
            playerIdentity =
                PlayerIdentity.InstanceList.Find(playerIdentity => playerIdentity.networkSteamID == networkSteamID);
            if (playerIdentity != null)
            {
                break;
            }

            yield return null;
        }

        checkNull = true;
        playerAppearance.Initialize(this);
        playerMove.Initialize(this);
    }
}