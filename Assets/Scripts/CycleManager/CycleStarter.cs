using UnityEngine;

[RequireComponent(typeof(Collider))] 
public class CycleStarter : MonoBehaviour
{
    [Header("Referência")]
    public CycleManager cycleManager;

    private void OnMouseDown()
    {
        Debug.Log("DEBUG: Objeto 3D clicado!");
        
        if (cycleManager != null)
        {
            cycleManager.OnStarterObjectClicked(); // Chama a função lógica de estados
        }
        else
        {
            Debug.LogError("DEBUG: O CycleManager não foi atribuído no CycleStarter!");
        }
    }
}