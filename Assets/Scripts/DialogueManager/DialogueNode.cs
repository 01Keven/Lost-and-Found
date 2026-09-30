using UnityEngine;

// Ações específicas para o jogo de Achados e Perdidos
public enum DialogueAction
{
    Nothing,
    AskForID,       
    DismissNPC,     
    CloseDialogue,  
    LeaveQueue      // NOVO: Faz o NPC ir embora e chama o próximo da fila
}

[CreateAssetMenu(fileName = "New Dialog", menuName = "Achados e Perdidos/Novo Node de Dialogo")]
public class DialogueNode : ScriptableObject
{
    [Header("Informações da Fala")]
    public string npcName;
    
    [TextArea(3, 5)]
    public string dialogueText; 

    [Header("Configuração de Saída")]
    [Tooltip("Marque TRUE se esta for a fala final antes do NPC ir embora (não precisa criar 'Choices' abaixo)")]
    public bool isFinalNode = false;

    [Header("Opções de Resposta")]
    public DialogueChoice[] choices;
}

[System.Serializable]
public class DialogueChoice
{
    public string choiceText; 
    public DialogueNode nextNode; 
    public DialogueAction action = DialogueAction.Nothing;
}