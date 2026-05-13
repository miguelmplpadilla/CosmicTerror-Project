namespace Resources.Scripts.xNode.DialogueCreator.Nodes
{
    public class AnimationDialogueNode : ConectionsNode
    {
        public AnimationType animationType;
        
        public enum AnimationType
        {
            IDLE,
            HAPPY,
            SAD,
            ANGRY
        }
    }
}