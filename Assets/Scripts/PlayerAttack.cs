using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public Animator animator;
    public LayerMask enemyLayers;

    [Header ("Individual Stats")]
    public Transform attackPoint;
    public float attackRange = 0.5f;

    public int attackDamage = 1;
    private bool isAttacking = false;
    [SerializeField] private float cooldown;

    private void Update()
    {
        if (cooldown <= 0)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Attack();
                cooldown = 0.5f;
            }
        }
        else
        {
            cooldown -= Time.deltaTime;
        }
    }
        

    private void Attack()
    {
        isAttacking = true;

        // Play an attack animation
        animator.SetTrigger("attack");

        // Detect enemies in range of attack
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        // Damage them
        foreach (Collider2D enemy in hitEnemies)
        {
            StartCoroutine(enemy.GetComponent<enemyHealth>().takeDamage(attackDamage));
            Debug.Log("HIt");
        }
    }

    private void OnDrawGizmosSelected()
    {

        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
