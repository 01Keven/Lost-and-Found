using UnityEngine;

[CreateAssetMenu(fileName = "NovoNPC", menuName = "Achados e Perdidos/NPC Data")]
public class NpcData : ScriptableObject
{
    public string npcName;
    [TextArea] public string initialDialogue;
    
    [Header("Objetivos & Golpistas")]
    [Tooltip("Marque como TRUE se este NPC não perdeu nada e só quer roubar itens")]
    public bool isScammer; 
    
    [Tooltip("O ID do item correto. Deixe vazio se for um golpista.")]
    public string correctItemID; 
    
    [Header("Documentos")]
    public GameObject idCardPrefab;
}