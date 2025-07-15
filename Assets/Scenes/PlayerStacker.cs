using UnityEngine;
using System.Collections.Generic;

public class PlayerStacker : MonoBehaviour
{
    [Header("Referência")]
    public Transform stackPoint;            // objeto vazio nas costas

    [Header("Configuração")]
    public float stackSpacingY = 1.2f;
    public float pickupRange = 2f;

    private readonly List<EnemyRagdoll> stacked = new();

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T)) TryPickupEnemy();
        if (Input.GetKeyDown(KeyCode.Y)) DropEnemy();
    }

    /* ---------- Coletar ---------- */
    void TryPickupEnemy()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, pickupRange);
        foreach (var h in hits)
        {
            EnemyRagdoll rag = h.GetComponentInParent<EnemyRagdoll>();
            if (rag != null && rag.isRagdolled && !rag.isStacked)
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
