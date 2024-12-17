using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;

    [Header("Movement")]
    [SerializeField] private float speed;
    [SerializeField] private float jumpPower;
    private float dashSpeed = 15;

    private Rigidbody2D body;
    private Animator animator;
    private BoxCollider2D boxCollider;

    // walljump has a cooldown so you cant spam walljump
    private float wallJumpCooldown;
    private GameObject cam;

    private bool isMovementEnabled = true;

    [Header("Knockback")]
    public float KBForce;
    public float KBCounter;
    public float KBTotalTime;

    public bool KnockFromRight;
    public int coin;

    private bool isDashing;

    [SerializeField] private DialogueUI dialogueUI;

    public DialogueUI DialogueUI => dialogueUI;

    public IInteractable Interactable { get; set; }

    public KeyCode moveLeftKey = KeyCode.A;
    public KeyCode moveRightKey = KeyCode.D;
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode dashKey = KeyCode.LeftShift;
    public KeyCode interactKey = KeyCode.E;

    /*void Start()
    {
        LoadControls();
    }*/

    private void Awake()
    {
        string currentLevel = SceneManager.GetActiveScene().name;
        if (SceneController.instance != null)
        {
            Vector2 entryPoint = SceneController.instance.GetEntryPoint(currentLevel);
            
            if (entryPoint != Vector2.zero)
                transform.position = entryPoint;
        }
    

        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        cam = GameObject.FindGameObjectWithTag("PlayerCam");
    }

    private void Update()
    {
        if (dialogueUI.IsOpen)
            return;

        float horizontalInput = 0f;
        
        if (isMovementEnabled)
        {
            if (Input.GetKey(moveLeftKey))
                horizontalInput = -1f;
            else if (Input.GetKey(moveRightKey))
                horizontalInput = 1f;
        }

        if (KBCounter <= 0)
        {
            flipCharacter(horizontalInput);

            // Animator-Einstellungen
            animator.SetBool("isRunning", horizontalInput != 0);
            animator.SetBool("isGrounded", isGrounded());

            // Dash-Logik
            if (Input.GetKeyDown(dashKey) && isGrounded())
                dash(horizontalInput);
            else if (!isDashing)
                body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);

            // Wall-Jump-Logik
            if (wallJumpCooldown > 0.2f && KBCounter <= 0)
            {
                handleWallJump(horizontalInput);
            }
            else
                wallJumpCooldown += Time.deltaTime;

            // Sprung-Logik
            if (Input.GetKey(jumpKey))
                Jump();

            // Interaktions-Logik
            if (Input.GetKeyDown(interactKey))
                Interactable?.Interact(this);
        }
        else
        {
            handleKnockback(horizontalInput);
        }
    }

    private void Jump()
    {
        if (isGrounded())
        {
            body.velocity = new Vector2(body.velocity.x, jumpPower);
            animator.SetTrigger("jump");
        }
        else if (isOnWall() && !isGrounded())
        {
            wallJumpCooldown = 0;
            body.velocity = new Vector2(-Mathf.Sign(transform.localScale.x), 18);
        }
    }

    private void handleKnockback(float horizontalInput)
    {
        if (KBCounter > 0)
        {
            if (KnockFromRight)
                body.velocity = new Vector2(-KBForce, KBForce / 2);
            else
                body.velocity = new Vector2(KBForce, KBForce / 2);

            KBCounter -= Time.deltaTime;
        }
    }

    private void handleWallJump(float horizontalInput)
    {
        if (isOnWall())
        {
            body.gravityScale = 8;
            body.velocity = Vector2.zero;
        }
        else
            body.gravityScale = 4f;
    }

    private void flipCharacter(float horizontalInput)
    {
        if (horizontalInput > 0.01f)
            transform.localScale = new Vector3(0.135f, 0.135f, 1);
        else if (horizontalInput < -0.01f)
            transform.localScale = new Vector3(-0.135f, 0.135f, 1);
    }

    private void dash(float horizontalInput)
    {
        if (!isDashing && isGrounded())
        {
            isDashing = true;
            body.velocity = new Vector2(horizontalInput * dashSpeed, body.velocity.y);
            animator.SetTrigger("dodge");
        }
        StartCoroutine(endDash());
    }

    private IEnumerator endDash()
    {
        yield return new WaitForSeconds(0.5f);
        isDashing = false;
    }

    private bool isGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        return raycastHit.collider != null;
    }

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

    public void disableMovement()
    {
        isMovementEnabled = false;
    }

    public void enableMovement()
    {
        isMovementEnabled = true;
    }

    public void SetControls(KeyCode newMoveLeft, KeyCode newMoveRight, KeyCode newJump, KeyCode newDash, KeyCode newInteract)
    {
        moveLeftKey = newMoveLeft;
        moveRightKey = newMoveRight;
        jumpKey = newJump;
        dashKey = newDash;
        interactKey = newInteract;
    }

    public void LoadControls()
    {
        // Laden der Tastenbelegungen aus PlayerPrefs
        if (PlayerPrefs.HasKey("MoveLeftKey"))
            moveLeftKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("MoveLeftKey"));

        if (PlayerPrefs.HasKey("MoveRightKey"))
            moveRightKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("MoveRightKey"));

        if (PlayerPrefs.HasKey("JumpKey"))
            jumpKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("JumpKey"));

        if (PlayerPrefs.HasKey("DashKey"))
            dashKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("DashKey"));

        if (PlayerPrefs.HasKey("InteractKey"))
            interactKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("InteractKey"));

        Debug.Log("Kontrollen geladen.");
    }
}

