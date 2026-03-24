using System.Collections;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private float moveSpeed = 3f;

    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Animator animator;

    private Vector2 input;
    private PlayerHealthBar healthBar;

    private int currentHealth;
    private float lastDamageTime;
    private const float damageCooldown = 0.6f;

    private bool isDead;
    private bool controlsEnabled = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();

        healthBar = FindObjectOfType<PlayerHealthBar>();

        currentHealth = maxHealth;

        if (healthBar)
        {
            healthBar.UpdateHealth(currentHealth, maxHealth);
        }
    }

    private void Update()
    {
        if (!controlsEnabled || PauseController.isGamePaused)
            return;

        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");

        input = input.normalized;
        UpdateAnimation();
    }

    private void FixedUpdate()
    {
        if (!controlsEnabled || PauseController.isGamePaused)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        rb.linearVelocity = input * moveSpeed;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Enemy")) return;
        if (Time.time - lastDamageTime < damageCooldown) return;
        if (isDead) return;

        TakeDamage(10);
    }

    private void TakeDamage(int amount)
    {
        currentHealth -= amount;
        lastDamageTime = Time.time;

        StartCoroutine(DamageFlash());

        healthBar?.UpdateHealth(currentHealth, maxHealth);

        if (currentHealth <= 0)
            StartCoroutine(Die());
    }

    private IEnumerator DamageFlash()
    {
        sprite.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        sprite.color = Color.white;
    }

    private IEnumerator Die()
    {
        isDead = true;
        controlsEnabled = false;
        rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(0.5f);

        if (ArenaState.ActiveArena != null)
        {
            ArenaState.ActiveArena.OnPlayerDied();
        }
    }

    public void Respawn()
    {
        isDead = false;
        controlsEnabled = true;
        currentHealth = maxHealth;

        rb.linearVelocity = Vector2.zero;

        healthBar?.UpdateHealth(currentHealth, maxHealth);
    }

    public void DisableControls()
    {
        controlsEnabled = false;
        rb.linearVelocity = Vector2.zero;
    }

    public void EnableControls()
    {
        controlsEnabled = true;
    }

    private void UpdateAnimation()
    {
        if (!animator) return;

        animator.SetBool("IsWalking", input != Vector2.zero);

        if (input != Vector2.zero)
        {
            animator.SetFloat("MoveX", input.x);
            animator.SetFloat("MoveY", input.y);
            animator.SetFloat("LastMoveX", input.x);
            animator.SetFloat("LastMoveY", input.y);
        }
    }
}