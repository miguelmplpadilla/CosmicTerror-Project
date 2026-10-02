public class NPC : NPCBase
{
    public DialogueCreator startDialogue;

    protected override void PlayDialogue()
    {
        if (startDialogue != null) 
            DialogueController.instance.StartDialogue(startDialogue, this);
    }
}