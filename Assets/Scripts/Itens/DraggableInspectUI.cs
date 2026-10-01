using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DraggableInspectUI : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    public Image displayImage;
    private InteractableDeskItem linked3DItem;
    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    // 1. Usado para as Identidades (Oculta o item 3D)
    public void Show(InteractableDeskItem item3D)
    {
        linked3DItem = item3D;
        
        if (item3D.inspectImage2D != null && displayImage != null)
        {
            displayImage.sprite = item3D.inspectImage2D;
        }
        
        gameObject.SetActive(true);
    }

    // 2. NOVA FUNÇÃO: Usado para papéis soltos/manuais (Não oculta o 3D)
    public void ShowStaticNote(Sprite noteSprite)
    {
        linked3DItem = null; // Garante que não há vínculo de desaparecimento
        
        if (noteSprite != null && displayImage != null)
        {
            displayImage.sprite = noteSprite;
        }
        
        gameObject.SetActive(true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        rectTransform.SetAsLastSibling(); 
    }

    private void Update()
    {
        if (gameObject.activeSelf && Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame)
        {
            Close();
        }
    }

    private void Close()
    {
        gameObject.SetActive(false);
        
        // Só tenta reativar se for uma identidade que estava vinculada
        if (linked3DItem != null)
        {
            linked3DItem.gameObject.SetActive(true);
        }
    }
}