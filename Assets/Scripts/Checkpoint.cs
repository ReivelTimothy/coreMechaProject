using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Header("Checkpoint Visuals")]
    public Color activeColor = Color.green; // Warna saat checkpoint diaktifkan
    
    private bool isActivated = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Cek jika objek yang menyenggol punya Tag "Player" dan checkpoint belum aktif
        if (!isActivated && collision.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            
            if (playerHealth != null)
            {
                // Ambil posisi checkpoint ini dan set sebagai titik respawn baru di PlayerHealth
                playerHealth.SetNewRespawnPoint(transform.position);
                isActivated = true;

                // Penanda Visual: Ubah warna Sprite menjadi Hijau saat berhasil disentuh
                SpriteRenderer sr = GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.color = activeColor;
                }

                Debug.Log($"Checkpoint {gameObject.name} berhasil diaktifkan!");
            }
        }
    }
}