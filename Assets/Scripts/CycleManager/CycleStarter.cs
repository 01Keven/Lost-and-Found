using UnityEngine;

[RequireComponent(typeof(Collider))] // Garante que o objeto terá colisão para receber o clique
public class CycleStarter : MonoBehaviour
{
    [Header("Referência")]
    public CycleManager cycleManager;

    // Detecta o clique com o botão esquerdo do mouse sobre o objeto 3D
    private void OnMouseDown()
    {
        Debug.Log("DEBUG: Objeto 3D de início de ciclo clicado!");
        
        if (cycleManager != null)
        {
            cycleManager.StartCycle();
        }
        else
        {
            Debug.LogError("DEBUG: O CycleManager não foi atribuído no CycleStarter!");
        }
    }
}