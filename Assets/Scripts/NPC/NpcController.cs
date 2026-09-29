using UnityEngine;
using System.Collections;

public class NpcController : MonoBehaviour
{
    public NpcData myData;
    
    private Transform deskSpawnPoint; 
    private bool idSpawned = false;

    private void Start()
    {
        GameObject pontoNaMesa = GameObject.Find("DeskSpawnPoint");
        if (pontoNaMesa != null) deskSpawnPoint = pontoNaMesa.transform;
        else Debug.LogError("ERRO: O NPC não achou o 'DeskSpawnPoint' na cena!");

        // Inicia o sistema de diálogo imersivo chamando a Instância diretamente
        if (myData.startingNode != null && DialogueManager.Instance != null)
        {
            DialogueManager.Instance.StartDialogue(myData.startingNode, this);
        }
        else if (DialogueManager.Instance == null)
        {
            Debug.LogError("ERRO: O DialogueManager não foi encontrado na cena. Coloque o script em um objeto vazio!");
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