using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Identidade e Turno")]
    public GameObject askIdButton; 
    private NpcController currentNpc;
    public bool hasReturnedID = false; // Controle de estado da devolução

    [Header("Interação de Itens")]
    public GameObject itemContextMenu;
    public DraggableInspectUI inspectUIWindow; 
    public Transform deskItemSpawnPoint; // Ponto central da mesa para onde os itens da prateleira vão

    [Header("Botões do Menu de Contexto")]
    public GameObject btnInspect;
    public GameObject btnPutOnTable;
    public GameObject btnReturnToShelf;
    public GameObject btnGiveBack;
    public GameObject btnDismissNpc;

    private InteractableDeskItem currentHoveredItem;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        if (askIdButton != null) askIdButton.SetActive(false);
        if (itemContextMenu != null) itemContextMenu.SetActive(false);
    }

    public void ShowAskIdButton(NpcController npc)
    {
        currentNpc = npc;
        hasReturnedID = false;
        
        if (askIdButton != null) askIdButton.SetActive(true);
        if (btnDismissNpc != null) btnDismissNpc.SetActive(false); // Esconde ao iniciar
    }

    public void OnAskIdClicked()
    {
        if (currentNpc != null)
        {
            currentNpc.AskForID();
            askIdButton.SetActive(false); 
        }
    }

    public void OpenItemContextMenu(InteractableDeskItem item)
    {
        currentHoveredItem = item;
        itemContextMenu.SetActive(true);
        
        if (Mouse.current != null) itemContextMenu.transform.position = Mouse.current.position.ReadValue();

        // Configura quais botões aparecem baseados no estado do item
        btnInspect.SetActive(true);
        
        if (item.currentLocation == InteractableDeskItem.ItemLocation.OnShelf)
        {
            btnPutOnTable.SetActive(true);
            btnReturnToShelf.SetActive(false);
            btnGiveBack.SetActive(false);
        }
        else if (item.currentLocation == InteractableDeskItem.ItemLocation.OnDesk)
        {
            btnPutOnTable.SetActive(false);
            
            // Só pode devolver pra prateleira se for um objeto 3D
            btnReturnToShelf.SetActive(item.itemType == InteractableDeskItem.ItemType.Object3D);
            
            // Se for identidade, "Give Back" sempre aparece. Se for item 3D, depende de já ter devolvido a ID.
            if (item.itemType == InteractableDeskItem.ItemType.Document2D)
                btnGiveBack.SetActive(true);
            else
                btnGiveBack.SetActive(hasReturnedID);
        }
    }

    // --- FUNÇÕES DOS BOTÕES DO MENU DE CONTEXTO ---

    public void OnInspectButtonClicked()
    {
        itemContextMenu.SetActive(false);
        if (currentHoveredItem == null) return;

        if (currentHoveredItem.itemType == InteractableDeskItem.ItemType.Document2D)
        {
            currentHoveredItem.gameObject.SetActive(false);
            inspectUIWindow.Show(currentHoveredItem); // Abre UI 2D
        }
        else
        {
            currentHoveredItem.Start3DInspection(); // Puxa pra câmera
        }
    }

    public void OnPutOnTableButtonClicked()
    {
        itemContextMenu.SetActive(false);
        if (currentHoveredItem != null) currentHoveredItem.MoveToDesk(deskItemSpawnPoint);
    }

    public void OnReturnToShelfButtonClicked()
    {
        itemContextMenu.SetActive(false);
        if (currentHoveredItem != null) currentHoveredItem.ReturnToShelf();
    }

    public void OnGiveBackButtonClicked()
    {
        itemContextMenu.SetActive(false);
        if (currentHoveredItem == null) return;

        if (currentHoveredItem.itemType == InteractableDeskItem.ItemType.Document2D)
        {
            hasReturnedID = true; 
            
            // Mostra o botão de expulsar o NPC agora que a identidade foi checada e devolvida
            if (btnDismissNpc != null) btnDismissNpc.SetActive(true);
            
            Destroy(currentHoveredItem.gameObject);
        }
        else
        {
            // O NPC recebe o item 3D
            currentNpc.ReceiveItem(currentHoveredItem.itemID);
            if (btnDismissNpc != null) btnDismissNpc.SetActive(false); // Esconde a UI
            Destroy(currentHoveredItem.gameObject);
        }
    }

    public void OnDismissNpcClicked()
    {
        if (currentNpc != null)
        {
            currentNpc.RefuseAndDismiss();
            btnDismissNpc.SetActive(false); // Esconde após usar
        }
    }
}