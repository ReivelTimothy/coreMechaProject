using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 3;
    public int currentHealth;

    [Header("Respawn Settings")]
    public Vector3 currentRespawnPoint;

    private Rigidbody2D rb;

    private void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();

        // Set titik respawn awal ke posisi tempat Player pertama kali ditaruh di Map
        currentRespawnPoint = transform.position;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"Player terkena damage! Sisa HP: {currentHealth}");

        if (currentHealth > 0)
        {
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
        Debug.Log("HP Habis! Game Over / Reload Scene.");
        // Reload scene saat ini jika HP habis (otomatis reset ke titik awal map)
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Dipanggil oleh objek Checkpoint untuk memperbarui titik respawn
    public void SetNewRespawnPoint(Vector3 newPosition)
    {
        currentRespawnPoint = newPosition;
        Debug.Log($"Titik Respawn diperbarui ke: {newPosition}");
    }
}