using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementJoystick : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float smoothSpeed = 10f; // Quanto maior, mais rápido a transição
    private Vector2 moveInput;
    private Vector2 smoothedInput;
    private CharacterController controller;
    private Animator animator;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    void Update()
    {
        // Suaviza o input (keyboard ou joystick)
        smoothedInput = Vector2.Lerp(smoothedInput, moveInput, Time.deltaTime * smoothSpeed);

        Vector3 move = new Vector3(smoothedInput.x, 0, smoothedInput.y);
        controller.SimpleMove(move * moveSpeed);

        if (move != Vector3.zero)
            transform.forward = move;

        animator.SetFloat("Speed", move.magnitude); // Atualiza blend ou transições
    }
}
