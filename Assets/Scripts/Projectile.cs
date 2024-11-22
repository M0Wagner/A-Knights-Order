using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed;
    private float direction;
    private bool hit;

    private Animator animator;
    private BoxCollider2D boxCollider;

    private void Awake() // Corrected method name
    {
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        if (hit) return;
        float movementSpeed = speed * Time.deltaTime * direction;
        transform.Translate(movementSpeed, 0, 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        hit = true;
        boxCollider.enabled = false;
        animator.SetTrigger("Explode");
        // Optionally deactivate the projectile after the animation
        // Invoke("Deactivate", 0.5f); // Adjust the time as needed
    }

    public void SetDirection(float _direction)
    {
        direction = _direction;
        gameObject.SetActive(true);
        hit = false;
        boxCollider.enabled = true;

        float localScaleX = transform.localScale.x;
        if (Mathf.Sign(localScaleX) != _direction)
            localScaleX = _direction;

        transform.localScale = new Vector3(localScaleX, transform.localScale.y, transform.localScale.z); // Corrected to use localScale
    }

    private void Deactivate()
    {
        gameObject.SetActive(false); // Deactivate the projectile
    }
}