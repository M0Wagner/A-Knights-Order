using UnityEngine;
using UnityEngine.SceneManagement;

public class UIPausePage : MonoBehaviour
{

    private void Awake()
    {
        Hide();
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Resume()
    {
        Debug.Log("Resume game");
        Time.timeScale = 1;
        gameObject.SetActive(false);
    }

    public void QuitToMenu()
    {
        Debug.Log("bye bye");
        SceneManager.LoadScene(0);
    }
}
