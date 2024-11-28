using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FinishPoint : MonoBehaviour
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
                SceneController.instance.NextLevel();
            } else
            {
                SceneController.instance.LoadSceneByName(levelName);
            }
        }
    }
}
