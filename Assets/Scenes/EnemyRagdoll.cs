using UnityEngine;

public class EnemyRagdoll : MonoBehaviour
{
    public Rigidbody[] ragdollBodies;

    void Start()
    {
        foreach (var rb in ragdollBodies)
        {
            rb.isKinematic = true;
        }
    }

    public void ActivateRagdoll()
    {
        foreach (var rb in ragdollBodies)
        {
            rb.isKinematic = false;
        }

        Destroy(GetComponent<Animator>());
    }
}
