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
    private GameObject currentButton = null;
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
        currentButton = targetImage;
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
        if (key == keyForLeft || key == keyForRight || key == keyForJump || key == keyForDash || key == keyForInteract)
        {
            Debug.LogWarning("Der Key " + key.ToString() + " ist bereits einer Steuerung zugeordnet.");
            return;
        }

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

        if (currentButton != null)
        {
            currentButton.GetComponent<Image>().sprite = newSprite;

            if (currentButton == buttonLeft)
            {
                keyForLeft = key;
                PlayerPrefs.SetString("MoveLeftKey", keyForLeft.ToString());
            }
            else if (currentButton == buttonRight)
            {
                keyForRight = key;
                PlayerPrefs.SetString("MoveRightKey", keyForRight.ToString());
            }
            else if (currentButton == buttonInteract)
            {
                keyForInteract = key;
                PlayerPrefs.SetString("InteractKey", keyForInteract.ToString());
            }
            PlayerPrefs.Save();


            Debug.Log("Taste für " + currentButton.name + " gesetzt: " + key);
            currentButton = null;
            waitingForInput = false;
        }
        else
        {
            Debug.LogError("Sprite konnte nicht geladen werden oder Button ist null.");
        }
    }


    public void SaveChanges()
    {
        PlayerPrefs.SetString("MoveLeftKey", keyForLeft.ToString());
        PlayerPrefs.SetString("MoveRightKey", keyForRight.ToString());
        PlayerPrefs.SetString("JumpKey", keyForJump.ToString());
        PlayerPrefs.SetString("DashKey", keyForDash.ToString());
        PlayerPrefs.SetString("InteractKey", keyForInteract.ToString());

        Debug.Log("Key Left: " + keyForLeft + " Änderungen gespeichert.");
    }

    public void ResetToDefault()
    {
        PlayerPrefs.SetString("MoveLeftKey", KeyCode.A.ToString());
        PlayerPrefs.SetString("MoveRightKey", KeyCode.D.ToString());
        PlayerPrefs.SetString("JumpKey", KeyCode.Space.ToString());
        PlayerPrefs.SetString("DashKey", KeyCode.LeftShift.ToString());
        PlayerPrefs.SetString("InteractKey", KeyCode.E.ToString());

        buttonLeft.GetComponent<Image>().sprite = Resources.Load<Sprite>("A");
        buttonRight.GetComponent<Image>().sprite = Resources.Load<Sprite>("D");
        buttonInteract.GetComponent<Image>().sprite = Resources.Load<Sprite>("E");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
