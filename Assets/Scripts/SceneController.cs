using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController instance;
    [SerializeField] Animator animator;
    PlayerMovement playerMovement;

    private Dictionary<string, Vector2> entryPoints = new Dictionary<string, Vector2>();

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

    // for jumping to chosen level
    public void LoadSceneByName(string sceneName)
    {
        StartCoroutine(LoadLevel(sceneName));
    }

    // set animation for level change
    IEnumerator LoadLevel(string sceneName)
    {
        animator.SetTrigger("End");
        playerMovement.disableMovement();
        yield return new WaitForSeconds(1);
        SceneManager.LoadSceneAsync(sceneName);
        animator.SetTrigger("Start");
        playerMovement.enableMovement();
    }

    public void SetEntryPoint(string levelName, Vector2 position)
    {
        entryPoints[levelName] = position;
    }

    public Vector2 GetEntryPoint(string levelName)
    {
        if (entryPoints.ContainsKey(levelName))
            return entryPoints[levelName];
        
        return Vector2.zero;
    }
}
