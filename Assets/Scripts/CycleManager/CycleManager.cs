using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
    public Transform npcSpawnPoint; // Referência para o ponto atrás do balcão

    private int currentCycleIndex = 0;
    private int currentNpcIndex = 0;
    private bool isCycleActive = false;
    
    private GameObject currentActiveNpc; // Guarda o NPC que está na tela agora

    [Header("Input de Teste")]
    public InputActionReference nextNpcAction;

    private void OnEnable()
    {
        if (nextNpcAction != null)
        {
            nextNpcAction.action.Enable();
            nextNpcAction.action.performed += OnNextNpcPerformed;
        }
    }

    private void OnDisable()
    {
        if (nextNpcAction != null)
        {
            nextNpcAction.action.Disable();
            nextNpcAction.action.performed -= OnNextNpcPerformed;
        }
    }

    public void StartCycle()
    {
        if (isCycleActive || currentCycleIndex >= cycles.Count) return;

        isCycleActive = true;
        currentNpcIndex = 0;
        
        Debug.Log($"DEBUG: === INICIANDO CICLO: {cycles[currentCycleIndex].cycleName.ToUpper()} ===");
        SpawnNextNPC();
    }

    private void SpawnNextNPC()
    {
        // 1. Destrói o NPC anterior, se houver alguém no balcão
        if (currentActiveNpc != null)
        {
            Destroy(currentActiveNpc);
        }

        CycleData currentCycle = cycles[currentCycleIndex];

        // 2. Verifica se ainda há NPCs na fila deste ciclo
        if (currentNpcIndex < currentCycle.npcPrefabs.Count)
        {
            // 3. Pega o prefab específico da lista e cria ele no SpawnPoint
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

    private void OnNextNpcPerformed(InputAction.CallbackContext context)
    {
        if (!isCycleActive) return;
        SpawnNextNPC(); // Destrói o atual e chama o próximo
    }

    private void EndCycle()
    {
        isCycleActive = false;
        Debug.Log($"DEBUG: === FIM DO CICLO: {cycles[currentCycleIndex].cycleName.ToUpper()} ===");
        currentCycleIndex++; 
    }
}