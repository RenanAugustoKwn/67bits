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

        // Calcula o nível na pilha (lógica de hierarquia pai-filho encadeada)
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
        // A velocidade global é convertida para a direção relativa à rotação do pai.
        // Isso garante que a inércia seja aplicada "para trás" ou "para os lados"
        // em relação à ORIENTAÇÃO ATUAL do pai.
        Vector3 parentVelocityLocal = Quaternion.Inverse(transform.parent.rotation) * parentVelocityGlobal;


        // Calcula a força de inércia para este nível, progressivamente maior para os de cima
        float currentInertiaStrength = baseInertiaStrength + (inertiaIncreasePerLevel * stackLevel);

        // Aplica o multiplicador de curvatura para exagerar o arco
        currentInertiaStrength *= curvatureMultiplier;

        // --- CALCULA A ROTAÇÃO DE INÉRCIA USANDO A VELOCIDADE LOCALIZADA ---
        // Invertemos os sinais para inércia oposta ao movimento
        // Usamos parentVelocityLocal.z para o eixo X e parentVelocityLocal.x para o eixo Y e Z.
        // Note: Se o parentVelocityLocal.x é usado para Y e Z, a guinada e o roll serão correlacionados.
        // Se preferir o roll (eixo Z) independente, pode usar outro componente da velocidade ou ajustá-lo.
        Quaternion targetRotation = Quaternion.Euler(
            -parentVelocityLocal.z * currentInertiaStrength, // Rotação no Eixo X (Pitch) - Inclina para trás/frente em relação ao pai
            -parentVelocityLocal.x * currentInertiaStrength, // Rotação no Eixo Y (Yaw) - Inverte para esquerda/direita em relação ao pai
            parentVelocityLocal.x * currentInertiaStrength  // Rotação no Eixo Z (Roll) - Inclina para os lados em relação ao pai
        );

        // Limita o ângulo de inclinação e guinada
        Vector3 eulerAngles = targetRotation.eulerAngles;

        eulerAngles.x = ClampAngle(eulerAngles.x, -maxTiltAngle, maxTiltAngle);
        eulerAngles.z = ClampAngle(eulerAngles.z, -maxTiltAngle, maxTiltAngle);
        eulerAngles.y = ClampAngle(eulerAngles.y, -maxYawAngle, maxYawAngle);

        targetRotation = Quaternion.Euler(eulerAngles);

        // Aplica o amortecimento para que a rotação retorne ao normal suavemente.
        // O baixo valor de 'damping' aqui é crucial para o efeito "corda".
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
}