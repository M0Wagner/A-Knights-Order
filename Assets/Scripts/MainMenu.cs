using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NewBehaviourScript : MonoBehaviour
{
    public Button buttonLeft;
    public Button buttonRight;
    private bool waitingForInput = false;
    public PlayerMovement playerMovement;

    private KeyCode keyForLeft = KeyCode.None;
    private KeyCode keyForRight = KeyCode.None;

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
        string path = "Menu/Controls/KeyImages/";

        Sprite newSprite = null;

        if (key == KeyCode.A)
        {
            newSprite = Resources.Load<Sprite>(path + "A");
        }
        else if (key == KeyCode.D)
        {
            newSprite = Resources.Load<Sprite>(path + "D");
        }
        else if (key == KeyCode.LeftArrow)
        {
            newSprite = Resources.Load<Sprite>(path + "LeftArrow");
        }
        else if (key == KeyCode.RightArrow)
        {
            newSprite = Resources.Load<Sprite>(path + "RightArrow");
        }

        if (newSprite != null)
        {
            if (buttonLeft != null && waitingForInput && keyForLeft == KeyCode.None)
            {
                buttonLeft.GetComponent<Image>().sprite = newSprite;
                keyForLeft = key; // Speichern der Taste für Move Left
                playerMovement.SetControls(keyForLeft, keyForRight, playerMovement.jumpKey, playerMovement.dashKey, playerMovement.interactKey);
            }
            else if (buttonRight != null && waitingForInput && keyForRight == KeyCode.None)
            {
                buttonRight.GetComponent<Image>().sprite = newSprite;
                keyForRight = key; // Speichern der Taste für Move Right
                playerMovement.SetControls(keyForLeft, keyForRight, playerMovement.jumpKey, playerMovement.dashKey, playerMovement.interactKey);
            }
        }
        else
        {
            Debug.LogError("Sprite konnte nicht geladen werden! Überprüfe den Pfad.");
        }
    }

    public void SaveChanges()
    {
        PlayerPrefs.SetString("MoveLeftKey", keyForLeft.ToString());
        PlayerPrefs.SetString("MoveRightKey", keyForRight.ToString());
        PlayerPrefs.SetString("JumpKey", playerMovement.jumpKey.ToString());
        PlayerPrefs.SetString("DashKey", playerMovement.dashKey.ToString());
        PlayerPrefs.SetString("InteractKey", playerMovement.interactKey.ToString());

        PlayerPrefs.Save(); // Speichern in die Datei
        Debug.Log("Änderungen gespeichert.");
    }


    public void QuitGame()
    {
        Application.Quit();
    }
}
