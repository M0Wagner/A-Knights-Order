using System.Collections;
using UnityEngine;

public class DestructibleBox : MonoBehaviour
{
    [SerializeField] private int maxHealth = 3;
    private int currentHealth;

    [SerializeField] private GameObject itemToDropPrefab;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Break();
        }
        else
        {
            StartCoroutine(FlashRed());
        }
    }

    private void Break()
    {
        if (itemToDropPrefab != null)
        {
            Instantiate(itemToDropPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }

    private IEnumerator FlashRed()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.white;
    }
}