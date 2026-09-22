using UnityEngine;

[CreateAssetMenu(fileName = "NovoNPC", menuName = "Achados e Perdidos/NPC Data")]
public class NpcData : ScriptableObject
{
    public string npcName;
    [TextArea] public string initialDialogue;
    
    [Header("Objetivos")]
    public string correctItemID; // O ID do item que ele realmente perdeu (ex: "relogio_ouro")
    
    [Header("Documentos")]
    public GameObject idCardPrefab; // O prefab 3D/2D da identidade que vai cair na mesa
}