using UnityEngine;

public class LockedDoor : MonoBehaviour, IInteractable
{
    [SerializeField] private Sprite lockedSprite;
    [SerializeField] private Sprite openSprite;
    [SerializeField] private bool isLocked = true;

    [SerializeField] private string requiredItemId = "key";
    [SerializeField] private int quantityRequired = 1;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Collider2D physicalCollider;

    private void Awake()
    {
        UpdateDoorState();
    }

    public void Interact(PlayerMovement player)
    {
        if (!isLocked) return;

        if (InventoryData.instance.HasItem(requiredItemId, quantityRequired))
        {
            InventoryData.instance.DecreaseItem(requiredItemId, quantityRequired);
            
            Unlock();

            if (player.Interactable is LockedDoor door && door == this)
            {
                player.Interactable = null;
            }
        }
        else
        {
            Debug.Log("This door is locked. You need a key!");
        }
    }

    private void Unlock()
    {
        isLocked = false;
        UpdateDoorState();
        Debug.Log("Door Unlocked!");
    }

    private void UpdateDoorState()
    {
        spriteRenderer.sprite = isLocked ? lockedSprite : openSprite;
        
        if (physicalCollider != null)
        {
            physicalCollider.enabled = isLocked;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isLocked && collision.CompareTag("Player") && collision.TryGetComponent(out PlayerMovement player))
        {
            player.Interactable = this;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision.TryGetComponent(out PlayerMovement player))
        {
            if (player.Interactable is LockedDoor door && door == this)
            {
                player.Interactable = null;
            }
        }
    }
}