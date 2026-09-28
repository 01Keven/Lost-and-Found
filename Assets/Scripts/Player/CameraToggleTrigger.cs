using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CameraToggleTrigger : MonoBehaviour
{
    private void OnMouseDown()
    {
        if (CameraController.Instance != null)
        {
            CameraController.Instance.ToggleView();
        }
    }
}