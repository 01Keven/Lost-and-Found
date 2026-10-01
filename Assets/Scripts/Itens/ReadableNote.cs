using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ReadableNote : MonoBehaviour
{
    [Tooltip("A imagem 2D em alta resolução que aparecerá na tela")]
    public Sprite noteImage2D;

    private void OnMouseDown()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.OpenNoteUI(noteImage2D);
        }
    }
}