using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CycleManager : MonoBehaviour
{
    [System.Serializable]
    public struct CycleData
    {
        public string cycleName;
        public int npcCount;
    }

    public List<CycleData> cycles;
    private int currentCycleIndex = 0;
    private int currentNpcIndex = 0;
    private bool isCycleActive = false;

    public InputActionReference nextNpcAction;

    private void Onable()
    {
        if (nextNpcAction != null)
        {
            nextNpcAction.action.Disable();
            nextNpcAction.action.performed -= OnNextNpcPerfomed;
        }     
    }

    public void StartCycle()
    {
        if (isActiveAndEnabled)
        {
            Debug.Log("Fila começando");
            return;
        }

        if (currentCycleIndex >= cycles.Count)
        {
            Debug.Log("todos os ciclos encerraram");
            return;
        }

        isCycleActive = true;
        currentNpcIndex = 0;
        CycleData currentCycle = cycles[currentCycleIndex];

        Debug.Log($"DEBUG: === INICIANDO CICLO: {currentCycle.cycleName.ToUpper()} ===");
        Debug.Log($"DEBUG: Fila criada com {currentCycle.npcCount} NPCs.");

        SpawnNextNPC();

    }

    private void SpawnNextNPC()
    {
        CycleData currentCycle = cycles[currentCycleIndex];

        if (currentCycleIndex < currentCycle.npcCount)
        {
            currentCycleIndex++;
            Debug.Log($"DEBUG: [NPC CHEGOU] Atendendo NPC {currentNpcIndex} de {currentCycle.npcCount} (Ciclo: {currentCycle.cycleName}).");
        }
        else
        {
            EndCycle();
        }


    }

    private void OnNextNpcPerfomed(InputAction.CallbackContext context)
    {
        if (!isCycleActive)
        {
            Debug.Log("DEBUG: Input 'nextNpc' pressionado. Dispensando NPC atual...");
            SpawnNextNPC();
        }
    }

    private void EndCycle()
    {
        isCycleActive = false;

        currentCycleIndex++;

        if (currentCycleIndex >= cycles.Count)
        {
            Debug.Log("DEBUG: O shopping fechou. Fim do dia!");
        }
        else
        {
            Debug.Log($"DEBUG: Clique no objeto 3D para iniciar o próximo ciclo ({cycles[currentCycleIndex].cycleName}).");
        }
    }
}
