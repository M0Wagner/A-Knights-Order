using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public string itemId;
    
    public int quantity = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (InventoryData.instance != null && !string.IsNullOrEmpty(itemId))
            {
                InventoryData.instance.AddItem(itemId, quantity);
                
                Destroy(gameObject);
            }
        }
    }
}