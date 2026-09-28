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

    // Chamado pelo UIManager para exibir a tela
    public void Show(InteractableDeskItem item3D)
    {
        linked3DItem = item3D;
        
        if (item3D.inspectImage2D != null && displayImage != null)
        {
            displayImage.sprite = item3D.inspectImage2D;
        }
        
        gameObject.SetActive(true);
    }

    // Permite arrastar a UI pela tela
    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta;
    }

    // Traz a janela para a frente se houver outras UIs
    public void OnPointerDown(PointerEventData eventData)
    {
        rectTransform.SetAsLastSibling(); 
    }

    private void Update()
    {
        // Fecha com o clique direito do mouse se estiver ativo
        if (gameObject.activeSelf && Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame)
        {
            Close();
        }
    }

    private void Close()
    {
        gameObject.SetActive(false);
        
        // Faz o item 3D da mesa reaparecer
        if (linked3DItem != null)
        {
            linked3DItem.gameObject.SetActive(true);
        }
    }
}