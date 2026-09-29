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

    // 1. Adicione esta nova função em qualquer lugar do seu UIManager
    public void ClearNpcState()
    {
        currentNpc = null;
        hasReturnedID = false; // Reseta a permissão de entregar itens
    }

    // 2. Substitua a sua função OpenItemContextMenu atual por esta versão:
    public void OpenItemContextMenu(InteractableDeskItem item)
    {
        currentHoveredItem = item;
        itemContextMenu.SetActive(true);
        
        if (Mouse.current != null) itemContextMenu.transform.position = Mouse.current.position.ReadValue();

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
            btnReturnToShelf.SetActive(item.itemType == InteractableDeskItem.ItemType.Object3D);
            
            // TRAVA DE SEGURANÇA: Verifica se existe alguém no balcão
            bool isNpcAtDesk = (currentNpc != null);

            if (item.itemType == InteractableDeskItem.ItemType.Document2D)
            {
                btnGiveBack.SetActive(isNpcAtDesk); // Identidade só volta se tiver NPC
            }
            else
            {
                // Item 3D só é entregue se tiver NPC E a identidade já tiver sido devolvida
                btnGiveBack.SetActive(isNpcAtDesk && hasReturnedID);
            }
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
            
            // Esconde os elementos da UI após o uso
            btnDismissNpc.SetActive(false); 
            itemContextMenu.SetActive(false); // Garante que o menu do item fecha para evitar cliques fantasmas
        }
    }
}