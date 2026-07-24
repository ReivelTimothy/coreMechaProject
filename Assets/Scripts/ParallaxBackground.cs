using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [Header("Target Camera")]
    public Transform cameraTransform;

    [Header("Parallax Multiplier")]
    [Tooltip("0 = Diam tak berhingga (mengikuti kamera 100%)\n0.3 = Bergerak lambat (Jauh)\n0.7 = Bergerak agak cepat (Dekat)")]
    public Vector2 parallaxEffectMultiplier = new Vector2(0.5f, 0.5f);

    [Header("Smoothness")]
    [Tooltip("Semakin tinggi semakin halus / smooth gerakannya")]
    public float smoothing = 10f;

    [Header("Infinite Horizontal Loop")]
    public bool infiniteHorizontal = false;

    private Vector3 startPosition;
    private Vector3 startCameraPosition;
    private float textureSizeX;

    void Start()
    {
        if (cameraTransform == null)
        {
            if (Camera.main != null)
            {
                cameraTransform = Camera.main.transform;
            }
        }

        startPosition = transform.position;

        if (cameraTransform != null)
        {
            startCameraPosition = cameraTransform.position;
        }

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && spriteRenderer.sprite != null)
        {
            Texture2D texture = spriteRenderer.sprite.texture;
            textureSizeX = (texture.width / spriteRenderer.sprite.pixelsPerUnit) * transform.localScale.x;
        }
    }

    void LateUpdate()
    {
        if (cameraTransform == null) return;

        // Hitung jarak pergerakan kamera dari posisi awal
        float targetX = startPosition.x + (cameraTransform.position.x - startCameraPosition.x) * (1f - parallaxEffectMultiplier.x);
        float targetY = startPosition.y + (cameraTransform.position.y - startCameraPosition.y) * (1f - parallaxEffectMultiplier.y);

        Vector3 targetPosition = new Vector3(targetX, targetY, startPosition.z);

        // Gunakan Lerp agar pergerakan ultra-smooth tanpa getaran/jittering
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothing * Time.deltaTime);

        // Infinite horizontal loop jika diaktifkan
        if (infiniteHorizontal && textureSizeX > 0)
        {
            if (Mathf.Abs(cameraTransform.position.x - transform.position.x) >= textureSizeX)
            {
                float offsetPositionX = (cameraTransform.position.x - transform.position.x) % textureSizeX;
                transform.position = new Vector3(cameraTransform.position.x + offsetPositionX, transform.position.y, transform.position.z);
            }
        }
    }
}
