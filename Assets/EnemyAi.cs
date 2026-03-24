using UnityEngine;
using TMPro;

public class EnemyAi : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private int enemyHealth = 60;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float chaseRadius = 5f;
    [SerializeField] private float damageCooldown = 0.3f;

    [Header("UI")]
    [SerializeField] private GameObject damageTextPrefab;

    private float lastDamageTime = -999f;
    private Rigidbody2D rb;
    private Animator animator;
    private Transform player;
    private Vector2 moveDir;
    private FloatingHealthBar healthBar;

    private ArenaController ownerArena;

    public void SetArena(ArenaController arena)
    {
        ownerArena = arena;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        healthBar = GetComponentInChildren<FloatingHealthBar>();
        if (healthBar != null)
        {
            healthBar.SetTarget(transform);
            healthBar.offset = new Vector3(0, 0.5f, 0);
            healthBar.UpdateHealthBar(enemyHealth, enemyHealth);
        }
    }

    void FixedUpdate()
    {
        if (player == null)
            return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= chaseRadius)
        {
            moveDir = (player.position - transform.position).normalized;
            rb.linearVelocity = moveDir * moveSpeed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            moveDir = Vector2.zero;
        }

        if (PauseController.isGamePaused)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (animator != null && moveDir != Vector2.zero)
        {
            animator.SetFloat("MoveX", moveDir.x);
            animator.SetFloat("MoveY", moveDir.y);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Weapon"))
            return;

        if (Time.time - lastDamageTime < damageCooldown || enemyHealth <= 0)
            return;

        lastDamageTime = Time.time;
        enemyHealth -= 10;

        CameraShake.Instance?.Shake(0.3f);

        ShowDamageText("-10");

        if (healthBar != null)
            healthBar.UpdateHealthBar(enemyHealth, 60);

        if (enemyHealth <= 0)
            Die();
    }

    private void Die()
    {
        ownerArena?.NotifyEnemyKilled();
        Destroy(gameObject);
    }

    private void ShowDamageText(string text)
    {
        if (damageTextPrefab == null || Camera.main == null)
            return;

        GameObject canvas = GameObject.Find("Canvas");
        if (canvas == null)
            return;

        GameObject dmg = Instantiate(damageTextPrefab, canvas.transform);
        dmg.GetComponent<TextMeshProUGUI>()?.SetText(text);

        Vector2 screenPos = Camera.main.WorldToScreenPoint(transform.position + Vector3.up);
        dmg.GetComponent<RectTransform>().position = screenPos;
    }
}