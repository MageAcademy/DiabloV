using Mirror;

public class PlayerAppearance : NetworkBehaviour
{
    private bool isInitialized = false;

    private Player player = null;


    public void Initialize(Player player)
    {
        isInitialized = true;
        this.player = player;
    }
}