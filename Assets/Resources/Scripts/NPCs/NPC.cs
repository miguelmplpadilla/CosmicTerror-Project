public class NPC : NPCBase
{
    public DialogueCreator startDialogue;

    protected override void Start()
    {
        base.Start();
        
        if (startDialogue != null) 
            DialogueController.instance.StartDialogue(startDialogue, this);
    }
}