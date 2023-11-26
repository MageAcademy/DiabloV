using Mirror;

public class PlayerAppearance : NetworkBehaviour
{
    private Player player = null;

    private bool isInitialized = false;


    public void Initialize(Player player)
    {
        isInitialized = true;
        this.player = player;
    }
}
