using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveActivator : MonoBehaviour, IInteractable
{
    public bool isInRange;

    public void Interact(PlayerMovement player)
    {
        Debug.Log("Player interacted with the object");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isInRange = true;
            //player.Interactable = this;
            Debug.Log("Save is now interactable");
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isInRange = false;
            //player.Interactable = null;
            Debug.Log("Save is now not interactable");
        }
    }
}