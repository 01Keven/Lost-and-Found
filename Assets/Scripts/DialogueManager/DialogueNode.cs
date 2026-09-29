using UnityEngine;

// Ações específicas para o jogo de Achados e Perdidos
public enum DialogueAction
{
    Nothing,
    AskForID,       // Faz o NPC jogar a identidade na mesa
    DismissNPC,     // Expulsa o NPC (chama o RefuseAndDismiss)
    CloseDialogue   // Apenas fecha a caixa de texto para o player olhar a mesa
}

[CreateAssetMenu(fileName = "New Dialog", menuName = "Achados e Perdidos/Novo Node de Dialogo")]
public class DialogueNode : ScriptableObject
{
    [Header("Informações da Fala")]
    public string npcName;
    
    [TextArea(3, 5)]
    public string dialogueText; 

    [Header("Opções de Resposta")]
    [Tooltip("Deixe vazio para aparecer apenas um botão de 'Avançar'")]
    public DialogueChoice[] choices;
}

[System.Serializable]
public class DialogueChoice
{
    public string choiceText; 
    public DialogueNode nextNode; 
    public DialogueAction action = DialogueAction.Nothing;
}