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
        animator.SetTrigger("attack");

        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider2D hit in hitObjects)
        {

            if (hit.TryGetComponent<DestructibleBox>(out DestructibleBox box))
            {
                box.TakeDamage(attackDamage);
                Debug.Log("Hit a box: " + hit.name);
            }
            else
            {
                StartCoroutine(hit.GetComponent<enemyHealth>().takeDamage(attackDamage));
                Debug.Log("Hit an enemy: " + hit.name);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {

        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
