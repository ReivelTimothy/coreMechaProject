using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    // Global Flag untuk menandai apakah scene dimuat ulang untuk RETRY (langsung main) atau EXIT (buka Main Menu)
    public static bool shouldAutoStartGame = false;

    [Header("UI Panels & Elements")]
    public GameObject mainMenuPanel;
    public Button startButton;

    [Header("Game Control (Optional Direct Reference)")]
    public bool pauseTimeOnStart = true;

    [Header("Gameplay UI Elements")]
    public GameObject healthUIContainer; // Panel UI HP (Kepala MC)

    private void Start()
    {
        // Jika dipicu dari tombol RETRY (mati / finish), langsung masuk ke permainan tanpa tampilkan Main Menu
        if (shouldAutoStartGame)
        {
            if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
            if (healthUIContainer != null) healthUIContainer.SetActive(true);

            Time.timeScale = 1f;

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayInGameBGM();
            }

            return;
        }

        // Tampilan Normal Main Menu saat awal buka game atau dipicu dari tombol EXIT
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(true);
        }

        // Play BGM Main Menu
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMainMenuBGM();
        }

        // Sembunyikan UI HP saat di Main Menu
        if (healthUIContainer != null)
        {
            healthUIContainer.SetActive(false);
        }

        // Freeze waktu game jika pauseTimeOnStart dicentang
        if (pauseTimeOnStart)
        {
            Time.timeScale = 0f;
        }

        // Pasang listener pada Start Button jika dislot di Inspector
        if (startButton != null)
        {
            startButton.onClick.AddListener(StartGame);
        }
    }

    public void StartGame()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClickSFX();
            AudioManager.Instance.PlayInGameBGM();
        }

        // Sembunyikan Main Menu UI
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(false);
        }

        // Tampilkan UI HP saat game mulai dimainkan
        if (healthUIContainer != null)
        {
            healthUIContainer.SetActive(true);
        }

        // Kembalikan alur waktu normal game
        Time.timeScale = 1f;
    }
}
