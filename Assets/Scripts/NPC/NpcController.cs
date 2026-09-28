using UnityEngine;
using System.Collections; // Necessário para Corrotinas

public class NpcController : MonoBehaviour
{
    public NpcData myData;
    
    private Transform deskSpawnPoint; 
    private bool idSpawned = false;

    private void Start()
    {
        GameObject pontoNaMesa = GameObject.Find("DeskSpawnPoint");
        
        if (pontoNaMesa != null)
        {
            deskSpawnPoint = pontoNaMesa.transform;
        }
        else
        {
            Debug.LogError("ERRO: O NPC não achou o 'DeskSpawnPoint' na cena!");
        }

        // Inicia a sequência de fala
        StartCoroutine(RoutineTalk());
    }

    private IEnumerator RoutineTalk()
    {
        // 1. O NPC solta a fala
        Debug.Log($"[{myData.npcName}]: {myData.initialDialogue}");
        
        // 2. Aguarda 2 segundos (ajuste este tempo conforme necessário ou vincule a um sistema de caixa de texto)
        yield return new WaitForSeconds(2f);
        
        // 3. Avisa a UI para mostrar o botão e passa este NPC como referência
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowAskIdButton(this);
        }
    }

    public void AskForID()
    {
        if (!idSpawned && myData.idCardPrefab != null && deskSpawnPoint != null)
        {
            Instantiate(myData.idCardPrefab, deskSpawnPoint.position, deskSpawnPoint.rotation);
            idSpawned = true;
            Debug.Log("Identidade colocada na mesa.");
        }
    }
    
    // ... (Mantenha o Start, a Corrotina e o AskForID do script anterior) ...

    // Chamado pelo UIManager quando o jogador entrega um item 3D da mesa
    public void ReceiveItem(string givenItemID)
    {
        if (myData.isScammer)
        {
            // Entregou algo para quem queria roubar: ERRO
            ScoreManager.Instance.RegisterAction(false, $"Entregou {givenItemID} para o GOLPISTA {myData.npcName}.");
        }
        else if (givenItemID == myData.correctItemID)
        {
            // Entregou o item certo: ACERTO
            ScoreManager.Instance.RegisterAction(true, $"Entregou {givenItemID} corretamente para {myData.npcName}.");
        }
        else
        {
            // Entregou o item errado: ERRO
            ScoreManager.Instance.RegisterAction(false, $"Entregou item errado ({givenItemID}) para {myData.npcName} (Esperava {myData.correctItemID}).");
        }

        // Passa para o próximo NPC
        FindObjectOfType<CycleManager>().StartCycle();
    }

    // Nova função: Chamada quando o jogador diz "Você não perdeu nada, vá embora"
    public void RefuseAndDismiss()
    {
        if (myData.isScammer)
        {
            // Mandou o golpista embora de mãos vazias: ACERTO
            ScoreManager.Instance.RegisterAction(true, $"Identificou o golpista {myData.npcName} e o mandou embora.");
            
            // FUTURO: Aqui entrará o evento de "Typing QTE" se o golpista tentar pegar o item à força
        }
        else
        {
            // Mandou embora alguém que realmente tinha um item: ERRO
            ScoreManager.Instance.RegisterAction(false, $"Recusou ajudar o NPC legítimo {myData.npcName}. Ele foi embora triste.");
        }

        // Passa para o próximo NPC
        FindObjectOfType<CycleManager>().StartCycle();
    }

    private void RegisterDelivery(string npc, string item, bool success)
    {
        Debug.Log($"[LOG DE TURNO] NPC: {npc} | Entregue: {item} | Acertou: {success}");
    }
}