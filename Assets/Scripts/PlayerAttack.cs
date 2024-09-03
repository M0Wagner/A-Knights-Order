using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public Animator animator;
    public LayerMask enemyLayers;

    [Header ("Individual Stats")]
    public Transform attackPoint;
    public float attackRange = 0.5f;

    public int attackDamage = 1;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            Attack();
    }
        

    private void Attack()
    {
        // Play an attack animation
        animator.SetTrigger("attack");

        // Detect enemies in range of attack
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        // Damage them
        foreach (Collider2D enemy in hitEnemies)
        {
            StartCoroutine(enemy.GetComponent<enemyHealth>().takeDamage(attackDamage));
        }

    }

    private void OnDrawGizmosSelected()
    {

        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }


}
