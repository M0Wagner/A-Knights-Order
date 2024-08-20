using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float speed;
    [SerializeField] private float jumpPower;
    
    private Rigidbody2D body;
    private Animator animator;
    private BoxCollider2D boxCollider;
    private float wallJumpCooldown;
    private GameObject cam;

    // knockback
    public float KBForce;
    public float KBCounter;
    public float KBTotalTime;

    public bool KnockFromRight;


    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        cam = GameObject.FindGameObjectWithTag("PlayerCam");
    }

    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");

        // if not hit by enemy player is able to move
        if (KBCounter <= 0)
        {
            // flip character when moving
            if (horizontalInput > 0.01f)
                transform.localScale = Vector3.one;
            else if (horizontalInput < -0.01f)
                transform.localScale = new Vector3(-1, 1, 1);
        }
        // player gets knockback when getting hit
        else
        {
            if (KnockFromRight)
            {
                body.velocity = new Vector2(-KBForce, KBForce / 2);
            }
            if (!KnockFromRight)
            {
                body.velocity = new Vector2(KBForce, KBForce / 2);
            }

            KBCounter -= Time.deltaTime;
        }

        // set animator input
        animator.SetBool("isRunning", horizontalInput != 0);
        animator.SetBool("isGrounded", isGrounded());

        // look 
        if (Input.GetKey(KeyCode.W))
            LookUp();

        // dodge
        if (Input.GetKey(KeyCode.LeftShift))
            Dodge(horizontalInput);

        // wall jump logic + jump
        if (wallJumpCooldown > 0.2f && KBCounter <= 0)
        {
            // movement 
            body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);

            // reduce gravity to slowly slide down wall
            if (isOnWall())
            {
                body.gravityScale = 8;
                body.velocity = Vector2.zero;
            }

            // reset gravity to normal
            else
                body.gravityScale = 4f;

            // check if space is pressed for jump
            if (Input.GetKey(KeyCode.Space))
                Jump();
        }
        else
            wallJumpCooldown += Time.deltaTime;
    }

    // function to enable jumping
    private void Jump()
    {
        if (isGrounded())
        {
            body.velocity = new Vector2(body.velocity.x, jumpPower);
            animator.SetTrigger("jump");
        }

        // enable wall jump
        else if (isOnWall() && !isGrounded())
        {
            wallJumpCooldown = 0;
            body.velocity = new Vector2(-Mathf.Sign(transform.localScale.x), 18);
        }
    }

    // dodge / slide player
    private void Dodge(float horizontalInput)
    {
        if (isGrounded())
        {
            body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);
            animator.SetTrigger("dodge");
        }  
    }

    // not working
    private void LookUp()
    {
        cam.transform.Translate(0, 100, 0);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
    }

    // check if player is on the ground to enable jump
    private bool isGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        return raycastHit.collider != null;
    }

    // check if player is on wall to enable wallljump
    private bool isOnWall()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, new Vector2(transform.localScale.x, 0), 0.1f, wallLayer);
        return raycastHit.collider != null;
    }
}