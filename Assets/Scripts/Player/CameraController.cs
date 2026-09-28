using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;

    [Header("Configurações de Rotação")]
    public float rotationTime = 0.5f;
    
    private bool isLookingAtShelf = false;
    private bool isRotating = false;
    private Quaternion deskRotation;
    private Quaternion shelfRotation;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        
        // Salva as rotações base
        deskRotation = transform.rotation;
        shelfRotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + 180f, transform.eulerAngles.z);
    }

    public void ToggleView()
    {
        if (isRotating) return;
        isLookingAtShelf = !isLookingAtShelf;
        StartCoroutine(RotateCameraRoutine(isLookingAtShelf ? shelfRotation : deskRotation));
    }

    private IEnumerator RotateCameraRoutine(Quaternion targetRot)
    {
        isRotating = true;
        Quaternion startRot = transform.rotation;
        float elapsed = 0f;

        while (elapsed < rotationTime)
        {
            transform.rotation = Quaternion.Slerp(startRot, targetRot, elapsed / rotationTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.rotation = targetRot;
        isRotating = false;
    }
}