using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeginPoint : MonoBehaviour
{
    [SerializeField] bool goNextLevel;
    [SerializeField] string levelName;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (goNextLevel)
            {
                // go to next level
                SceneController.instance.previousLevel();
            } else 
            {
                SceneController.instance.LoadSceneByName(levelName);
            }
        }
    }
}
