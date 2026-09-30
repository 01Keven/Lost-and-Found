using UnityEngine;
using System.Collections;

public class NpcController : MonoBehaviour
{
    public NpcData myData;
    
    private Transform deskSpawnPoint; 
    private bool idSpawned = false;

    // ...
    public DialogueNode lastNode; // Guarda a última conversa
    // ...

    private void Start()
    {
        GameObject pontoNaMesa = GameObject.Find("DeskSpawnPoint");
        if (pontoNaMesa != null) deskSpawnPoint = pontoNaMesa.transform;
        else Debug.LogError("ERRO: O NPC não achou o 'DeskSpawnPoint' na cena!");

        // 1. Avisa a Interface quem é este NPC (Conserta o bug do "Give Back")
        if (UIManager.Instance != null)
        {
            UIManager.Instance.SetCurrentNpc(this);
        }

        // 2. Inicia o sistema de diálogo
        if (myData.startingNode != null && DialogueManager.Instance != null)
        {
            DialogueManager.Instance.StartDialogue(myData.startingNode, this);
        }
    }

    // 3. Detecta o clique no corpo do NPC
    private void OnMouseDown()
    {
        if (lastNode != null && DialogueManager.Instance != null)
        {
            // Reabre o diálogo na última fala registrada
            DialogueManager.Instance.StartDialogue(lastNode, this);
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

    public void ReceiveItem(string givenItemID)
    {
        if (myData.isScammer)
        {
            ScoreManager.Instance.RegisterAction(false, $"Entregou {givenItemID} para o GOLPISTA {myData.npcName}.");
        }
        else if (givenItemID == myData.correctItemID)
        {
            ScoreManager.Instance.RegisterAction(true, $"Entregou {givenItemID} corretamente para {myData.npcName}.");
        }
        else
        {
            ScoreManager.Instance.RegisterAction(false, $"Entregou item errado ({givenItemID}) para {myData.npcName} (Esperava {myData.correctItemID}).");
        }

        Object.FindAnyObjectByType<CycleManager>().FinishCurrentNpc();
    }

    public void RefuseAndDismiss()
    {
        if (myData.isScammer)
        {
            ScoreManager.Instance.RegisterAction(true, $"Identificou o golpista {myData.npcName} e o mandou embora.");
        }
        else
        {
            ScoreManager.Instance.RegisterAction(false, $"Recusou ajudar o NPC legítimo {myData.npcName}. Ele foi embora triste.");
        }

        Object.FindAnyObjectByType<CycleManager>().FinishCurrentNpc();
    }
}