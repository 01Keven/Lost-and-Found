using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TypingQTEManager : MonoBehaviour
{
    public static TypingQTEManager Instance;

    [Header("Referências da UI")]
    public GameObject qtePanel;
    public TextMeshProUGUI targetPhraseText;
    public TMP_InputField inputField;
    public Image timerBar;
    
    [Header("Configurações")]
    public float timeLimit = 3.5f;
    private string[] phrases = { "SOLTE ISSO", "SAIA DAQUI", "LADRAO", "SEGURANCA", "DEIXE NA MESA" };

    private string currentTarget;
    private float timer;
    private bool isActive;
    
    private NpcController thiefNpc;
    private InteractableDeskItem stolenItem;

    private void Awake() 
    { 
        if (Instance == null) Instance = this; 
        qtePanel.SetActive(false); 
    }

    public void StartQTE(NpcController npc, InteractableDeskItem item)
    {
        thiefNpc = npc;
        stolenItem = item;
        
        currentTarget = phrases[Random.Range(0, phrases.Length)];
        targetPhraseText.text = currentTarget;
        inputField.text = "";
        timer = timeLimit;
        isActive = true;

        qtePanel.SetActive(true);
        
        // Força o foco automático para o jogador só precisar digitar
        EventSystem.current.SetSelectedGameObject(inputField.gameObject);
        inputField.ActivateInputField();
    }

   private void Update()
    {
        if (!isActive) return;

        timer -= Time.deltaTime;
        if (timerBar != null) timerBar.fillAmount = timer / timeLimit;

        // Puxão físico: Move o item na direção exata do NPC ladrão
        if (stolenItem != null && thiefNpc != null)
        {
            Vector3 directionToNpc = (thiefNpc.transform.position - stolenItem.transform.position).normalized;
            stolenItem.transform.position += directionToNpc * (Time.deltaTime * 0.5f);
        }

        // Verifica condição de vitória (ignorando maiúsculas/minúsculas e espaços no fim)
        if (inputField.text.Trim().ToUpper() == currentTarget.ToUpper())
        {
            EndQTE(true);
        }
        else if (timer <= 0) // Falha por tempo
        {
            EndQTE(false);
        }
    }
    private void EndQTE(bool success)
    {
        isActive = false;
        qtePanel.SetActive(false);
        thiefNpc.ResolveSteal(success, stolenItem);
    }
}