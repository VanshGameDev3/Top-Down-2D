using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    private NPC npc;
    private bool playerInRange;

    [Header("UI")]
    public GameObject interactUI; 

    private void Awake()
    {
        npc = GetComponent<NPC>();

        if (interactUI != null)
            interactUI.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            if (interactUI != null && !npc.IsInDialogue)
                interactUI.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

            if (interactUI != null)
                interactUI.SetActive(false);
        }
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            npc.Interact();

            if (interactUI != null)
                interactUI.SetActive(false);
        }
    }
}