using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TrashZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        InteractableDeskItem item = other.GetComponent<InteractableDeskItem>();
        
        // Verifica se o objeto que entrou na área é um Documento (Identidade)
        if (item != null && item.itemType == InteractableDeskItem.ItemType.Document2D)
        {
            if (UIManager.Instance != null) UIManager.Instance.ShowTrashButton(item);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        InteractableDeskItem item = other.GetComponent<InteractableDeskItem>();
        
        // Esconde o botão se o jogador arrastar a identidade para fora da área
        if (item != null && item.itemType == InteractableDeskItem.ItemType.Document2D)
        {
            if (UIManager.Instance != null) UIManager.Instance.HideTrashButton(item);
        }
    }
}