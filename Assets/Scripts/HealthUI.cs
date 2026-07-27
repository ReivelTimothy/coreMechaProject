using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [Header("Player Health Reference")]
    public PlayerHealth playerHealth;

    [Header("UI Icons Settings")]
    public Image headIconPrefab; // Image prefab berisi sprite kepala MC
    public Transform heartsContainer; // Parent container tempat ikon kepala dideretkan

    private List<Image> spawnedIcons = new List<Image>();

    private void Start()
    {
        if (playerHealth == null)
        {
            playerHealth = FindFirstObjectByType<PlayerHealth>();
        }

        SetupHealthIcons();
    }

    private void Update()
    {
        if (playerHealth != null)
        {
            UpdateHealthIcons(playerHealth.currentHealth);
        }
    }

    public void SetupHealthIcons()
    {
        if (playerHealth == null || heartsContainer == null) return;

        // Bersihkan child yang merupakan hasil spawn sebelumnya
        foreach (Transform child in heartsContainer)
        {
            if (headIconPrefab != null && child.gameObject == headIconPrefab.gameObject)
            {
                // Jangan hapus prefab template jika ditaruh di dalam container
                child.gameObject.SetActive(false);
                continue;
            }
            Destroy(child.gameObject);
        }
        spawnedIcons.Clear();

        if (headIconPrefab == null) return;

        // Spawn ikon kepala sebanyak Max HP
        for (int i = 0; i < playerHealth.maxHealth; i++)
        {
            Image newIcon = Instantiate(headIconPrefab, heartsContainer);
            newIcon.gameObject.SetActive(true);
            spawnedIcons.Add(newIcon);
        }
    }

    public void UpdateHealthIcons(int currentHP)
    {
        for (int i = 0; i < spawnedIcons.Count; i++)
        {
            if (i < currentHP)
            {
                // HP ada: Tampilkan penuh (berwarna)
                spawnedIcons[i].color = Color.white;
                spawnedIcons[i].gameObject.SetActive(true);
            }
            else
            {
                // HP berkurang: Bisa disembunyikan atau di-faded/redupkan
                spawnedIcons[i].gameObject.SetActive(false);
            }
        }
    }
}
