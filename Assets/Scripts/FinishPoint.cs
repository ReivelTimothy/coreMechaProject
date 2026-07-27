using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FinishPoint : MonoBehaviour
{
    [Header("Finish UI Elements")]
    public GameObject finishPanel; // Panel "TO BE CONTINUED"
    public Button retryButton;     // Tombol Retry
    public Button exitButton;      // Tombol Exit

    [Header("Collect Transition Settings")]
    public float collectAnimationDuration = 0.6f; // Durasi efek kristal menghilang saat disentuh

    private bool isFinished = false;

    private void Start()
    {
        if (finishPanel != null)
        {
            finishPanel.SetActive(false);
        }

        if (retryButton != null)
        {
            retryButton.onClick.AddListener(RestartGame);
        }

        if (exitButton != null)
        {
            exitButton.onClick.AddListener(ExitGame);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isFinished && collision.CompareTag("Player"))
        {
            isFinished = true;
            StartCoroutine(FinishSequence());
        }
    }

    private IEnumerator FinishSequence()
    {
        Debug.Log("Player menyentuh Crystal Heart Finish Point!");

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayFinishSFX();
        }

        // 1. Jalankan Animasi Parameter jika pakai Animator (misal Trigger "Collect")
        Animator anim = GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetTrigger("Collect");
        }

        // 2. Animasi Visual Kristal Menghilang (Scale mengecil + Transparan/Fade Out)
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Vector3 initialScale = transform.localScale;
        Color initialColor = sr != null ? sr.color : Color.white;
        float elapsed = 0f;

        while (elapsed < collectAnimationDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / collectAnimationDuration;

            // Efek mengecil sambil naik sedikit (pop up effect)
            transform.localScale = Vector3.Lerp(initialScale, initialScale * 1.5f, t) * (1f - t);

            if (sr != null)
            {
                Color c = initialColor;
                c.a = Mathf.Lerp(1f, 0f, t);
                sr.color = c;
            }

            yield return null;
        }

        // Sembunyikan sprite kristal setelah animasi hilang selesai
        if (sr != null) sr.enabled = false;

        // 3. Tampilkan Panel UI "TO BE CONTINUED" dengan Fade In Mulus
        if (finishPanel != null)
        {
            finishPanel.SetActive(true);

            CanvasGroup canvasGroup = finishPanel.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = finishPanel.AddComponent<CanvasGroup>();
            }

            canvasGroup.alpha = 0f;
            float fadeSpeed = 3f;

            while (canvasGroup.alpha < 1f)
            {
                canvasGroup.alpha += Time.unscaledDeltaTime * fadeSpeed;
                yield return null;
            }
            canvasGroup.alpha = 1f;

            Time.timeScale = 0f; // Freeze game setelah transisi layar selesai sempurna
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

    public void ExitGame()
    {
        Time.timeScale = 1f;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClickSFX();
            AudioManager.Instance.PlayMainMenuBGM();
        }

        // Kembalikan ke Main Menu awal
        MainMenuUI.shouldAutoStartGame = false;

        Debug.Log("Keluar ke Main Menu.");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
