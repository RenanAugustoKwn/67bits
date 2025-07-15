using UnityEngine;
using System.Collections.Generic;

public class PlayerStacker : MonoBehaviour
{
    [Header("Camadas")]
    public LayerMask greenGroundMask;   // atribua “GreenZone” no Inspector
    public float greenGroundCheckRadius = 0.9f;

    [Header("Referência")]
    public Transform stackPoint;            // objeto vazio nas costas
    public GameController gameController;
    public GameObject[] iconsImage;

    [Header("Configuração")]
    public float stackSpacingY = 1.2f;
    public float pickupRange = 2f;

    private readonly List<EnemyRagdoll> stacked = new();
    private bool isInGreenGround = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T) || Input.GetKeyDown(KeyCode.JoystickButton2)) TryPickupEnemy();
        if (Input.GetKeyDown(KeyCode.Y) || Input.GetKeyDown(KeyCode.JoystickButton1)) DropEnemy();
        isInGreenGround = Physics.CheckSphere(transform.position, greenGroundCheckRadius, greenGroundMask);
        UpdatePickupIcon();
    }
    /* ---------- Mostrar / esconder ícone ---------- */
    void UpdatePickupIcon()
    {
        if (iconsImage.Length < 2) return;

        if (isInGreenGround)
        {
            iconsImage[0].SetActive(false);  // Desativa X
            iconsImage[1].SetActive(true);   // Ativa B
        }
        else
        {
            bool canPickup = false;

            Collider[] hits = Physics.OverlapSphere(transform.position, pickupRange);
            foreach (var h in hits)
            {
                EnemyRagdoll rag = h.GetComponentInParent<EnemyRagdoll>();
                if (rag != null && rag.isRagdolled && !rag.isStacked && rag.pickupBool)
                {
                    canPickup = true;
                    break;
                }
            }

            iconsImage[0].SetActive(canPickup); // Ativa X se pode pegar
            iconsImage[1].SetActive(false);     // Desativa B fora da área
        }
    }
    /* ---------- Coletar ---------- */
    void TryPickupEnemy()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, pickupRange);

                // Limite com base no level atual
        if (stacked.Count >= gameController.level)
        {
            Debug.Log("Limite de empilhamento atingido para este nível.");
            return;
        }

        foreach (var h in hits)
        {
            EnemyRagdoll rag = h.GetComponentInParent<EnemyRagdoll>();
            if (rag != null && rag.isRagdolled && !rag.isStacked && rag.pickupBool)
            {
                Vector3 offset = Vector3.up * (stackSpacingY);
                if (stacked.Count != 0)
                {
                    rag.StackOnto(stacked[stacked.Count - 1].transform, offset);
                    stacked[stacked.Count - 1].gameObject.AddComponent<StackInertia>();
                }
                else
                {
                    rag.StackOnto(stackPoint, offset);
                }
                stacked.Add(rag);
                rag.gameObject.GetComponent<CapsuleCollider>().isTrigger = true;
                break;
            }
        }
    }

    /* ---------- Soltar ---------- */
    void DropEnemy()
    {
        if (stacked.Count == 0) return;
        EnemyRagdoll rag = stacked[^1];
        Destroy(rag.gameObject.GetComponent<StackInertia>());
        stacked.RemoveAt(stacked.Count - 1);
        rag.UnstackAndDrop();                            // cai naturalmente, Boy separado

    }
}
