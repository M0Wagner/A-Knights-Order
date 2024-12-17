using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NewBehaviourScript : MonoBehaviour
{
    public GameObject buttonLeft;
    public GameObject buttonRight;
    public GameObject buttonInteract;
    private bool waitingForInput = false;
    PlayerMovement playerMovement;

    private KeyCode keyForLeft = KeyCode.None;
    private KeyCode keyForRight = KeyCode.None;
    private KeyCode keyForJump = KeyCode.Space;
    private KeyCode keyForDash = KeyCode.LeftShift;
    private KeyCode keyForInteract = KeyCode.None;

    public void PlayGame()
    {
        // right now its the index File->Build Settings
        // name works also
        // sets the next level "SceneManager.GetActiveScene().buildIndex + 1"
        SceneManager.LoadScene(1);
    }

    public void Controls()
    {
        SceneManager.LoadScene(6);
    }


    // Controls "Level"
    public void Menu() {
        SceneManager.LoadScene(0);
    }

    public void ChangeMoveButton(GameObject targetImage)
    {
        waitingForInput = true;
        Debug.Log("Bitte eine Taste für " + targetImage.name + " drücken!");
    }

    void Update()
    {
        if (waitingForInput)
        {
            foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(key))
                {
                    if (key != KeyCode.None)
                    {
                        UpdateKeyImage(key);

                        waitingForInput = false;
                        Debug.Log("Taste gedrückt: " + key.ToString());
                    }
                }
            }
        }
    }

    void UpdateKeyImage(KeyCode key)
    {
        Sprite newSprite = null;

        if (key == KeyCode.A)
        {
            newSprite = Resources.Load<Sprite>("A");
        }
        else if (key == KeyCode.D)
        {
            newSprite = Resources.Load<Sprite>("D");
        }
        else if (key == KeyCode.LeftArrow)
        {
            newSprite = Resources.Load<Sprite>("LeftArrow");
        }
        else if (key == KeyCode.RightArrow)
        {
            newSprite = Resources.Load<Sprite>("RightArrow");
        }
        Debug.Log(newSprite + " | " + key);

        //if (newSprite != null)
        //{
            if (buttonLeft != null && waitingForInput)
            {
                buttonLeft.GetComponent<Image>().sprite = newSprite;
                keyForLeft = key;
                Debug.Log("Setting new Key for buttonLeft: " + keyForLeft);
                playerMovement.SetControls(keyForLeft, keyForRight, playerMovement.jumpKey, playerMovement.dashKey, playerMovement.interactKey);
            }
            else if (buttonRight != null && waitingForInput)
            {
                buttonRight.GetComponent<Image>().sprite = newSprite;
                keyForRight = key;
                playerMovement.SetControls(keyForLeft, keyForRight, playerMovement.jumpKey, playerMovement.dashKey, playerMovement.interactKey);
            }
            else if (buttonInteract != null && waitingForInput)
            {
                buttonInteract.GetComponent<Image>().sprite = newSprite;
                keyForInteract = key;
                playerMovement.SetControls(keyForLeft, keyForRight, playerMovement.jumpKey, playerMovement.dashKey, keyForInteract);
            }
        }
        //else
        //{
        //    Debug.LogError("Sprite konnte nicht geladen werden! Überprüfe den Pfad.");
        //}
    //}

    public void SaveChanges()
    {
        PlayerPrefs.SetString("MoveLeftKey", keyForLeft.ToString());
        PlayerPrefs.SetString("MoveRightKey", keyForRight.ToString());
        PlayerPrefs.SetString("JumpKey", keyForJump.ToString());
        PlayerPrefs.SetString("DashKey", keyForDash.ToString());
        PlayerPrefs.SetString("InteractKey", keyForInteract.ToString());

        PlayerPrefs.Save();
        Debug.Log("Key Left: " + keyForLeft + " Änderungen gespeichert.");
    }


    public void QuitGame()
    {
        Application.Quit();
    }
}
