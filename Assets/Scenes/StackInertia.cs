using Unity.Mathematics; // Adicionado, mas n�o estritamente necess�rio para este script. Usaremos UnityEngine.Quaternion.
using UnityEngine;

public class StackInertia : MonoBehaviour
{
    [Header("Configura��es da In�rcia")]
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

        // Calcula o n�vel na pilha (l�gica de hierarquia pai-filho encadeada)
        // Isso assume que o "pai" direto � o n�vel abaixo na pilha
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

        // --- CORRE��O PRINCIPAL: TRANSFORMAR VELOCIDADE PARA O ESPA�O LOCAL DO PAI ---
        // A velocidade global � convertida para a dire��o relativa � rota��o do pai.
        // Isso garante que a in�rcia seja aplicada "para tr�s" ou "para os lados"
        // em rela��o � ORIENTA��O ATUAL do pai.
        Vector3 parentVelocityLocal = Quaternion.Inverse(transform.parent.rotation) * parentVelocityGlobal;


        // Calcula a for�a de in�rcia para este n�vel, progressivamente maior para os de cima
        float currentInertiaStrength = baseInertiaStrength + (inertiaIncreasePerLevel * stackLevel);

        // Aplica o multiplicador de curvatura para exagerar o arco
        currentInertiaStrength *= curvatureMultiplier;

        // --- CALCULA A ROTA��O DE IN�RCIA USANDO A VELOCIDADE LOCALIZADA ---
        // Invertemos os sinais para in�rcia oposta ao movimento
        // Usamos parentVelocityLocal.z para o eixo X e parentVelocityLocal.x para o eixo Y e Z.
        // Note: Se o parentVelocityLocal.x � usado para Y e Z, a guinada e o roll ser�o correlacionados.
        // Se preferir o roll (eixo Z) independente, pode usar outro componente da velocidade ou ajust�-lo.
        Quaternion targetRotation = Quaternion.Euler(
            -parentVelocityLocal.z * currentInertiaStrength, // Rota��o no Eixo X (Pitch) - Inclina para tr�s/frente em rela��o ao pai
            -parentVelocityLocal.x * currentInertiaStrength, // Rota��o no Eixo Y (Yaw) - Inverte para esquerda/direita em rela��o ao pai
            parentVelocityLocal.x * currentInertiaStrength  // Rota��o no Eixo Z (Roll) - Inclina para os lados em rela��o ao pai
        );

        // Limita o �ngulo de inclina��o e guinada
        Vector3 eulerAngles = targetRotation.eulerAngles;

        eulerAngles.x = ClampAngle(eulerAngles.x, -maxTiltAngle, maxTiltAngle);
        eulerAngles.z = ClampAngle(eulerAngles.z, -maxTiltAngle, maxTiltAngle);
        eulerAngles.y = ClampAngle(eulerAngles.y, -maxYawAngle, maxYawAngle);

        targetRotation = Quaternion.Euler(eulerAngles);

        // Aplica o amortecimento para que a rota��o retorne ao normal suavemente.
        // O baixo valor de 'damping' aqui � crucial para o efeito "corda".
        transform.localRotation = Quaternion.Slerp(transform.localRotation, initialLocalRotation * targetRotation, Time.deltaTime * damping);
    }

    // Fun��o auxiliar para limitar �ngulos e evitar problemas com valores negativos
    float ClampAngle(float angle, float min, float max)
    {
        angle = angle % 360;
        if (angle > 180) angle -= 360;
        if (angle < -180) angle += 360;
        return Mathf.Clamp(angle, min, max);
    }
}