using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CameraToggleTrigger : MonoBehaviour
{
    private void OnMouseDown()
    {
        // SE A TRAVA ESTIVER ATIVA, CANCELA A AÇÃO
        if (DialogueManager.Instance != null && DialogueManager.Instance.isDialogueActive) return;

        if (CameraController.Instance != null)
        {
            CameraController.Instance.ToggleView();
        }
    }
}