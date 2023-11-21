using Mirror;

public class PlayerMove : NetworkBehaviour
{
    private Player player = null;


    public void Initialize(Player player)
    {
        this.player = player;
    }
}