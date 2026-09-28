using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider))]
public class InteractableDeskItem : MonoBehaviour
{
    public string itemID;
    
    public Sprite inspectImage2D;

    [Header("Limites da Mesa ")]
    [Tooltip("impedir que o item caia da mesa")]
    public float minX = -3f;
    public float maxX = 3f;
    public float minZ = -2f;
    public float maxZ = 2f;

    private float lockedYHeight;
    private Vector3 offset;
    private Plane deskPlane;
    private bool isDragging = false;

    private void OnMouseDown()
    {
        isDragging = true;
        
        // 1. Salva a altura inicial exata do objeto (Eixo Y) para não deixá-lo flutuar
        lockedYHeight = transform.position.y;
        
        // 2. Cria um plano matemático invisível virado para cima, exatamente na altura do item
        deskPlane = new Plane(Vector3.up, new Vector3(0, lockedYHeight, 0));
        
        // 3. Calcula a diferença entre onde o mouse clicou e o centro do objeto
        Ray ray = Camera.main.ScreenPointToRay(GetMouseScreenPosition());
        if (deskPlane.Raycast(ray, out float distance))
        {
            Vector3 hitPoint = ray.GetPoint(distance);
            offset = transform.position - hitPoint;
        }
    }

    private void OnMouseUp()
    {
        isDragging = false;
    }

    private void OnMouseDrag()
    {
        // 4. Lança um raio da câmera até o mouse
        Ray ray = Camera.main.ScreenPointToRay(GetMouseScreenPosition());
        
        // 5. Verifica onde o raio intercepta o plano da mesa
        if (deskPlane.Raycast(ray, out float distance))
        {
            Vector3 targetPos = ray.GetPoint(distance) + offset;
            
            // 6. Trava a altura (Y) para o valor inicial
            targetPos.y = lockedYHeight;
            
            // 7. Trava o movimento lateral (X) e profundidade (Z) dentro dos limites da mesa
            targetPos.x = Mathf.Clamp(targetPos.x, minX, maxX);
            targetPos.z = Mathf.Clamp(targetPos.z, minZ, maxZ);
            
            // 8. Aplica a posição final corrigida
            transform.position = targetPos;
        }
    }

    private void OnMouseOver()
    {
        if (Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame) 
        {
            if (UIManager.Instance != null)
            {
                UIManager.Instance.OpenItemContextMenu(this);
            }
            InspectItem();
        }
    }

    private void InspectItem()
    {
        Debug.Log($"Inspecionando detalhadamente o item: {itemID}");
    }

    private Vector2 GetMouseScreenPosition()
    {
        return Mouse.current != null ? Mouse.current.position.ReadValue() : Vector2.zero;
    }
}