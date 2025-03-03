using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestController : MonoBehaviour
{
    PlayerMovement playerMovement;

    private void Start()
    {
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
    }
    public void openChest()
    {
        Debug.Log("Opened chest");
    }
}
