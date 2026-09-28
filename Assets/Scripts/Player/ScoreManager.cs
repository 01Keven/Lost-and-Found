using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("Placar do Turno")]
    public int correctActions = 0;
    public int wrongActions = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Função global para registrar qualquer ação do player
    public void RegisterAction(bool isCorrect, string logDetail)
    {
        if (isCorrect) 
            correctActions++;
        else 
            wrongActions++;

        Debug.Log($"[SCORE] Resultado: {(isCorrect ? "ACERTO" : "ERRO")} | {logDetail}");
        Debug.Log($"[PLACAR ATUAL] Acertos: {correctActions} | Erros: {wrongActions}");
    }
}