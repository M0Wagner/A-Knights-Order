using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BeginPoint : MonoBehaviour
{
    [SerializeField] bool goNextLevel;
    [SerializeField] string levelName;
    public Vector2 exitPostion;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (goNextLevel)
            {
                // go to next level
                //SceneController.instance.previousLevel();
            } else 
            {
                Debug.Log(SceneManager.GetActiveScene().name);
                Debug.Log(levelName);
                Debug.Log(exitPostion);



                SceneController.instance.SetEntryPoint(SceneManager.GetActiveScene().name, exitPostion);
                SceneController.instance.LoadSceneByName(levelName);
            }
        }
    }
}
