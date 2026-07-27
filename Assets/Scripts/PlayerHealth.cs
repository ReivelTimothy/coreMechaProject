using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 3;
    public int currentHealth;

    [Header("Respawn Settings")]
    public Vector3 currentRespawnPoint;

    [Header("Game Over UI")]
    public GameObject gameOverPanel;
    public UnityEngine.UI.Button retryButton;

    private Rigidbody2D rb;

    private void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();

        // Set titik respawn awal ke posisi tempat Player pertama kali ditaruh di Map
        currentRespawnPoint = transform.position;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        if (retryButton != null)
        {
            retryButton.onClick.AddListener(RestartGame);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"Player terkena damage! Sisa HP: {currentHealth}");

        if (currentHealth > 0)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayHurtSFX();
            }

            Respawn();
        }
        else
        {
            Die();
        }
    }

    public void Respawn()
    {
        // Pindahkan posisi Player ke titik respawn terakhir
        transform.position = currentRespawnPoint;

        // Reset momentum kecepatan fisik agar player tidak meluncur saat respawn
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        Debug.Log("Player kembali ke Respawn Point terakhir.");
    }

    private void Die()
    {
        Debug.Log("HP Habis! Game Over.");

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayDieSFX();
        }

        // Matikan pergerakan/karakter jika perlu
        gameObject.SetActive(false);

        // Freeze game dan tampilkan Game Over Panel "YOU DIED!"
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            Time.timeScale = 0f;
        }
        else
        {
            RestartGame();
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClickSFX();
            AudioManager.Instance.PlayInGameBGM();
        }

        // Tandai bahwa game di-restart secara langsung (melewati Main Menu)
        MainMenuUI.shouldAutoStartGame = true;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Dipanggil oleh objek Checkpoint untuk memperbarui titik respawn
    public void SetNewRespawnPoint(Vector3 newPosition)
    {
        currentRespawnPoint = newPosition;
        Debug.Log($"Titik Respawn diperbarui ke: {newPosition}");
    }
}