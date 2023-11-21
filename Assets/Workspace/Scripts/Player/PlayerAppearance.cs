using Mirror;

public class PlayerAppearance : NetworkBehaviour
{
    private Player player = null;


    public void Initialize(Player player)
    {
        this.player = player;
    }
}
