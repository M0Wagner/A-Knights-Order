using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeginPoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            // go to next level
            SceneController.instance.previousLevel();
        }
    }
}
