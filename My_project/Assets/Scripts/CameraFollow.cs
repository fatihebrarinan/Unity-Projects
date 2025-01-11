using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // Takip edilecek karakter
    public float smoothSpeed = 0.125f; // Kameran�n takip h�z�
    public Vector3 offset; // Kameran�n karaktere g�re pozisyonu

    void LateUpdate()
    {
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}