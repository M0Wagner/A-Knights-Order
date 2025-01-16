using UnityEngine;
using System.Collections;

public class EnemyAI1 : MonoBehaviour
{
    [Header("Idel")]
    [SerializeField] float idelMovementSpeed;
    [SerializeField] Vector2 idelMovementDirection;

    [Header("AttackUpNDown")]
    [SerializeField] float attackMovementSpeed;
    [SerializeField] Vector2 attackMovementDirection;

    [Header("AttackPlayer")]
    [SerializeField] float attackPlayerSpeed;
    [SerializeField] Transform player;
    [SerializeField] float dashCooldown = 10f; // Cooldown between dashes
    [SerializeField] float chargeTime = 1.5f; // Time to charge before dashing
    private float dashCooldownTimer;

    [Header("Other")]
    [SerializeField] Transform goundCheckUp;
    [SerializeField] Transform goundCheckDown;
    [SerializeField] Transform goundCheckWall;
    [SerializeField] float groundCheckRadius;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float damage;
    // implement for player knockback
    public PlayerMovement playerMovement;
    private bool isTouchingUp;
    private bool isTouchingDown;
    private bool isTouchingWall;
    private bool hasPlayerPositon;

    private Vector2 playerPosition;

    private bool facingLeft = true;
    private bool goingUp = true;
    private bool checkIfHit = false;
    private Rigidbody2D enemyRB;
    private Animator enemyAnim;
    private Vector2 directionToPlayer;

    private bool isCharging = false;

    void Start()
    {
        idelMovementDirection.Normalize();
        attackMovementDirection.Normalize();
        enemyRB = GetComponent<Rigidbody2D>();
        enemyAnim = GetComponent<Animator>();
        dashCooldownTimer = dashCooldown;
    }

    void Update()
    {
        
        isTouchingUp = Physics2D.OverlapCircle(goundCheckUp.position, groundCheckRadius, groundLayer);
        isTouchingDown = Physics2D.OverlapCircle(goundCheckDown.position, groundCheckRadius, groundLayer);
        //Debug.Log(isTouchingDown);
        isTouchingWall = Physics2D.OverlapCircle(goundCheckWall.position, groundCheckRadius, groundLayer);
        

        if (checkIfHit && isTouchingDown) {
            enemyRB.linearVelocity = Vector2.zero; // Stop the enemy's movement
        }

        // Handle dash cooldown
        if (!isCharging && dashCooldownTimer <= 0)
        {
            StartCoroutine(ChargeAndDash());
            dashCooldownTimer = dashCooldown; // Reset cooldown after initiating charge
        }
        else if (!isCharging)
        {
            dashCooldownTimer -= Time.deltaTime;
        }

        // Default movement if not charging or dashing
        if (!isCharging)
        {
            AttackUpNDownState();
        }

        
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(goundCheckUp.position, groundCheckRadius);
        Gizmos.DrawWireSphere(goundCheckDown.position, groundCheckRadius);
        Gizmos.DrawWireSphere(goundCheckWall.position, groundCheckRadius);
    }

    public void IdelState()
    {
        if (isTouchingUp && goingUp)
        {
            ChangeDirection();
        }
        else if (isTouchingDown && !goingUp)
        {
            ChangeDirection();
        }

        if (isTouchingWall)
        {
            if (facingLeft)
            {
                Flip();
            }
            else if (!facingLeft)
            {
                Flip();
            }
        }
        enemyRB.linearVelocity = idelMovementSpeed * idelMovementDirection;
    }

    public void AttackUpNDownState()
    {
        if (isTouchingUp && goingUp)
        {
            ChangeDirection();
        }
        else if (isTouchingDown && !goingUp)
        {
            ChangeDirection();
        }

        if (isTouchingWall)
        {
            if (facingLeft)
            {
                Flip();
            }
            else if (!facingLeft)
            {
                Flip();
            }
        }
        //Debug.Log("aMD"+attackMovementDirection);
        enemyRB.linearVelocity = attackMovementSpeed * attackMovementDirection;
    }

    private IEnumerator ChargeAndDash()
    {
        // Start charging
        isCharging = true;
        enemyRB.linearVelocity = Vector2.zero; // Stop movement
        yield return new WaitForSeconds(chargeTime); // Wait for the charge time

        

        // Dash toward the player
        AttackPlayerState();
    }

    public void AttackPlayerState()
    {
        if (player == null)
        {
            Debug.LogError("Player Transform is not assigned!");
            return;
        }

        directionToPlayer = ((Vector2)player.position - (Vector2)transform.position).normalized;

        Vector2 temp = player.position;
        // Use MovePosition to move the enemy
        enemyRB.linearVelocity = attackMovementSpeed * directionToPlayer;

        //Flip the boss to face the player
        if ((directionToPlayer.x < 0 && !facingLeft) || (directionToPlayer.x > 0 && facingLeft))
        {
            Flip();
        }
        WaitToHit();
        StartCoroutine(StopAfterDash());
    }




    private IEnumerator StopAfterDash()
    {
        
        yield return new WaitForSeconds(1f); // Allow the dash to complete
        enemyRB.linearVelocity = Vector2.zero; // Stop the enemy's movement

        // Reset charging state
        isCharging = false;
        AttackUpNDownState();
    }

    private void WaitToHit()
    {
        checkIfHit = true;
    }

    void Flip()
    {
        facingLeft = !facingLeft;
        idelMovementDirection.x *= -1;
        attackMovementDirection.x *= -1;
        transform.Rotate(0, 180, 0);
    }

    void ChangeDirection()
    {
        goingUp = !goingUp;
        idelMovementDirection.y *= -1;
        attackMovementDirection.y *= -1;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // check if collides with player
        if (collision.CompareTag("Player"))
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
