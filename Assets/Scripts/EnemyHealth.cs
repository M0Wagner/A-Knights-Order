using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyHealth : MonoBehaviour
{
    public int maxHealth = 1;
    int currentHealth;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public IEnumerator takeDamage(int damage)
    {
        currentHealth -= damage;

        // Play hurt animation
        yield return new WaitForSeconds(0.25f);
        if (currentHealth > 0) 
        {
            Physics2D.IgnoreLayerCollision(7, 8, true);
            

            spriteRenderer.color = new Color(1, 0, 0, 0.5f);
            yield return new WaitForSeconds(0.5f);

            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(0.5f);
            Physics2D.IgnoreLayerCollision(7, 8, false);

        } else
        {
            Die();
        } 
    }

    void Die() 
    {
        Debug.Log("Enemy died");
        // Die animation
        Destroy(gameObject);
    }

}