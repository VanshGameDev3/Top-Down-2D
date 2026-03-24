using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{
    public NPC_Dialoge dialogueData;
    public GameObject dialoguePanel;
    public TMP_Text dialogueText, nameText;
    public Image potraitImage;
    private AudioSource audioSource;
    public bool IsInDialogue { get; private set; }

    private int dialogueIndex;
    private bool isTyping;
    private bool isDialogueActive;

    public void Interact()
    {
        if (dialogueData == null)
            return;

        if (!isDialogueActive)
            StartDialogue();
        else
            NextLine();
    }

    void StartDialogue()
    {
        isDialogueActive = true;
        IsInDialogue = true;

        dialogueIndex = 0;
        nameText.text = dialogueData.npcName;
        potraitImage.sprite = dialogueData.npcPotrait;

        FacePlayer();

        PlayVoice();

        dialoguePanel.SetActive(true);
        StartCoroutine(TypingLine());
    }

    void FacePlayer()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null) return;

        Vector2 dir = (player.transform.position - transform.position).normalized;

        Animator anim = GetComponent<Animator>();
        if (anim == null) return;

        anim.SetFloat("MoveX", dir.x);
        anim.SetFloat("MoveY", dir.y);
    }

    void PlayVoice()
    {
        if (dialogueData.voiceSound == null)
            return;

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.pitch = dialogueData.voicePitch;
        audioSource.PlayOneShot(dialogueData.voiceSound);
    }
    void NextLine()
    {
        if (isTyping)
        {
            StopAllCoroutines();
            dialogueText.text = dialogueData.dialogueLines[dialogueIndex];
            isTyping = false;
        }
        else if (++dialogueIndex < dialogueData.dialogueLines.Length)
        {
            StartCoroutine(TypingLine());
        }
        else
        {
            EndDialogue();
        }
    }

    IEnumerator TypingLine()
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char c in dialogueData.dialogueLines[dialogueIndex])
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(dialogueData.typingSpeed);
        }

        isTyping = false;
    }

    void EndDialogue()
    {
        StopAllCoroutines();
        isDialogueActive = false;
        IsInDialogue = false;

        dialogueText.text = "";
        dialoguePanel.SetActive(false);

        var interact = GetComponent<NPCInteraction>();
        if (interact != null && interact.interactUI != null)
            interact.interactUI.SetActive(true);
    }
}