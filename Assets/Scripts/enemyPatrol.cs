using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyPatrol : MonoBehaviour
{

    public float speed;

    // patrol between points
    [SerializeField] private GameObject pointA;
    [SerializeField] private GameObject pointB;

    private Rigidbody2D rb;
    private Animator animator;  
    private Transform currentPoint;

    [SerializeField] public float damage;

    // implement for player knockback
    public PlayerMovement playerMovement;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();   

        // enemy allways starts to fly to point B
        currentPoint = pointB.transform;
    }

    // Update is called once per frame
    void Update()
    {
        // isnt used once in script (Moritz??????????????????????)
        Vector2 point = currentPoint.position - transform.position;

        // change directions if enemy is at the point
        if (currentPoint == pointB.transform)
        {
            rb.velocity = new Vector2(speed, 0);
        }
        else
        {
            rb.velocity = new Vector2 (-speed, 0);
        }

        // rotate enemy to left
        if (Vector2.Distance(transform.position, currentPoint.position) < 0.5f && currentPoint == pointB.transform)
        {
            currentPoint = pointA.transform;
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }

        // rotate enemy to right
        if (Vector2.Distance(transform.position, currentPoint.position) < 0.5f && currentPoint == pointA.transform)
        {
            currentPoint = pointB.transform;
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(pointA.transform.position, 0.5f);
        Gizmos.DrawWireSphere(pointB.transform.position, 0.5f);
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
