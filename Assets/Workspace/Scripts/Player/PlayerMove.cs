using Mirror;
using UnityEngine;

public class PlayerMove : NetworkBehaviour
{
    [SyncVar] public Vector3 networkPosition = new Vector3();

    [SyncVar] public Quaternion networkRotation = new Quaternion();

    public Rigidbody rigidbody = null;

    private bool isInitialized = false;

    private float moveSpeed = 3f;

    private Player player = null;


    private void FixedUpdate()
    {
        if (!isInitialized)
        {
            return;
        }

        if (!isOwned)
        {
            transform.position = networkPosition;
            transform.rotation = networkRotation;
            return;
        }

        rigidbody.angularVelocity = Vector3.zero;
        rigidbody.velocity = new Vector3(InputManager.Movement.x * moveSpeed, 0f, InputManager.Movement.y * moveSpeed);
    }


    public void Initialize(Player player)
    {
        isInitialized = true;
        this.player = player;
        rigidbody.isKinematic = !isOwned;
    }
}