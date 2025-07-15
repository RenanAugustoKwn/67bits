using UnityEngine;

[RequireComponent(typeof(Collider))]
public class EnemyHitbox : MonoBehaviour
{
    public EnemyRagdoll ragdoll;   // arraste a refer�ncia no Inspector

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerFist"))   // ou �Player� se estiver usando OverlapSphere
        {
            //ragdoll.ActivateRagdoll();
        }
    }
}
