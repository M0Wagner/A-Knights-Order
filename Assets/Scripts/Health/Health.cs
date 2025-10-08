using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{
    // creates Header for organizing variables in Unity hub
    [Header ("Health")]
    [SerializeField] private float startingHealth;
    public float StartingHealth => startingHealth;
    
    // current health can be accessed from other script but not SET
    public float currentHealth { get; private set; }
    private Animator animator;
    private bool dead;

    [SerializeField] private float deathAnimationLength = 1.5f;
    [SerializeField] private float delayAfterDeath = 2.0f;

    [Header("iFrames")]
    [SerializeField] private float iFramesDuration;
    [SerializeField] private int numberOfFlashes;
    private SpriteRenderer spriteRenderer;

    [SerializeField] private PlayerBlock playerBlock;

    private void Awake()
    {
        currentHealth = startingHealth;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(float damage)
    {
        if (playerBlock != null && playerBlock.IsBlocking)
        {
            return;
        }
        else
        {
            // reduce health - taken damage, cant get below 0
            currentHealth = Mathf.Clamp(currentHealth - damage, 0, startingHealth);

            if (currentHealth > 0)
            {
                //animator.SetTrigger("hurt");
                StartCoroutine(Invulnerability());
            }
            else
            {
                StartCoroutine(DeathSequence());
            }
        }
    }

    public void HealDamage(float healValue)
    {
        currentHealth = Mathf.Clamp(currentHealth + healValue, 0, startingHealth);
    }

    private IEnumerator DeathSequence()
    {
        // avoid animation being started multiple times
        if (dead) yield break;
        dead = true;

        // 1. Trigger animation and disable controls
        if (animator != null) animator.SetTrigger("die");
        if (GetComponent<PlayerMovement>() != null)
        {
            GetComponent<PlayerMovement>().enabled = false;
        }

        // waiting for the animation to finish + the extra delay, then reload the scene (respawn)
        if (PlayerPrefs.HasKey("SaveRoom"))
        {
            yield return new WaitForSeconds(deathAnimationLength + delayAfterDeath);
            SceneManager.LoadSceneAsync(PlayerPrefs.GetString("SaveRoom"));
            //SceneController.instance.LoadSceneByName(PlayerPrefs.GetString("SaveRoom"));
        }
        else
        {
            yield return new WaitForSeconds(deathAnimationLength + delayAfterDeath);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    private IEnumerator Invulnerability()
    {
        Physics2D.IgnoreLayerCollision(7, 8, true);

        // could flash multiple times if wanted
        for (int i = 0; i < numberOfFlashes; i++)
        {
            // player flashes red when hit
            spriteRenderer.color = new Color(1, 0, 0, 0.5f);
            yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));

            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));
        }
        Physics2D.IgnoreLayerCollision(7, 8, false);
    }
}
