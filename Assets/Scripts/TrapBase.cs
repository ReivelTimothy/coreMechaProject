using UnityEngine;

public class TrapBase : MonoBehaviour
{
    [Header("Base Trap Settings")]
    public int damageAmount = 1;

    protected virtual void OnPlayerHit(GameObject player)
    {
        // Cari PlayerHealth baik di GameObject ini, Parent, atau Child
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth == null)
        {
            playerHealth = player.GetComponentInParent<PlayerHealth>();
        }
        if (playerHealth == null)
        {
            playerHealth = player.GetComponentInChildren<PlayerHealth>();
        }

        if (playerHealth != null)
        {
            Debug.Log($"[Trap] Trap menyentuh Player! Memberikan damage: {damageAmount}");
            playerHealth.TakeDamage(damageAmount);
        }
        else
        {
            Debug.LogWarning($"[Trap] Objek '{player.name}' tersentuh trap, tetapi tidak ditemukan script PlayerHealth di objek/parent/child tersebut!");
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || (collision.transform.parent != null && collision.transform.parent.CompareTag("Player")))
        {
            OnPlayerHit(collision.gameObject);
        }
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") || (collision.transform.parent != null && collision.transform.parent.CompareTag("Player")))
        {
            OnPlayerHit(collision.gameObject);
        }
    }
}