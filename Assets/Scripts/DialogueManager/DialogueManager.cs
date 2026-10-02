using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;
public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance; 

    [Header("Referências da UI")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    
    [Header("Sistema de Escolhas")]
    public GameObject choicesContainer; 
    public GameObject choiceButtonPrefab; 

    [Header("Configurações")]
    public float typingSpeed = 0.05f;

    // Referência direta ao NPC que está no balcão agora
    private NpcController currentNpc; 
    private Coroutine typingCoroutine;
    private bool isTyping = false;

    public bool isDialogueActive = false;

    // NOVO: Guarda a referência do nó atual para podermos puxar o texto completo
    private DialogueNode currentNode; 

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        dialoguePanel.SetActive(false);
    }

    // NOVO: Verifica cliques enquanto o texto está sendo digitado
    private void Update()
    {
        // Verifica o clique usando o Novo Input System
        if (isTyping && Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            CompleteTextInstantly();
        }
    }

    public void StartDialogue(DialogueNode startingNode, NpcController npc)
    {
        isDialogueActive = true; 
        
        currentNpc = npc;
        dialoguePanel.SetActive(true);
        
        if (CameraController.Instance != null) CameraController.Instance.ZoomIn();
        
        DisplayNode(startingNode);
    }

    public void DisplayNode(DialogueNode node)
    {
        currentNode = node; // NOVO: Salva o nó atual

        foreach (Transform child in choicesContainer.transform)
        {
            Destroy(child.gameObject);
        }

        nameText.text = node.npcName;
        
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeSentence(node));
    }

    private IEnumerator TypeSentence(DialogueNode node)
    {
        isTyping = true;
        dialogueText.text = "";
        
        foreach (char letter in node.dialogueText.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        ShowChoices(node);
    }

    // NOVO: Função que cancela a digitação lenta e mostra tudo de uma vez
    private void CompleteTextInstantly()
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        
        dialogueText.text = currentNode.dialogueText; // Coloca o texto inteiro
        isTyping = false;
        
        ShowChoices(currentNode); // Mostra os botões imediatamente
    }

    private void ShowChoices(DialogueNode node)
    {
        if (node.choices == null || node.choices.Length == 0)
        {
            if (node.isFinalNode)
                CreateButton("...", null, DialogueAction.LeaveQueue);
            else
                CreateButton("Sair", null, DialogueAction.CloseDialogue);
            return;
        }

        foreach (DialogueChoice choice in node.choices)
        {
            CreateButton(choice.choiceText, choice.nextNode, choice.action);
        }
    }

    private void OnChoiceClicked(DialogueNode nextNode, DialogueAction action)
    {
        if (isTyping) return; 

        switch (action)
        {
            case DialogueAction.AskForID:
                currentNpc.AskForID();
                EndDialogue();
                return;

            case DialogueAction.DismissNPC:
                currentNpc.RefuseAndDismiss();
                EndDialogue(); 
                return;

            case DialogueAction.CloseDialogue:
                EndDialogue();
                return;

            case DialogueAction.LeaveQueue:
                currentNpc.FinalizeAndLeave(); 
                EndDialogue();
                return;
        }

        if (nextNode != null) DisplayNode(nextNode); 
        else EndDialogue(); 
    }

    private void CreateButton(string text, DialogueNode nextNode, DialogueAction action)
    {
        GameObject buttonObj = Instantiate(choiceButtonPrefab, choicesContainer.transform);
        buttonObj.GetComponentInChildren<TextMeshProUGUI>().text = text;
        
        Button btn = buttonObj.GetComponent<Button>();
        btn.onClick.AddListener(() => OnChoiceClicked(nextNode, action));
    }

    private void EndDialogue()
    {
        isDialogueActive = false; 
        
        dialoguePanel.SetActive(false);
        
        if (CameraController.Instance != null) CameraController.Instance.ZoomOut();
    }
}