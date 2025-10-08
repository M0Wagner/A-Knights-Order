using UnityEngine;

public class PlayerBlock : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Animator animator;

    [Header("Input")]
    [SerializeField] private KeyCode blockKey = KeyCode.Mouse1; // Right click

    [Header("Block area (like attack)")]
    public Transform blockPoint;
    public float blockRange = 1f;
    public LayerMask enemyLayers;

    [Header("Effects on enemies while blocking")]
    public int reflectDamage = 0;
    public float knockbackForce = 1.5f;
    public float checkInterval = 0.05f;

    [Header("Gameplay")]
    [Range(0f, 1f)] public float damageMultiplierWhileBlocking = 0f;
    [SerializeField] private float blockCooldown = 0.25f;

    public bool IsBlocking { get; private set; }

    private static readonly int BlockBool    = Animator.StringToHash("isBlocking");
    private static readonly int BlockTrigger = Animator.StringToHash("block");

    private float cooldownTimer;
    private float checkTimer;

    private void Reset()
    {
        if (!animator) animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;

        // Start block on press
        if (Input.GetKeyDown(blockKey) && cooldownTimer <= 0f && !IsBlocking)
            StartBlock();

        if (IsBlocking)
        {
            checkTimer -= Time.deltaTime;
            if (checkTimer <= 0f)
            {
                CheckBlockContacts();
                checkTimer = checkInterval;
            }
        }

        // End block on release
        if (Input.GetKeyUp(blockKey) && IsBlocking)
            EndBlock();
    }

    private void StartBlock()
    {
        IsBlocking = true;
        if (animator)
        {
            animator.SetBool(BlockBool, true);
            animator.SetTrigger(BlockTrigger);
        }
        checkTimer = 0f; // do an immediate contact check
    }

    private void EndBlock()
    {
        IsBlocking = false;
        if (animator) animator.SetBool(BlockBool, false);
        cooldownTimer = blockCooldown;
    }

    private void CheckBlockContacts()
    {
        if (blockPoint == null) return;

        var hits = Physics2D.OverlapCircleAll(blockPoint.position, blockRange, enemyLayers);
        foreach (var h in hits)
        {
            if (reflectDamage > 0 && h.TryGetComponent(out enemyHealth eh))
            {
                StartCoroutine(eh.takeDamage(reflectDamage));
            }

            var rb = h.attachedRigidbody;
            if (rb != null)
            {
                float dir = Mathf.Sign(h.transform.position.x - transform.position.x);
                rb.linearVelocity = new Vector2(dir * knockbackForce, rb.linearVelocity.y);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!blockPoint) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(blockPoint.position, blockRange);
    }
}