using UnityEngine;

[RequireComponent(typeof(Collider))] 
public class CycleStarter : MonoBehaviour
{
    [Header("Referência")]
    public CycleManager cycleManager;

    private void OnMouseDown()
    {
        // SE A TRAVA ESTIVER ATIVA, CANCELA A AÇÃO
        if (DialogueManager.Instance != null && DialogueManager.Instance.isDialogueActive) return;

        Debug.Log("DEBUG: Objeto 3D clicado!");
        
        if (cycleManager != null)
        {
            cycleManager.OnStarterObjectClicked();
        }
        else
        {
            Debug.LogError("DEBUG: O CycleManager não foi atribuído no CycleStarter!");
        }
    }
}