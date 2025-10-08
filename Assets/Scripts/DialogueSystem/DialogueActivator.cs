using System.Collections;
using UnityEngine;

public class DialogueActivator : MonoBehaviour, IInteractable
{
    [SerializeField] private DialogueObject dialogueObject;

    [SerializeField] private bool giveOnce = true;
    [SerializeField] private string playerPrefsFlagKey = "Princess_KeyGiven";
    [SerializeField] private int keyAmount = 1;

    [SerializeField] private UIInventoryPage uiInventoryPage;
    [SerializeField] private Sprite uiKeyIcon;

    private bool alreadyGiven;
    private bool waitingForDialogue;

    private void Awake()
    {
        alreadyGiven = PlayerPrefs.GetInt(playerPrefsFlagKey, 0) == 1;
    }

    public void Interact(PlayerMovement player)
    {
        player.DialogueUI.ShowDialogue(dialogueObject);

        if (!alreadyGiven && !waitingForDialogue)
        {
            StartCoroutine(GiveRewardWhenDialogueEnds(player));
        }
    }

    private IEnumerator GiveRewardWhenDialogueEnds(PlayerMovement player)
    {
        waitingForDialogue = true;

        // Wait until the dialogue is closed, then give item to player
        yield return new WaitWhile(() => player.DialogueUI.IsOpen);

        Debug.Log("Dialogue ended, giving key");
        if (!alreadyGiven)
        {
            InventoryData.instance.AddItem("key", keyAmount);
            Debug.Log("Key given to player");
            alreadyGiven = true;

            if (giveOnce)
            {
                PlayerPrefs.SetInt(playerPrefsFlagKey, 1);
                PlayerPrefs.Save();
                Debug.Log("Saved player prefs flag");
            }
        }

        waitingForDialogue = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision.TryGetComponent(out PlayerMovement player))
        {
            player.Interactable = this;
            Debug.Log("Princess is now interactable");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision.TryGetComponent(out PlayerMovement player))
        {
            if (player.Interactable is DialogueActivator dialogueActivator && dialogueActivator == this)
                player.Interactable = null;
        }
    }
}