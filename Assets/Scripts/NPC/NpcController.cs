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
    
    public void ReceiveItem(string givenItemID)
    {
        bool isCorrect = (givenItemID == myData.correctItemID);
        
        RegisterDelivery(myData.npcName, givenItemID, isCorrect);
        
        if (isCorrect)
        {
            Debug.Log($"[{myData.npcName}]: Era isso mesmo, obrigado!");
        }
        else
        {
            Debug.Log($"[{myData.npcName}]: Ah... certo. Vou levar isso. Obrigado.");
        }

        Object.FindAnyObjectByType<CycleManager>().StartCycle();
    }

    private void RegisterDelivery(string npc, string item, bool success)
    {
        Debug.Log($"[LOG DE TURNO] NPC: {npc} | Entregue: {item} | Acertou: {success}");
    }
}