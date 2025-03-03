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
    private string buttonLeftImage = "";
    private string buttonRightImage = "";
    private string buttonInteractImage = "";

    private GameObject currentButton = null;
    private bool waitingForInput = false;
    PlayerMovement playerMovement;

    private KeyCode keyForLeft = KeyCode.None;
    private KeyCode keyForRight = KeyCode.None;
    private KeyCode keyForJump = KeyCode.Space;
    private KeyCode keyForDash = KeyCode.LeftShift;
    private KeyCode keyForInteract = KeyCode.None;

    void Start()
    {
        //PlayerPrefs.DeleteAll();
        LoadControlsImages();
    }

    public void PlayGame()
    {
        // right now its the index File->Build Settings
        // name works also
        // sets the next level "SceneManager.GetActiveScene().buildIndex + 1"
        if (PlayerPrefs.HasKey("SaveRoom")) {
            SceneManager.LoadSceneAsync(PlayerPrefs.GetString("SaveRoom"));
            //SceneController.instance.LoadSceneByName(PlayerPrefs.GetString("SaveRoom"));
        } else {
            SceneManager.LoadSceneAsync("Throne Room");
            //SceneController.instance.LoadSceneByName("Level1");
        }
    }

    public void Controls(string sceneName)
    {
        SceneManager.LoadSceneAsync(sceneName);
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
        // Überprüfen, ob der Key bereits einer Steuerung zugeordnet ist
        if (key == keyForLeft || key == keyForRight || key == keyForJump || key == keyForDash || key == keyForInteract)
        {
            Debug.LogWarning("Der Key " + key.ToString() + " ist bereits einer Steuerung zugeordnet.");
            return;
        }

        // Dictionary für KeyCode-Sprite-Zuordnungen
        Dictionary<KeyCode, string> keySpriteMap = new Dictionary<KeyCode, string>
        {
            { KeyCode.Alpha1, "1" },
            { KeyCode.Alpha2, "2" },
            { KeyCode.Alpha3, "3" },
            { KeyCode.Alpha4, "4" },
            { KeyCode.Alpha5, "5" },
            { KeyCode.Alpha6, "6" },
            { KeyCode.Alpha7, "7" },
            { KeyCode.Alpha8, "8" },
            { KeyCode.Alpha9, "9" },
            { KeyCode.Alpha0, "0" },
            { KeyCode.Q, "Q" },
            { KeyCode.W, "W" },
            { KeyCode.E, "E" },
            { KeyCode.R, "R" },
            { KeyCode.T, "T" },
            { KeyCode.Z, "Z" },
            { KeyCode.U, "U" },
            { KeyCode.I, "I" },
            { KeyCode.O, "O" },
            { KeyCode.P, "P" },
            { KeyCode.A, "A" },
            { KeyCode.S, "S" },
            { KeyCode.D, "D" },
            { KeyCode.F, "F" },
            { KeyCode.G, "G" },
            { KeyCode.H, "H" },
            { KeyCode.J, "J" },
            { KeyCode.K, "K" },
            { KeyCode.L, "L" },
            { KeyCode.Y, "Y" },
            { KeyCode.X, "X" },
            { KeyCode.C, "C" },
            { KeyCode.V, "V" },
            { KeyCode.B, "B" },
            { KeyCode.N, "N" },
            { KeyCode.M, "M" },
            { KeyCode.LeftArrow, "LeftArrow" },
            { KeyCode.RightArrow, "RightArrow" }
        };

        // Sprite laden
        if (!keySpriteMap.TryGetValue(key, out string spritePath))
        {
            Debug.LogWarning("Kein Sprite für die Taste " + key.ToString() + " gefunden.");
            return;
        }

        Sprite newSprite = Resources.Load<Sprite>(spritePath);

        if (newSprite == null)
        {
            Debug.LogError("Sprite konnte nicht geladen werden: " + spritePath);
            return;
        }

        // Aktuellen Button aktualisieren
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

            // Änderungen speichern
            PlayerPrefs.Save();

            Debug.Log("Taste für " + currentButton.name + " gesetzt: " + key);
            currentButton = null;
            waitingForInput = false;
        }
        else
        {
            Debug.LogError("Aktueller Button ist null. Aktion nicht möglich.");
        }
    }

    public void SaveChanges()
    {
        PlayerPrefs.SetString("MoveLeftKey", keyForLeft.ToString());
        PlayerPrefs.SetString("MoveRightKey", keyForRight.ToString());
        PlayerPrefs.SetString("JumpKey", keyForJump.ToString());
        PlayerPrefs.SetString("DashKey", keyForDash.ToString());
        PlayerPrefs.SetString("InteractKey", keyForInteract.ToString());
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

    public void LoadControlsImages() {
        if (PlayerPrefs.HasKey("MoveLeftKey"))
            buttonLeftImage = PlayerPrefs.GetString("MoveLeftKey");
            buttonLeft.GetComponent<Image>().sprite = Resources.Load<Sprite>(buttonLeftImage);
        
        if (PlayerPrefs.HasKey("MoveRightKey"))
            buttonRightImage = PlayerPrefs.GetString("MoveRightKey");
            buttonRight.GetComponent<Image>().sprite = Resources.Load<Sprite>(buttonRightImage);

        if (PlayerPrefs.HasKey("InteractKey"))
            buttonInteractImage = PlayerPrefs.GetString("InteractKey");
            buttonInteract.GetComponent<Image>().sprite = Resources.Load<Sprite>(buttonInteractImage);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
