using UnityEngine;

[CreateAssetMenu(fileName = "NovoNPC", menuName = "Achados e Perdidos/NPC Data")]
public class NpcData : ScriptableObject
{
    public string npcName;
    
    [Header("Árvore Inicial")]
    public DialogueNode startingNode; 
    
    [Header("Finais de Diálogo (Devem ter 'Is Final Node' = TRUE)")]
    public DialogueNode correctItemNode;
    public DialogueNode wrongItemNode;
    public DialogueNode dismissedNode;

    [Header("Objetivos & Golpistas")]
    public bool isScammer; 
    public string correctItemID; 
    
    [Header("Mecânica de Roubo (QTE)")]
    public bool willTryToSteal; // TRUE se ele tenta roubar o item ao colocá-lo na mesa
    public DialogueNode stealSuccessNode;
    public DialogueNode stealFailNode;

    [Header("Documentos")]
    public GameObject idCardPrefab;
    [Tooltip("Fala do NPC caso o jogador jogue o documento dele fora")]
    public DialogueNode idDestroyedNode;
}