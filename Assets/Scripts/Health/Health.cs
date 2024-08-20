using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float startingHealth;
    
    // current health can be accessed from other script but not SET
    public float currentHealth { get; private set; }
    private Animator animator;
    private bool dead;

    private void Awake()
    {
        currentHealth = startingHealth;
        animator = GetComponent<Animator>();
    }

    public void TakeDamage(float damage)
    {
        // reduce health - taken damage, cant get below 0
        currentHealth = Mathf.Clamp(currentHealth - damage, 0, startingHealth);

        if (currentHealth > 0)
        {
            animator.SetTrigger("hurt");
        }
        else
        {
            // avoid animation being started multiple times
            if (!dead)
            {
                animator.SetTrigger("die");

                // disable player movement
                GetComponent<PlayerMovement>().enabled = false;

                dead = true;
            }
        }
    }
}
