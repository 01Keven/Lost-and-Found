using System.Collections.Generic;
using UnityEngine;

public class CycleManager : MonoBehaviour
{
    [System.Serializable]
    public struct CycleData
    {
        public string cycleName; 
        [Tooltip("Arraste os prefabs dos NPCs deste ciclo aqui")]
        public List<GameObject> npcPrefabs; 
    }

    [Header("Configuração dos Ciclos")]
    public List<CycleData> cycles;
    public Transform npcSpawnPoint; 

    private int currentCycleIndex = 0;
    private int currentNpcIndex = 0;
    
    // Controles de estado do fluxo
    private bool isCycleActive = false;
    private bool isWaitingForNextNpc = false; 
    
    private GameObject currentActiveNpc; 

    // Função unificada chamada pelo objeto 3D
    public void OnStarterObjectClicked()
    {
        if (currentCycleIndex >= cycles.Count)
        {
            Debug.Log("DEBUG: Todos os ciclos já foram concluídos. Fim de expediente!");
            return;
        }

        if (!isCycleActive)
        {
            // O ciclo não começou ainda. Inicia o período (ex: Manhã).
            StartCycle();
        }
        else if (isWaitingForNextNpc)
        {
            // O ciclo está rolando e a mesa está livre. Chama o próximo.
            SpawnNextNPC();
        }
        else
        {
            // O jogador clicou no objeto 3D, mas o NPC atual ainda não foi resolvido.
            Debug.Log("DEBUG: Termine de atender o NPC atual antes de chamar o próximo!");
        }
    }

    private void StartCycle()
    {
        isCycleActive = true;
        currentNpcIndex = 0;
        Debug.Log($"DEBUG: === INICIANDO CICLO: {cycles[currentCycleIndex].cycleName.ToUpper()} ===");
        SpawnNextNPC();
    }

    private void SpawnNextNPC()
    {
        isWaitingForNextNpc = false; // A mesa agora está ocupada

        // Segurança para limpar a mesa, caso o NPC anterior ainda exista
        if (currentActiveNpc != null)
        {
            Destroy(currentActiveNpc);
        }

        CycleData currentCycle = cycles[currentCycleIndex];

        // Verifica se ainda há NPCs na fila deste ciclo
        if (currentNpcIndex < currentCycle.npcPrefabs.Count)
        {
            GameObject npcToSpawn = currentCycle.npcPrefabs[currentNpcIndex];
            currentActiveNpc = Instantiate(npcToSpawn, npcSpawnPoint.position, npcSpawnPoint.rotation);
            
            Debug.Log($"DEBUG: [NPC CHEGOU] Atendendo NPC {currentNpcIndex + 1} de {currentCycle.npcPrefabs.Count}.");
            currentNpcIndex++;
        }
        else
        {
            EndCycle();
        }
    }

    // Chamado pelo NpcController quando o player entrega o item ou expulsa o NPC
    // Chamado pelo NpcController quando o player entrega o item ou expulsa o NPC
    public void FinishCurrentNpc()
    {
        Debug.Log("DEBUG: Interação finalizada. O NPC foi embora. Limpando a mesa...");
        
        isWaitingForNextNpc = true; 
        
        // Destrói o NPC visualmente da cena
        if (currentActiveNpc != null)
        {
            Destroy(currentActiveNpc);
            currentActiveNpc = null;
        }

        // AVISA A UI PARA LIMPAR O ESTADO DO BALCÃO
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ClearNpcState();
        }

        // Limpeza da Mesa: Devolve itens esquecidos para a prateleira
        InteractableDeskItem[] allItems = FindObjectsOfType<InteractableDeskItem>();
        foreach (InteractableDeskItem item in allItems)
        {
            if (item.itemType == InteractableDeskItem.ItemType.Object3D && 
                item.currentLocation == InteractableDeskItem.ItemLocation.OnDesk)
            {
                item.ReturnToShelf();
            }
        }
    }

    private void EndCycle()
    {
        isCycleActive = false;
        isWaitingForNextNpc = false;
        
        Debug.Log($"DEBUG: === FIM DO CICLO: {cycles[currentCycleIndex].cycleName.ToUpper()} ===");
        currentCycleIndex++; 
    }
}