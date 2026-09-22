using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider))]
public class InteractableDeskItem : MonoBehaviour
{
    public string itemID; // Ex: "id_ze", "relogio_ouro"
    
    private Vector3 offset;
    private float zCoord;
    private bool isDragging = false;

    // Detecta o clique esquerdo para arrastar
    private void OnMouseDown()
    {
        zCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
        offset = gameObject.transform.position - GetMouseAsWorldPoint();
        isDragging = true;
    }

    private void OnMouseUp()
    {
        isDragging = false;
    }

    private void OnMouseDrag()
    {
        transform.position = GetMouseAsWorldPoint() + offset;
    }

    // O Update checa o botão direito para inspecionar, mas só se o mouse estiver sobre o objeto
    private void OnMouseOver()
    {
        // Verifica se há um mouse conectado e se o botão direito foi pressionado neste frame
        if (Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame) 
        {
            InspectItem();
        }
    }

    private void InspectItem()
    {
        Debug.Log($"Inspecionando detalhadamente o item: {itemID}");
        // Aqui você aciona um evento para abrir a UI de Inspeção.
        // Exemplo: InspectionUI.Instance.ShowDetails(this);
    }

    private Vector3 GetMouseAsWorldPoint()
    {
        // Lê a posição do mouse usando o New Input System
        Vector2 mouseScreenPos = Mouse.current != null ? Mouse.current.position.ReadValue() : Vector2.zero;
        
        Vector3 mousePoint = new Vector3(mouseScreenPos.x, mouseScreenPos.y, zCoord);
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }
}