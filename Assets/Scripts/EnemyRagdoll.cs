using UnityEngine;

[RequireComponent(typeof(Collider))]
public class EnemyRagdoll : MonoBehaviour
{
    [Header("Referências (arraste no Inspector)")]
    public Transform boyRoot;          // objeto “Boy”
    public Transform hips;             // osso Hips do ragdoll
    public Rigidbody[] ragdollBodies;  // todos os Rigidbodies dos ossos

    private Collider capsuleCol;       // CapsuleCollider do Enemy
    private Rigidbody rigidbodyEnemy;
    [HideInInspector] public bool isRagdolled;
    [HideInInspector] public bool isStacked;
    private bool delivered;
    public bool pickupBool = true;

    void Awake()
    {
        capsuleCol = GetComponent<Collider>();
        rigidbodyEnemy = GetComponent<Rigidbody>();
    }

    /* ---------- Derrubar ---------- */
    public void ActivateRagdoll(Vector3 forceDirection)
    {
        if (isRagdolled) return;

        boyRoot.SetParent(null); // Boy sai da hierarquia

        foreach (var rb in ragdollBodies) // Ativa física
        {
            rb.isKinematic = false;
            var c = rb.GetComponent<Collider>();
            if (c) c.enabled = true;
        }

        var anim = boyRoot.GetComponentInChildren<Animator>();
        if (anim) Destroy(anim);

        if (capsuleCol)
        {
            capsuleCol.isTrigger = true; // evita colisão com o ragdoll
            rigidbodyEnemy.constraints = RigidbodyConstraints.FreezeAll;
        }

        isRagdolled = true;
        isStacked = false;

        if (ragdollBodies.Length > 0)
        {
            ragdollBodies[0].AddForce(forceDirection, ForceMode.Impulse);
        }
    }

    /* ---------- Enemy segue o Hips ---------- */
    void FixedUpdate()
    {
        if (isRagdolled && !isStacked && hips != null)
            transform.position = hips.position;
    }

    /* ---------- Empilhar ---------- */
    public void StackOnto(Transform parent, Vector3 offset)
    {
        if (!isRagdolled || isStacked) return;

        foreach (var rb in ragdollBodies)
        {
            rb.isKinematic = true;
            var c = rb.GetComponent<Collider>();
            if (c) c.enabled = false;
        }

        boyRoot.SetParent(transform);                  // Boy volta como filho
        boyRoot.localPosition = Vector3.zero;
        hips.localPosition = Vector3.zero;
        boyRoot.localRotation = Quaternion.identity;

        if (capsuleCol) capsuleCol.isTrigger = false;  // volta a colidir na pilha

        transform.SetParent(parent);                   // empilha no player
        transform.localPosition = offset;
        transform.localRotation = Quaternion.identity;

        isStacked = true;
    }

    /* ---------- Soltar (sem impulso) ---------- */
    public void UnstackAndDrop()
    {
        pickupBool = false;
        if (!isStacked) return;

        transform.SetParent(null);                     // sai da pilha

        // Boy sai da hierarquia para novo ciclo
        boyRoot.SetParent(null);

        foreach (var rb in ragdollBodies)
        {
            rb.isKinematic = false;
            var c = rb.GetComponent<Collider>();
            if (c) c.enabled = true;
        }

        if (capsuleCol) capsuleCol.isTrigger = true;   // evita colisão

        isStacked = false;                             // corpo cai naturalmente
        // continua em ragdoll, pronto para ser recolhido com T
    }

    /* ---------- Destruir no chão verde ---------- */
    private void OnTriggerEnter(Collider other)
    {
        if (!delivered && other.CompareTag("GreenGround"))
        {
            pickupBool = false;
            delivered = true;
            var gameController = FindAnyObjectByType<GameController>();
            gameController.AddCoins(1);
            Destroy(boyRoot.gameObject, 0.5f);
            Destroy(gameObject,1f);                       // remove Enemy inteiro
        }
        else if(other.CompareTag("Ground"))
        {
            pickupBool = true;
        }
    }
}
