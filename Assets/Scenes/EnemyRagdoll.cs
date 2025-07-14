using UnityEngine;

[RequireComponent(typeof(Collider))]
public class EnemyRagdoll : MonoBehaviour
{
    [Header("Arraste TODOS os Rigidbodies dos ossos aqui")]
    public Rigidbody[] ragdollBodies;

    [HideInInspector] public bool isRagdolled;
    [HideInInspector] public bool isStacked;

    private bool delivered;        // evita múltiplas recompensas

    /* ---------- Ragdoll ON ---------- */
    public void ActivateRagdoll()
    {
        if (isRagdolled) return;

        foreach (var rb in ragdollBodies)
        {
            rb.isKinematic = false;
            rb.GetComponent<Collider>().enabled = true;
        }

        var anim = GetComponent<Animator>();
        if (anim) Destroy(anim);

        // habilita collider raiz para detecção de coleta
        var rootCol = GetComponent<Collider>();
        if (rootCol) { rootCol.enabled = true; rootCol.isTrigger = false; }

        isRagdolled = true;
        isStacked = false;
    }

    /* ---------- Empilhar ---------- */
    public void StackOnto(Transform parent, Vector3 offset)
    {
        if (!isRagdolled || isStacked) return;

        foreach (var rb in ragdollBodies)
        {
            rb.isKinematic = true;
            rb.GetComponent<Collider>().enabled = false;
        }

        var rootCol = GetComponent<Collider>();
        if (rootCol) rootCol.enabled = false;

        transform.SetParent(parent);
        transform.localPosition = offset;
        transform.localRotation = Quaternion.identity;

        isStacked = true;
    }

    /* ---------- Desempilhar + arremessar ---------- */
    public void UnstackAndActivatePhysics(Vector3 impulse)
    {
        if (!isStacked) return;

        transform.SetParent(null);

        foreach (var rb in ragdollBodies)
        {
            rb.isKinematic = false;
            rb.GetComponent<Collider>().enabled = true;
        }

        var rootCol = GetComponent<Collider>();
        if (rootCol) { rootCol.enabled = true; rootCol.isTrigger = false; }

        isStacked = false;

        ragdollBodies[0].AddForce(impulse, ForceMode.Impulse);
    }

    /* ---------- Recompensa no chão verde ---------- */
    private void OnCollisionEnter(Collision col)
    {
        if (delivered) return;

        if (col.collider.CompareTag("GreenGround"))
        {
            delivered = true;
            //PlayerCurrency.Add(10);             // +10 moedas
            Destroy(gameObject);                // some da cena
        }
    }
}
