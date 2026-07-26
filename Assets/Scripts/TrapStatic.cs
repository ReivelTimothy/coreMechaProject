using UnityEngine;

public class TrapStatic : TrapBase
{
    [Header("Static / Rotation Settings")]
    public bool isRotating = false;
    public float rotationSpeed = 180f;

    protected virtual void Update()
    {
        if (isRotating)
        {
            transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
        }
    }
}