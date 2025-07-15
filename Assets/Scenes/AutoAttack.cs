using UnityEngine;

public class AutoAttack : MonoBehaviour
{
    public float attackRange = 2f;
    public float attackCooldown = 1f;
    private float lastAttack;

    void Update()
    {
        if (Time.time - lastAttack < attackCooldown) return;

        Collider[] hits = Physics.OverlapSphere(transform.position, attackRange);
        foreach (var hit in hits)
        {
            var rag = hit.GetComponent<EnemyRagdoll>();
            if (rag != null && !rag.isRagdolled)
            {
                rag.ActivateRagdoll();
                lastAttack = Time.time;
                break;
            }
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
#endif
}
