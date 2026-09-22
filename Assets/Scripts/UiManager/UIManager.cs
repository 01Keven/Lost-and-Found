using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Elementos de UI")]
    public GameObject askIdButton; // Arraste o botão de pedir identidade aqui

    private NpcController currentNpc;

    private void Awake()
    {
        // Padrão Singleton para acesso global rápido
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        // Garante que o botão comece invisível
        if (askIdButton != null) askIdButton.SetActive(false);
    }

    // Chamado pelo NpcController quando termina de falar
    public void ShowAskIdButton(NpcController npc)
    {
        currentNpc = npc;
        if (askIdButton != null) askIdButton.SetActive(true);
    }

    // Você vai vincular esta função no evento OnClick() do botão na Unity
    public void OnAskIdClicked()
    {
        if (currentNpc != null)
        {
            currentNpc.AskForID();
            askIdButton.SetActive(false); // Esconde o botão após pedir
        }
    }
}