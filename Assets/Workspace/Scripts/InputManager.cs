using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static bool IsMoveValid = false;

    public static Vector2 Movement = new Vector2();


    private void Awake()
    {
        IsMoveValid = false;
        Movement = new Vector2();
    }


    public void OnMove(InputAction.CallbackContext ctx)
    {
        IsMoveValid = ctx.performed;
        Movement = ctx.ReadValue<Vector2>();
    }
}