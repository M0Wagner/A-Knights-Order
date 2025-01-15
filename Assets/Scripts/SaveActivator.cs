using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SaveActivator : MonoBehaviour, IInteractable
{
    public bool isInRange;
    public KeyCode interactKey;
    public UnityEvent interactAction;

    private void Update()
    {
        if (isInRange)
        {
            if (Input.GetKeyDown(interactKey))
            {
                interactAction.Invoke();
            }
        }
    }

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