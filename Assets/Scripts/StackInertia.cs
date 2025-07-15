using Unity.Mathematics; // Adicionado, mas não estritamente necessário para este script. Usaremos UnityEngine.Quaternion.
using UnityEngine;

public class StackInertia : MonoBehaviour
{
    [Header("Configurações da Inércia")]
    public float baseInertiaStrength = 1f;
    public float inertiaIncreasePerLevel = 3f;
    public float damping = 5f;
    public float maxTiltAngle = 30f;
    public float maxYawAngle = 5f;

    [Header("Efeito Arcado (Ajuste para Curvatura)")]
    public float curvatureMultiplier = 1.0f; 

    private Vector3 previousParentPosition;
    private Quaternion initialLocalRotation;
    private int stackLevel = 0;

    void Start()
    {
        if (transform.parent == null)
        {
            enabled = false;
            return;
        }

        previousParentPosition = transform.parent.position;
        initialLocalRotation = transform.localRotation;

        // Isso assume que o "pai" direto é o nível abaixo na pilha
        Transform current = transform.parent;
        while (current != null && current.GetComponent<StackInertia>() != null)
        {
            stackLevel++;
            current = current.parent;
        }
    }

    void Update()
    {
        // --- CALCULA A VELOCIDADE GLOBAL DO PAI ---
        Vector3 currentParentPosition = transform.parent.position;
        Vector3 parentVelocityGlobal = (currentParentPosition - previousParentPosition) / Time.deltaTime;
        previousParentPosition = currentParentPosition;

        // --- CORREÇÃO PRINCIPAL: TRANSFORMAR VELOCIDADE PARA O ESPAÇO LOCAL DO PAI ---
        Vector3 parentVelocityLocal = Quaternion.Inverse(transform.parent.rotation) * parentVelocityGlobal;


        // Calcula a força de inércia para este nível, progressivamente maior para os de cima
        float currentInertiaStrength = baseInertiaStrength + (inertiaIncreasePerLevel * stackLevel);

        // Aplica o multiplicador de curvatura para exagerar o arco
        currentInertiaStrength *= curvatureMultiplier;

        // --- CALCULA A ROTAÇÃO DE INÉRCIA USANDO A VELOCIDADE LOCALIZADA ---
        Quaternion targetRotation = Quaternion.Euler(
            -parentVelocityLocal.z * currentInertiaStrength, // Rotação no Eixo X (Pitch)
            -parentVelocityLocal.x * currentInertiaStrength, // Rotação no Eixo Y (Yaw)
            parentVelocityLocal.x * currentInertiaStrength  // Rotação no Eixo Z (Roll)
        );

        // Limita o ângulo de inclinação e guinada
        Vector3 eulerAngles = targetRotation.eulerAngles;

        eulerAngles.x = ClampAngle(eulerAngles.x, -maxTiltAngle, maxTiltAngle);
        eulerAngles.z = ClampAngle(eulerAngles.z, -maxTiltAngle, maxTiltAngle);
        eulerAngles.y = ClampAngle(eulerAngles.y, -maxYawAngle, maxYawAngle);

        targetRotation = Quaternion.Euler(eulerAngles);

        // Aplica o amortecimento para que a rotação retorne ao normal suavemente.
        transform.localRotation = Quaternion.Slerp(transform.localRotation, initialLocalRotation * targetRotation, Time.deltaTime * damping);
    }

    // Função auxiliar para limitar ângulos e evitar problemas com valores negativos
    float ClampAngle(float angle, float min, float max)
    {
        angle = angle % 360;
        if (angle > 180) angle -= 360;
        if (angle < -180) angle += 360;
        return Mathf.Clamp(angle, min, max);
    }
    public void ResetPreviousPosition()
    {
        if (transform.parent != null)
        {
            previousParentPosition = transform.parent.position;
        }
    }
}