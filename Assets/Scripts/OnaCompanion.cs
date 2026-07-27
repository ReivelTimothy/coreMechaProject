using UnityEngine;

public class OnaCompanion : MonoBehaviour
{
    public static OnaCompanion Instance { get; private set; }

    [Header("Follow & Hover Settings")]
    public Transform playerTransform;
    public Vector3 offset = new Vector3(-1.2f, 1.2f, 0f);
    public float followSpeed = 5f;
    public float hoverFrequency = 2f;
    public float hoverAmplitude = 0.15f;

    [Header("Sprites & States")]
    public SpriteRenderer spriteRenderer;
    public Sprite standbySprite;   // Sprite Biru (Standby)
    public Sprite cautionSprite;   // Sprite Oranye/Danger (Caution)

    [Header("Hazard Detection")]
    public float dangerDetectionRadius = 4f;
    public LayerMask hazardLayer;  // Layer Trap / Spikes

    private bool isCaution = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();

        // Otomatis cari Player jika belum di-assign
        if (playerTransform == null)
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p != null) playerTransform = p.transform;
        }

        SetCautionState(false);
    }

    private void Update()
    {
        if (playerTransform == null) return;

        // 1. Logika Smooth Hovering di Belakang Player
        float hoverY = Mathf.Sin(Time.time * hoverFrequency) * hoverAmplitude;
        Vector3 targetPos = playerTransform.position + offset + new Vector3(0f, hoverY, 0f);
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * followSpeed);

        // 2. Logika Flip mengikuti arah hadap Player
        if (playerTransform.localScale.x < 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }

        // 3. Logika Deteksi Bahaya (Spike / Trap)
        DetectHazards();
    }

    private void DetectHazards()
    {
        Collider2D hazardNear = Physics2D.OverlapCircle(playerTransform.position, dangerDetectionRadius, hazardLayer);

        if (hazardNear != null)
        {
            SetCautionState(true);
        }
        else
        {
            SetCautionState(false);
        }
    }

    public void SetCautionState(bool caution)
    {
        isCaution = caution;
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = isCaution ? cautionSprite : standbySprite;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (playerTransform != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(playerTransform.position, dangerDetectionRadius);
        }
    }
}
