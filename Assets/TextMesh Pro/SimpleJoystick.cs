using UnityEngine;
using UnityEngine.EventSystems; // Necessário para detecção de eventos da UI
using UnityEngine.InputSystem;   // Necessário para interagir com o Input System

public class SimpleJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] private RectTransform handle; // Arraste o "Joystick Handle" aqui no Inspector
    [SerializeField] private float range = 50f;    // Distância máxima que o handle pode mover

    private Vector2 startPosition;
    private Vector2 inputVector; // O vetor de entrada que você vai usar para mover o player

    public Vector2 InputVector => inputVector; // Propriedade pública para acessar o input

    void Start()
    {
        // Posição inicial do joystick background (centro)
        startPosition = GetComponent<RectTransform>().position;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Quando o dedo toca, inicia o arrasto
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Calcula a direção e distância do toque em relação ao centro
        Vector2 direction = eventData.position - startPosition;

        // Limita a distância do handle para não sair do background
        if (direction.magnitude > range)
        {
            direction = direction.normalized * range;
        }

        // Move o handle
        handle.position = startPosition + direction;

        // Normaliza o vetor de entrada para valores entre -1 e 1
        inputVector = direction / range;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Quando o dedo levanta, reseta o handle para o centro e o input para zero
        handle.position = startPosition;
        inputVector = Vector2.zero;
    }
}