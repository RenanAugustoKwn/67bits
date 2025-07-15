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
                rag.ActivateRagdoll();
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

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
#endif
}
