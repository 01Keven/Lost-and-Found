using UnityEngine;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Identidade e Turno")]
    public GameObject askIdButton; 
    private NpcController currentNpc;

    [Header("Interação de Itens")]
    public GameObject itemContextMenu; // O painelzinho com os 2 botões
    public DraggableInspectUI inspectUIWindow; // A janela de inspeção que criamos

    private InteractableDeskItem currentHoveredItem;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        if (askIdButton != null) askIdButton.SetActive(false);
        if (itemContextMenu != null) itemContextMenu.SetActive(false);
        if (inspectUIWindow != null) inspectUIWindow.gameObject.SetActive(false);
    }

    public void ShowAskIdButton(NpcController npc)
    {
        currentNpc = npc;
        if (askIdButton != null) askIdButton.SetActive(true);
    }

    public void OnAskIdClicked()
    {
        if (currentNpc != null)
        {
            currentNpc.AskForID();
            askIdButton.SetActive(false); 
        }
    }

    // ---- NOVAS FUNÇÕES PARA OS ITENS DA MESA ----

    public void OpenItemContextMenu(InteractableDeskItem item)
    {
        currentHoveredItem = item;
        itemContextMenu.SetActive(true);
        
        // Posiciona o menu no local exato do mouse na tela
        if (Mouse.current != null)
        {
            itemContextMenu.transform.position = Mouse.current.position.ReadValue();
        }
    }

    // Vincule esta função ao evento OnClick do botão "Inspect"
    public void OnInspectButtonClicked()
    {
        itemContextMenu.SetActive(false);

        if (currentHoveredItem != null)
        {
            // Oculta o objeto 3D
            currentHoveredItem.gameObject.SetActive(false);
            
            // Abre a janela de UI 2D passando o item como referência
            inspectUIWindow.Show(currentHoveredItem);
        }
    }

    // Vincule esta função ao evento OnClick do botão "Give Back"
    public void OnGiveBackButtonClicked()
    {
        itemContextMenu.SetActive(false);

        if (currentHoveredItem != null)
        {
            Debug.Log($"Devolvendo {currentHoveredItem.itemID} para o NPC. Transição de estado encaminhada.");
            
            // Aqui entra a lógica futura de escolher o item.
            // Por enquanto, destruímos o objeto para limpar a mesa.
            Destroy(currentHoveredItem.gameObject);
        }
    }
}