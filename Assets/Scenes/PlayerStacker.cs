using UnityEngine;
using System.Collections.Generic;

public class PlayerStacker : MonoBehaviour
{
    [Header("Referência")]
    public Transform stackPoint;           // arraste aqui o GameObject nas costas

    [Header("Configuração")]
    public float stackSpacingY = 1.2f;
    public float pickupRange = 2f;
    public float throwForce = 7f;

    private readonly List<GameObject> stacked = new();

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T)) TryPickupEnemy();
        if (Input.GetKeyDown(KeyCode.Y)) ThrowEnemy();
    }

    /* ---------- COLETAR ---------- */
    void TryPickupEnemy()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, pickupRange);
        foreach (var hit in hits)
        {
            if (!hit.CompareTag("Enemy")) continue;

            var rag = hit.GetComponent<EnemyRagdoll>();
            if (rag != null && rag.isRagdolled && !rag.isStacked)
            {
                Vector3 offset = Vector3.up * (stackSpacingY * stacked.Count);
                rag.StackOnto(stackPoint, offset);
                stacked.Add(hit.gameObject);
                break;                  // pega só um por vez
            }
        }
    }

    /* ---------- ARREMESSAR ---------- */
    void ThrowEnemy()
    {
        if (stacked.Count == 0) return;

        GameObject enemy = stacked[^1];
        stacked.RemoveAt(stacked.Count - 1);

        var rag = enemy.GetComponent<EnemyRagdoll>();
        Vector3 impulse = transform.forward * throwForce + Vector3.up * 3f;
        rag.UnstackAndActivatePhysics(impulse);
    }

    /* ---------- ENTREGA NA ZONA ---------- */
    public int DeliverEnemies()
    {
        int qtd = stacked.Count;
        foreach (var e in stacked) Destroy(e);
        stacked.Clear();
        return qtd;
    }
}
