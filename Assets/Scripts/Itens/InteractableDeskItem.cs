using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider))]
public class InteractableDeskItem : MonoBehaviour
{
    public enum ItemType { Document2D, Object3D }
    public enum ItemLocation { OnShelf, OnDesk, Inspecting }

    public string itemID;
    public ItemType itemType;
    public ItemLocation currentLocation = ItemLocation.OnDesk; // Identidades já nascem na mesa

    [Header("UI de Inspeção (Apenas Documento 2D)")]
    public Sprite inspectImage2D;

    [Header("Limites da Mesa")]
    public float minX = -3f; public float maxX = 3f;
    public float minZ = -2f; public float maxZ = 2f;

    // Memória de Posição
    private Vector3 originalShelfPosition;
    private Quaternion originalShelfRotation;
    private Vector3 preInspectPosition;
    private Quaternion preInspectRotation;

    // Física e Arraste
    private float lockedYHeight;
    private Vector3 offset;
    private Plane deskPlane;
    private bool isDragging = false;

    // Inspeção 3D
    public float rotationSpeed = 0.5f;

    private void Start()
    {
        if (currentLocation == ItemLocation.OnShelf)
        {
            originalShelfPosition = transform.position;
            originalShelfRotation = transform.rotation;
        }
    }

    private void Update()
    {
        // Lógica de Inspeção 3D em tempo real
        if (currentLocation == ItemLocation.Inspecting && itemType == ItemType.Object3D)
        {
            // Girar o item com o botão esquerdo segurado
            if (Mouse.current != null && Mouse.current.leftButton.isPressed)
            {
                Vector2 delta = Mouse.current.delta.ReadValue();
                transform.Rotate(Camera.main.transform.up, -delta.x * rotationSpeed, Space.World);
                transform.Rotate(Camera.main.transform.right, delta.y * rotationSpeed, Space.World);
            }

            // Sair da inspeção com o botão direito
            if (Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame)
            {
                End3DInspection();
            }
        }
    }

    private void OnMouseDown()
    {
        // SE A TRAVA ESTIVER ATIVA, CANCELA A AÇÃO IMEDIATAMENTE
        if (DialogueManager.Instance != null && DialogueManager.Instance.isDialogueActive) return;

        if (currentLocation != ItemLocation.OnDesk) return; // Só arrasta se estiver na mesa

        isDragging = true;
        lockedYHeight = transform.position.y;
        deskPlane = new Plane(Vector3.up, new Vector3(0, lockedYHeight, 0));
        
        Ray ray = Camera.main.ScreenPointToRay(GetMouseScreenPosition());
        if (deskPlane.Raycast(ray, out float distance))
        {
            offset = transform.position - ray.GetPoint(distance);
        }
    }

    private void OnMouseOver()
    {
        // SE A TRAVA ESTIVER ATIVA, CANCELA A AÇÃO
        if (DialogueManager.Instance != null && DialogueManager.Instance.isDialogueActive) return;

        // Abre o menu de opções com botão direito (apenas se não estiver inspecionando)
        if (currentLocation != ItemLocation.Inspecting && Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame) 
        {
            if (UIManager.Instance != null) UIManager.Instance.OpenItemContextMenu(this);
        }
    }

    private void OnMouseUp() { isDragging = false; }

    private void OnMouseDrag()
    {
        if (!isDragging || currentLocation != ItemLocation.OnDesk) return;

        Ray ray = Camera.main.ScreenPointToRay(GetMouseScreenPosition());
        if (deskPlane.Raycast(ray, out float distance))
        {
            Vector3 targetPos = ray.GetPoint(distance) + offset;
            targetPos.y = lockedYHeight;
            targetPos.x = Mathf.Clamp(targetPos.x, minX, maxX);
            targetPos.z = Mathf.Clamp(targetPos.z, minZ, maxZ);
            transform.position = targetPos;
        }
    }

    

    public void MoveToDesk(Transform deskSpawnPoint)
    {
        currentLocation = ItemLocation.OnDesk;
        transform.position = deskSpawnPoint.position;
        transform.rotation = deskSpawnPoint.rotation;
        CameraController.Instance.ToggleView(); // Vira a câmera de volta para a mesa
    }

    public void ReturnToShelf()
    {
        currentLocation = ItemLocation.OnShelf;
        transform.position = originalShelfPosition;
        transform.rotation = originalShelfRotation;
    }

    public void Start3DInspection()
    {
        preInspectPosition = transform.position;
        preInspectRotation = transform.rotation;
        currentLocation = ItemLocation.Inspecting;

        // Move o item para frente da câmera principal
        transform.position = Camera.main.transform.position + Camera.main.transform.forward * 1.5f; 
    }

    private void End3DInspection()
    {
        // Retorna para onde estava antes (mesa ou prateleira)
        transform.position = preInspectPosition;
        transform.rotation = preInspectRotation;
        currentLocation = (preInspectPosition == originalShelfPosition) ? ItemLocation.OnShelf : ItemLocation.OnDesk;
    }

    private Vector2 GetMouseScreenPosition() => Mouse.current != null ? Mouse.current.position.ReadValue() : Vector2.zero;
}