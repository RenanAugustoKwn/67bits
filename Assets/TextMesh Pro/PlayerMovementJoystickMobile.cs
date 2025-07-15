using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementJoystickMobile : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float smoothSpeed = 10f;
    private Vector2 moveInput; // Isso continuar� funcionando para teclado/gamepad
    private Vector2 smoothedInput;
    private CharacterController controller;
    private Animator animator;

    // Adicione esta refer�ncia ao seu SimpleJoystick
    public SimpleJoystick uiJoystick;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();

        // Tenta encontrar o SimpleJoystick na cena se n�o for atribu�do no Inspector
        if (uiJoystick == null)
        {
            uiJoystick = FindObjectOfType<SimpleJoystick>();
            if (uiJoystick == null)
            {
                Debug.LogWarning("SimpleJoystick n�o encontrado! O movimento do jogador pode n�o funcionar.");
            }
        }
    }

    // Mant�m este m�todo para input de teclado/gamepad (opcional)
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    void Update()
    {
        Vector2 currentInput = moveInput; // Come�a com o input de teclado/gamepad

        // Se o joystick da UI estiver dispon�vel e sendo usado, ele sobrescreve o input
        if (uiJoystick != null && uiJoystick.InputVector != Vector2.zero)
        {
            currentInput = uiJoystick.InputVector;
        }
        // Caso contr�rio, se o joystick n�o estiver sendo usado, ele usa o input de teclado/gamepad (se houver)

        // Suaviza o input
        smoothedInput = Vector2.Lerp(smoothedInput, currentInput, Time.deltaTime * smoothSpeed);

        Vector3 move = new Vector3(smoothedInput.x, 0, smoothedInput.y);
        controller.SimpleMove(move * moveSpeed);

        if (move != Vector3.zero)
        {
            // Rota��o suave do personagem
            transform.forward = Vector3.Slerp(transform.forward, move, Time.deltaTime * smoothSpeed);
        }

        animator.SetFloat("Speed", move.magnitude);
    }
}