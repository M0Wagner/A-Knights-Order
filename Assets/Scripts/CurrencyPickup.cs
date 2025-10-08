using UnityEngine;

public class CurrencyPickup : MonoBehaviour
{
    public int pickupQuantity = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            InventoryData.instance.AddCoins(pickupQuantity);
            
            Destroy(gameObject);
        }
    }
}