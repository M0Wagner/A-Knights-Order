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

    // walljump has a cooldown so you cant spam walljump
    private float wallJumpCooldown;
    private GameObject cam;

    // knockback
    public float KBForce;
    public float KBCounter;
    public float KBTotalTime;

    public bool KnockFromRight;

    // set variables when game is started
    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        cam = GameObject.FindGameObjectWithTag("PlayerCam");
    }

    private void Update()
    {
        // get input from A-D or < - >
        float horizontalInput = Input.GetAxis("Horizontal");

        // if not hit by enemy player is able to move
        if (KBCounter <= 0)
        {
            // might me unneccessary to put in here 
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
                // - is for knockback to left
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
        
        // look (not working yet)
        if (Input.GetKey(KeyCode.W))
            LookUp();

        // dodge / slide, check if button is pressed
        if (Input.GetKeyDown(KeyCode.LeftShift))
            Dodge(horizontalInput);

        // wall jump logic + jump
        if (wallJumpCooldown > 0.2f && KBCounter <= 0)
        {
            // movement (left / right)
            body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);

            // reduce gravity to slowly slide down wall
            if (isOnWall())
            {
                body.gravityScale = 8;
                body.velocity = Vector2.zero;
            }

            // reset gravity to normal if not on wall
            else
                body.gravityScale = 4f;

            // check if space is pressed for jump
            if (Input.GetKey(KeyCode.Space))
                Jump();
        }
        else
            wallJumpCooldown += Time.deltaTime;
    }

    // jump logic
    private void Jump()
    {
        // check if player is on the ground
        if (isGrounded())
        {
            body.velocity = new Vector2(body.velocity.x, jumpPower);
            animator.SetTrigger("jump");
        }

        // walljump logic
        // enable wall jump, can only be done if not on the ground
        else if (isOnWall() && !isGrounded())
        {
            wallJumpCooldown = 0;

            // move player upwards (walljump)
            body.velocity = new Vector2(-Mathf.Sign(transform.localScale.x), 18);
        }
    }

    // dodge / slide player logic
    private void Dodge(float horizontalInput)
    {
        if (isGrounded())
        {
            body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);
            animator.SetTrigger("dodge");
        }  
    }

    // not working yet, reservated to look upwards / downwards
    private void LookUp()
    {
        cam.transform.Translate(0, 100, 0);
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

    public bool canAttack()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        return horizontalInput == 0 && isGrounded() && !isOnWall();
    }
}