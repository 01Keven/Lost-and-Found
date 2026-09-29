using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

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

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        dialoguePanel.SetActive(false);
    }

    // Agora recebe o NpcController para saber quem está executando as ações
    public void StartDialogue(DialogueNode startingNode, NpcController npc)
    {
        currentNpc = npc;
        dialoguePanel.SetActive(true);
        DisplayNode(startingNode);
    }

    public void DisplayNode(DialogueNode node)
    {
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

    private void ShowChoices(DialogueNode node)
    {
        if (node.choices == null || node.choices.Length == 0)
        {
            // Cria um botão padrão para fechar caso o node não tenha opções configuradas
            CreateButton("Sair", null, DialogueAction.CloseDialogue);
            return;
        }

        foreach (DialogueChoice choice in node.choices)
        {
            CreateButton(choice.choiceText, choice.nextNode, choice.action);
        }
    }

    private void CreateButton(string text, DialogueNode nextNode, DialogueAction action)
    {
        GameObject buttonObj = Instantiate(choiceButtonPrefab, choicesContainer.transform);
        buttonObj.GetComponentInChildren<TextMeshProUGUI>().text = text;
        
        Button btn = buttonObj.GetComponent<Button>();
        btn.onClick.AddListener(() => OnChoiceClicked(nextNode, action));
    }

    private void OnChoiceClicked(DialogueNode nextNode, DialogueAction action)
    {
        if (isTyping)
        {
            // Opcional: Se clicar enquanto digita, preenche o texto todo de uma vez
            return; 
        }

        // Executa a ação da escolha baseada no Enum
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
        }

        // Se a ação for "Nothing", apenas avança para o próximo node
        if (nextNode != null)
        {
            DisplayNode(nextNode); 
        }
        else
        {
            EndDialogue(); 
        }
    }

    private void EndDialogue()
    {
        dialoguePanel.SetActive(false);
    }
}