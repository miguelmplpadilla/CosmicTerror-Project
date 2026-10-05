using Resources.Scripts.NPCs.AskNPC;

public class NPC : NPCBase
{
    public DialogueCreator startDialogue;

    protected override void PlayDialogue()
    {
        if (startDialogue != null) 
            StartCoroutine(DialogueController.instance.StartDialogue(startDialogue, this));
    }

    protected override void AskNPC(AskQuestionNPCEvent askQuestion)
    {
        
    }
}