/*
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;

    [Header ("Movement")]
    [SerializeField] private float speed;
    [SerializeField] private float jumpPower;
    private float dashSpeed = 15;
    
    private Rigidbody2D body;
    private Animator animator;
    private BoxCollider2D boxCollider;

    // walljump has a cooldown so you cant spam walljump
    private float wallJumpCooldown;
    private GameObject cam;

    private bool isMovementEnabled = true;

    [Header ("Knockback")]
    // knockback
    public float KBForce;
    public float KBCounter;
    public float KBTotalTime;

    public bool KnockFromRight;
    public int coin;

    private bool isDashing;

    [SerializeField] private DialogueUI dialogueUI;

    public DialogueUI DialogueUI => dialogueUI;

    public IInteractable Interactable {  get; set; }

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
        if (dialogueUI.IsOpen)
            return;

        float horizontalInput;
        // get input from A-D or < - >
        if (isMovementEnabled)
            horizontalInput = Input.GetAxis("Horizontal");
        else 
            horizontalInput = 0;



        // if not hit by enemy player is able to move
        if (KBCounter <= 0)
        {
            flipCharacter(horizontalInput);


            // set animator input
            animator.SetBool("isRunning", horizontalInput != 0);
            animator.SetBool("isGrounded", isGrounded());


            // dodge / slide, check if button is pressed
            if (Input.GetKeyDown(KeyCode.LeftShift) && isGrounded())
                dash(horizontalInput);
            else if (!isDashing)
                // movement (left / right)
                body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);



            // wall jump logic
            if (wallJumpCooldown > 0.2f && KBCounter <= 0)
            {
                handleWallJump(horizontalInput);
            }
            else
                wallJumpCooldown += Time.deltaTime;


            // check if space is pressed for jump
            if (Input.GetKey(KeyCode.Space))
                Jump();

            if (Input.GetKey(KeyCode.E))
                    Interactable?.Interact(this);
        }


        // player gets knockback when getting hit
        else
        {
            handleKnockback(horizontalInput);
        }
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


    private void handleKnockback(float horizontalInput)
    {
        if (KBCounter > 0)
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
    }


    private void handleWallJump(float horizontalInput)
    {
        // reduce gravity to slowly slide down wall
        if (isOnWall())
        {
            body.gravityScale = 8;
            body.velocity = Vector2.zero;
        }

        // reset gravity to normal if not on wall
        else
            body.gravityScale = 4f;
    }


    private void flipCharacter(float horizontalInput)
    {
        // flip character when moving
        if (horizontalInput > 0.01f)
            transform.localScale = new Vector3(0.135f, 0.135f, 1);
        else if (horizontalInput < -0.01f)
            transform.localScale = new Vector3(-0.135f, 0.135f, 1);
    }


    // dodge / slide player logic
    private void dash(float horizontalInput)
    {
        if (!isDashing && isGrounded())
        {
            isDashing = true;

            body.velocity = new Vector2(horizontalInput * dashSpeed, body.velocity.y);
            animator.SetTrigger("dodge");
        }

        StartCoroutine(endDash());
    }


    private IEnumerator endDash()
    {
        // Dash duration
        yield return new WaitForSeconds(0.5f);  
        isDashing = false;
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

    public void disableMovement()
    {
        isMovementEnabled = false;
    }


    public void enableMovement()
    {
        isMovementEnabled = true;
    }
}
*/