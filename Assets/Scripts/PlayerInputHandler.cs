using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerInputActions actions;
    private PlayerMovementJoystick movement;

    void Awake()
    {
        actions = new PlayerInputActions();
        movement = GetComponent<PlayerMovementJoystick>();
    }

    void OnEnable() => actions.Enable();
    void OnDisable() => actions.Disable();

    void Start()
    {
        actions.Player.Move.performed += movement.OnMove;
        actions.Player.Move.canceled += movement.OnMove;
    }
}
