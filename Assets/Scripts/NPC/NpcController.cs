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

    public bool hasAttemptedSteal = false;

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
        DialogueNode nextNode = null;

        if (myData.isScammer)
        {
            ScoreManager.Instance.RegisterAction(false, $"Entregou {givenItemID} para o GOLPISTA {myData.npcName}.");
            nextNode = myData.wrongItemNode;
        }
        else if (givenItemID == myData.correctItemID)
        {
            ScoreManager.Instance.RegisterAction(true, $"Entregou {givenItemID} corretamente para {myData.npcName}.");
            nextNode = myData.correctItemNode;
        }
        else
        {
            ScoreManager.Instance.RegisterAction(false, $"Entregou item errado ({givenItemID}) para {myData.npcName}.");
            nextNode = myData.wrongItemNode;
        }

        // Inicia a fala de despedida
        if (nextNode != null && DialogueManager.Instance != null)
            DialogueManager.Instance.StartDialogue(nextNode, this);
        else
            FinalizeAndLeave(); // Failsafe caso você esqueça de preencher o Node no Inspector
    }

    public void RefuseAndDismiss()
    {
        if (myData.isScammer)
            ScoreManager.Instance.RegisterAction(true, $"Identificou o golpista {myData.npcName} e o mandou embora.");
        else
            ScoreManager.Instance.RegisterAction(false, $"Recusou ajudar o NPC legítimo {myData.npcName}.");

        // Inicia a fala de despedida
        if (myData.dismissedNode != null && DialogueManager.Instance != null)
            DialogueManager.Instance.StartDialogue(myData.dismissedNode, this);
        else
            FinalizeAndLeave();
    }

    // Chamado pelo TypingQTEManager
    // Chamado pelo TypingQTEManager
    public void ResolveSteal(bool playerWon, InteractableDeskItem item)
    {
        if (playerWon)
        {
            ScoreManager.Instance.RegisterAction(true, $"Impediu o roubo de {myData.npcName}.");

            // O player venceu: desfaz o puxão e deixa o item seguro no centro da mesa
            if (item != null && deskSpawnPoint != null)
            {
                item.transform.position = deskSpawnPoint.position;
            }

            if (myData.stealFailNode != null) DialogueManager.Instance.StartDialogue(myData.stealFailNode, this);
            else FinalizeAndLeave();
        }
        else
        {
            ScoreManager.Instance.RegisterAction(false, $"{myData.npcName} conseguiu roubar o item!");
            
            if (item != null) 
            {
                // Desativa o item da cena NA MESMA HORA. Isso impede que o CycleManager tente guardá-lo.
                item.gameObject.SetActive(false); 
                Destroy(item.gameObject);
            }
            
            if (myData.stealSuccessNode != null) DialogueManager.Instance.StartDialogue(myData.stealSuccessNode, this);
            else FinalizeAndLeave();
        }
    }

    public void FinalizeAndLeave()
    {
        Object.FindAnyObjectByType<CycleManager>().FinishCurrentNpc();
    }
}