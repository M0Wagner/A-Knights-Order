using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    public Transform player; // Reference to the player
    public Animator animator; // Reference to the Animator component
    public float phaseSwitchHealthThreshold = 50f; // Health threshold to switch to phase 2
    public float health = 1f; // Boss health
    public float speed = 2f; // Movement speed for phase one
    public float phaseTwoSpeedMultiplier = 1.5f; // Speed multiplier for phase two
    public float attackCooldown = 10f; // Time between attacks
    public float stopDistance = 3f; // Distance at which the boss stops moving towards the player
    public float maxDistance = 15f;
    public float attackRange = 1.5f; // Distance at which the boss can attack the player
    public float damageAmount = 10f; // Amount of damage the boss deals to the player

    private bool isPhaseTwo = false;
    private bool isAttacking = false;
    private float attackCooldownTimer = 0f;
    private bool isInAttackCooldown = false; // Track if the boss is in cooldown after an attack
    private bool hasAttackedThisCycle = false; // Track if the boss has already attacked in this cycle
    private bool isFacingPlayer = true; // Track if the boss is currently facing the player
    private Coroutine facePlayerCoroutine; // Reference to the coroutine

    public enemyHealth enemyHealth; // Reference to the enemyHealth script

    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject[] fireBalls;


    void Start()
    {
        if (!animator) animator = GetComponent<Animator>();
        if (!enemyHealth) enemyHealth = GetComponent<enemyHealth>();
    }

    void Update()
    {
        attackCooldownTimer += Time.deltaTime;

        // Only call FacePlayer if the coroutine is not running
        if (isFacingPlayer)
        {
            FacePlayer();
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        //Debug.Log(distanceToPlayer);

        if (!isAttacking && distanceToPlayer > stopDistance)    //&& distanceToPlayer < maxDistance
        {
            MoveTowardsPlayer();
        }
        else if (distanceToPlayer <= stopDistance)
        {
            animator.SetBool("IsMoving", false);
            if (distanceToPlayer <= attackRange && !isInAttackCooldown && !hasAttackedThisCycle)
            {
                AttackPlayer();
            }
        }
        /*else if (distanceToPlayer >= maxDistance)
        {
            animator.SetBool("IsMoving", false);
            rangedAttack();
        }*/

        // Handle cooldown timer
        if (isInAttackCooldown)
        {
            // Check if cooldown period has elapsed
            if (attackCooldownTimer >= attackCooldown)
            {
                isInAttackCooldown = false; // Reset cooldown flag
                attackCooldownTimer = 0f; // Reset timer
                hasAttackedThisCycle = false; // Reset attack flag for the next cycle
            }
        }
    }

    private void FacePlayer()
    {
        // Check if the boss is already facing the correct direction
        Vector3 scale = transform.localScale;
        if (player.position.x < transform.position.x && scale.x > 0)
        {
            scale.x *= -1; // Flip left
            transform.localScale = scale;
        }
        else if (player.position.x > transform.position.x && scale.x < 0)
        {
            scale.x *= -1; // Flip right
            transform.localScale = scale;
        }
    }

    private void MoveTowardsPlayer()
    {
        // Move towards the player's position
        float step = speed * Time.deltaTime;
        Vector3 targetPosition = new Vector3(player.position.x, transform.position.y, transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);

        // Trigger movement animation
        animator.SetBool("IsMoving", true);
    }

    private void AttackPlayer()
    {
        int randomNumber = Random.Range(1, 5); // Updated to include the fourth attack

        // Trigger attack animation
        if (randomNumber == 1)
        {
            animator.SetTrigger("Attack_basic");
        }
        else if (randomNumber == 2)
        {
            StartCoroutine(PerformStrikeAttack());
        }
        else if (randomNumber == 3)
        {
            animator.SetTrigger("Attack_basic_2");
            damageAmount = 20f;
        }
        else if (randomNumber == 4) // New jump attack logic
        {
            StartCoroutine(PerformJumpAttack());
        }

        // Set the cooldown flag and attack flag
        isInAttackCooldown = true;
        hasAttackedThisCycle = true; // Mark that an attack has occurred
        attackCooldownTimer = 0f; // Reset cooldown timer
    }

    // Coroutine for the jump attack
    private IEnumerator PerformJumpAttack()
    {
        // Trigger jump attack animation
        animator.SetTrigger("Attack_jump_sword");

        // Calculate the jump arc
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = player.position;

        float jumpHeight = 3f; // Height of the jump
        float jumpDuration = 1f; // Time for the jump to complete
        float elapsedTime = 0f;

        while (elapsedTime < jumpDuration)
        {
            // Parabolic jump formula
            float t = elapsedTime / jumpDuration;
            float height = 4 * jumpHeight * t * (1 - t); // Simple parabola equation

            Vector3 currentPosition = Vector3.Lerp(startPosition, targetPosition, t);
            currentPosition.y += height;

            transform.position = currentPosition;
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // Snap position to the target at the end
        transform.position = targetPosition;

        // Apply damage to the player (if in range)
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= attackRange)
        {
            damagePlayer();
        }

        // Wait for a short moment before resuming movement
        yield return new WaitForSeconds(0.5f);
    }

    /*private void rangedAttack()
    {

        Debug.Log("hier");
        animator.SetTrigger("Attack_skill");

        fireBalls[FindFireball()].transform.position = firePoint.position;
        fireBalls[FindFireball()].GetComponent<Projectile>().SetDirection(Mathf.Sign(transform.localScale.x));
        
    }*/

    private int FindFireball()
    {
        for (int i = 0; i < fireBalls.Length; i++)
        {
            if (!fireBalls[i].activeInHierarchy)
                return i;
        }
        return 0;
    }

    private void damagePlayer()
    {
        // Call the player's damage method
        Health playerHealth = player.GetComponent<Health>(); // Assuming the player has a Health script
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damageAmount);
        }

        damageAmount = 10f;
    }

    private IEnumerator Sleep(float sleepDuration)
    {
        yield return new WaitForSeconds(sleepDuration); // Wait for the specified duration
    }

    private IEnumerator PerformStrikeAttack()
    {
        // Move backward
        Vector3 moveBackPosition = transform.position + (transform.right * -2f); // Move 2 units back
        float moveBackTime = 0.5f; // Time to move back
        float elapsedTime = 0f;

        while (elapsedTime < moveBackTime)
        {
            transform.position = Vector3.Lerp(transform.position, moveBackPosition, elapsedTime / moveBackTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Trigger attack animation
        animator.SetTrigger("Attack_strike");

        // Wait for animation duration (adjust based on your animation length)
        //yield return new WaitForSeconds(0.05f); // Assume animation takes 0.5 seconds

        // Move forward to the player's position
        Vector3 moveToPlayerPosition = player.position;
        elapsedTime = 0f;
        float moveToPlayerTime = 0.2f; // Time to move back to the player

        while (elapsedTime < moveToPlayerTime)
        {
            transform.position = Vector3.Lerp(transform.position, moveToPlayerPosition, elapsedTime / moveToPlayerTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }


    public void StopMovement()
        {
            // Call this when attacking to stop movement temporarily
            animator.SetBool("IsMoving", false);
            isAttacking = true;
        }

        public void ResumeMovement()
        {
            // Resume movement after attack
            isAttacking = false;
        }


    private void Die()
        {
            Debug.Log("Boss defeated!");
            animator.SetTrigger("Die"); // Play death animation
                                        // Optionally, instantiate loot or perform other actions here
            Destroy(gameObject, 2f); // Destroy the boss after a delay
        }
    }