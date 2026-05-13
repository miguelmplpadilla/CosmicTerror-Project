namespace Resources.Scripts.xNode.DialogueCreator.Nodes
{
    public class DialogueNode : ConectionsNode
    {
        public Speaker speaker;
        public LocalizableString dialogueText;
        
        public enum Speaker
        {
            PLAYER,
            NPC
        }
    }
}