using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurrencyPickup : MonoBehaviour
{
    public enum PickupObject{COIN};
    public int pickupQuantity;
    public PickupObject currentObject;
    PlayerMovement playerMovement;
    [SerializeField] private Text coins;

    private void Start()
    {
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (currentObject == PickupObject.COIN)
            {
                playerMovement.coin += pickupQuantity;
                coins.text = "" + playerMovement.coin;
            }
            Destroy(gameObject);
        } 
    }
}
