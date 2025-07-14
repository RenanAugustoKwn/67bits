using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementJoystick : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Vector2 moveInput;
    private CharacterController controller;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    void Update()
    {
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);
        controller.SimpleMove(move * moveSpeed);
        if (move != Vector3.zero)
            transform.forward = move;
    }
}
