using Mirror;

public class PlayerMove : NetworkBehaviour
{
    private Player player = null;

    private bool isInitialized = false;


    public void Initialize(Player player)
    {
        isInitialized = true;
        this.player = player;
    }
}