using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    public float gravityScale;
    private bool isFacingRight = true;
    private bool isGrounded = false;
    public int level = 1;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator minion;
    private Animator bomb;

    public CoinManager cm; // For coin counter
 
    public int maxHealth = 5;
    public int currentHealth;
    public HealthBar healthBar;

    private bool flagTriggered = false;
    private Vector3 respawnPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        minion = GetComponent<Animator>();
        rb.gravityScale = gravityScale;
        bomb = GetComponent<Animator>();

        LoadPlayerData(); //save load
        healthBar.SetMaxHealth(maxHealth);
        healthBar.SetHealth(currentHealth);
        respawnPosition = transform.position; //save loads
    }

    void Update()
    {
        Move();
        Jump();
        CheckLevelTransition();
        Debug.Log("Persistent Data Path: " + Application.persistentDataPath);
    }

    void Move()
    {
        float move = 0f;

        if (Input.GetKey(KeyCode.A))
        {
            move = -1f;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            move = 1f;
        }

        rb.velocity = new Vector2(move * moveSpeed, rb.velocity.y);

        if (move > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (move < 0 && isFacingRight)
        {
            Flip();
        }
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.W) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            isGrounded = false;
        }
        isGrounded = true;

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    void CheckLevelTransition()
    {
        if (transform.position.x > -0.4f && transform.position.x < 0.89f && transform.position.y < 5.43f && transform.position.y > 3.91f)
        {
            SavePlayerData(); // Save data before transitioning to another scene
            SceneManager.LoadScene("level2");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("coin"))
        {
            Destroy(other.gameObject);
            cm.coinCount++;
        }
        else if (other.gameObject.CompareTag("bomb"))
        {
            bomb.SetTrigger("sparkPrefab");
            TakeDamage(1);
            Destroy(other.gameObject);
        }
        else if (other.gameObject.CompareTag("endFlag"))
        {
            // cm.coinCount = 0;
            SavePlayerData(); // Save data before transitioning to another scene
            SceneManager.LoadScene("level1");
        }
        else if (other.gameObject.CompareTag("Flag") && !flagTriggered)
        {
            currentHealth = maxHealth;
            healthBar.SetMaxHealth(maxHealth);
            cm.coinCount = 0;
            flagTriggered = true;
        }
        else if (other.gameObject.CompareTag("water"))
        {
            SavePlayerData(); // Save data before transitioning to another scene
            //Vector3 newPosition = new Vector3(-8.48f, 3.78f, 0f); 
            //SceneManager.LoadScene("level2");
            transform.position = respawnPosition;
        }
    }

    void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);

        if (currentHealth <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void SavePlayerData()
    {
        SaveSystem.SavePlayer(this);
    }

    public void LoadPlayerData()
    {
        PlayerData data = SaveSystem.LoadPlayer();
        if (data != null)
        {
            level = data.level;
            currentHealth = data.currentHealth;
            //transform.position = new Vector3(data.position[0], data.position[1], data.position[2]);
            cm.coinCount = data.coins;
        }
        else
        {
            currentHealth = maxHealth;  // Only set to max health if no data is loaded
        }
    }

}
