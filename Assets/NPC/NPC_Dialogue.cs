using UnityEngine;

[CreateAssetMenu(fileName ="NewNPCDialogue", menuName = "NPC Dialogue")]
public class NPC_Dialoge : ScriptableObject
{
    public string npcName;
    public Sprite npcPotrait;
    public string[] dialogueLines;
    public float typingSpeed = .05f;
    public AudioClip voiceSound;
    public float voicePitch = 1f;
    public bool[] autoProgressLines;
    public float autoProgressDelay = 1.5f;
}