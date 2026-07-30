public class DialogueNode : ConectionsNode
{
    public Speaker speaker = Speaker.NPC;
    public LocalizableString dialogueText;
        
    public enum Speaker
    {
        PLAYER,
        NPC
    }
}