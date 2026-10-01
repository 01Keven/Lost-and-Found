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

    [Header("Configurações de Zoom (Diálogo)")]
    public float zoomFOV = 40f; // O valor do Field of View quando focado (menor = mais zoom)
    public float zoomTime = 0.3f; // Quão rápido o zoom acontece
    private float defaultFOV;
    private Coroutine zoomCoroutine;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        
        // Salva as rotações base
        deskRotation = transform.rotation;
        shelfRotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + 180f, transform.eulerAngles.z);

        // Salva o FOV padrão da câmera assim que o jogo começa
        if (Camera.main != null)
        {
            defaultFOV = Camera.main.fieldOfView;
        }
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

    // --- NOVAS FUNÇÕES DE ZOOM ---

    public void ZoomIn()
    {
        if (Camera.main == null) return;
        if (zoomCoroutine != null) StopCoroutine(zoomCoroutine);
        zoomCoroutine = StartCoroutine(ZoomRoutine(zoomFOV));
    }

    public void ZoomOut()
    {
        if (Camera.main == null) return;
        if (zoomCoroutine != null) StopCoroutine(zoomCoroutine);
        zoomCoroutine = StartCoroutine(ZoomRoutine(defaultFOV));
    }

    private IEnumerator ZoomRoutine(float targetFOV)
    {
        float startFOV = Camera.main.fieldOfView;
        float elapsed = 0f;

        while (elapsed < zoomTime)
        {
            // O Lerp faz a transição suave entre o zoom atual e o alvo
            Camera.main.fieldOfView = Mathf.Lerp(startFOV, targetFOV, elapsed / zoomTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
        Camera.main.fieldOfView = targetFOV;
    }
}