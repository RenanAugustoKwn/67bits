using UnityEngine;

public class DeliveryZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        var stacker = other.GetComponent<PlayerStacker>();
        if (stacker == null) return;

        int delivered = stacker.DeliverEnemies();
        if (delivered > 0)
        {
            int coins = delivered * 5;
            //PlayerCurrency.Add(coins);
            Debug.Log($"Entregou {delivered} inimigos: +{coins} moedas!");
        }
    }
}
