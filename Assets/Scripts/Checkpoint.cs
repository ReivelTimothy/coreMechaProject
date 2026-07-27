using System.Collections;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Header("Checkpoint Visuals")]
    public Color activeColor = Color.green;
    public Sprite activatedSprite; // Opsi jika pakai sprite bendera/apapun saat aktif

    [Header("UI Notification")]
    public GameObject checkpointUIPopup; // Image / Canvas bertuliskan "CHECKPOINT REACHED"
    public float popupDuration = 2f; // Durasi gambar muncul di layar (detik)

    private bool isActivated = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isActivated && collision.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            
            if (playerHealth != null)
            {
                playerHealth.SetNewRespawnPoint(transform.position);
                isActivated = true;

                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayCheckpointSFX();
                }

                // Penanda Visual 1: Ubah Sprite atau Jalankan Animasi
                Animator anim = GetComponent<Animator>();
                if (anim != null)
                {
                    anim.SetBool("IsActivated", true);
                    anim.SetTrigger("Activate");
                }
                else
                {
                    SpriteRenderer sr = GetComponent<SpriteRenderer>();
                    if (sr != null)
                    {
                        if (activatedSprite != null)
                        {
                            sr.sprite = activatedSprite;
                        }
                        else
                        {
                            sr.color = activeColor;
                        }
                    }
                }

                // Penanda Visual 2: Tampilkan Pop-Up UI "CHECKPOINT REACHED"
                if (checkpointUIPopup != null)
                {
                    StartCoroutine(ShowUIPopup());
                }

                Debug.Log($"Checkpoint {gameObject.name} berhasil diaktifkan!");
            }
        }
    }

    private IEnumerator ShowUIPopup()
    {
        checkpointUIPopup.SetActive(true);

        // Ambil atau tambahkan CanvasGroup untuk animasi transparansi (Fade In / Fade Out)
        CanvasGroup canvasGroup = checkpointUIPopup.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = checkpointUIPopup.AddComponent<CanvasGroup>();
        }

        // 1. Fade In (Muncul Mulus dari Transparan ke Jelas)
        float fadeSpeed = 3f;
        canvasGroup.alpha = 0f;

        while (canvasGroup.alpha < 1f)
        {
            canvasGroup.alpha += Time.deltaTime * fadeSpeed;
            yield return null;
        }
        canvasGroup.alpha = 1f;

        // 2. Tahan Muncul selama popupDuration
        yield return new WaitForSeconds(popupDuration);

        // 3. Fade Out (Hilang Mulus)
        while (canvasGroup.alpha > 0f)
        {
            canvasGroup.alpha -= Time.deltaTime * fadeSpeed;
            yield return null;
        }
        canvasGroup.alpha = 0f;

        checkpointUIPopup.SetActive(false);
    }
}