using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aiChase : MonoBehaviour
{
    public GameObject player;
    public float speed;
    [SerializeField] public float damage;

    private float distance;

    public PlayerMovement playerMovement;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        distance = Vector2.Distance(transform.position, player.transform.position);
        Vector2 direction = player.transform.position - transform.position;
        direction.Normalize(); 
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        if (angle > 90 || angle < -90)
        {
            transform.localScale = new Vector3(6, -6, 6);
        } else
        {
            transform.localScale = new Vector3(6, 6, 6);
        }

        if (distance < 10)
        {
            transform.position = Vector2.MoveTowards(this.transform.position, player.transform.position, speed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(Vector3.forward * angle);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        // check if collides with player
        if (collision.tag == "Player")
        {
            // start counter
            playerMovement.KBCounter = playerMovement.KBTotalTime;

            if (collision.transform.position.x <= transform.position.x)
            {
                playerMovement.KnockFromRight = true;
            }
            if (collision.transform.position.x > transform.position.x)
            {
                playerMovement.KnockFromRight = false;
            }

            // player takes damage
            collision.GetComponent<Health>().TakeDamage(damage);
        }
    }
}
