using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController instance;
    [SerializeField] Animator animator;
    PlayerMovement playerMovement;

    private void Awake()
    {
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();

        // dont destroy while loading
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        // if new scene has this object aswell => destroy
        else
        {
            Destroy(gameObject);
        }
    }

    // for jumping to next level
    public void NextLevel()
    {
        StartCoroutine(LoadLevel());
    }

    // for jumping to chosen level
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadSceneAsync(sceneName);
    }

    // set animation for level change
    IEnumerator LoadLevel()
    {
        animator.SetTrigger("End");
        playerMovement.disableMovement();
        yield return new WaitForSeconds(1);
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
        animator.SetTrigger("Start");
        playerMovement.enableMovement();
    }
}
