using System.Collections;
using UnityEngine;

public class AutoAttack : MonoBehaviour
{
    public float attackRange = 2f;
    public float attackCooldown = 1f;
    private float lastAttack;

    [Header("Animação")]
    public Animator animator;

    void Update()
    {
        if (Time.time - lastAttack < attackCooldown) return;

        Collider[] hits = Physics.OverlapSphere(transform.position, attackRange);
        foreach (var hit in hits)
        {
            var rag = hit.GetComponent<EnemyRagdoll>();
            if (rag != null && !rag.isRagdolled)
            {
                PunchAnimation();
                StartCoroutine(DelayActivateRagdoll(rag.gameObject, 0.5f));
                lastAttack = Time.time;
                break;
            }
        }
    }
    void PunchAnimation()
    {
        if (animator && animator.isActiveAndEnabled)
        {
            animator.SetTrigger("Punch");
            Invoke(nameof(ResetPunchBool), 0.5f);
        }
        else
        {
            Debug.LogWarning("Animator não atribuído ou desativado.");
        }
    }
    void ResetPunchBool()
    {
        animator.SetBool("Punch", false);
    }
    IEnumerator DelayActivateRagdoll(GameObject enemy, float delay)
    {
        yield return new WaitForSeconds(delay);
        Vector3 direction = (enemy.transform.position - transform.position).normalized;
        float punchForce = 100f;
        enemy.GetComponent<EnemyRagdoll>().ActivateRagdoll(direction * punchForce);

    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
#endif
}
