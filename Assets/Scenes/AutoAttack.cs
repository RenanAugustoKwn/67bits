using UnityEngine;

public class AutoAttack : MonoBehaviour
{
    public float attackRange = 2f;
    public float attackCooldown = 1f;
    private float lastAttackTime;

    void Update()
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            Collider[] hitEnemies = Physics.OverlapSphere(transform.position, attackRange);
            foreach (Collider col in hitEnemies)
            {
                if (col.CompareTag("Enemy"))
                {
                    Debug.Log("Encostou!!!!!!");
                    EnemyRagdoll ragdoll = col.GetComponent<EnemyRagdoll>();
                    if (ragdoll != null)
                    {
                        ragdoll.ActivateRagdoll();
                        lastAttackTime = Time.time;
                        break;
                    }
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
