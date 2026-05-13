using System;
using Resources.Scripts.xNode.DialogueCreator.Nodes;
using UnityEngine;

namespace Resources.Scripts.NPCs
{
    public class DialogueController : MonoBehaviour
    {
        public static DialogueController instance;

        private void Awake()
        {
            instance = this;
        }

        public void StartDialogue(DialogueCreator dialogueCreator)
        {
            BaseNode firstDialogueNode =
                (dialogueCreator.nodes.Find(node => node is StartDialogueNode) as StartDialogueNode).baseOutput;

            if (firstDialogueNode == null) return;

            switch (firstDialogueNode)
            {
                case DialogueNode dialogueNode:
                    ShowText(dialogueNode);
                    break;
                case AnimationDialogueNode animationDialogueNode:
                    //TODO: Play animation in NPC
                    break;
                case ObjectDialogueNode objectDialogueNode:
                    //TODO: Create object in scene
                    break;
            }
        }

        private void ShowText(DialogueNode dialogueNode)
        {
            
        }
    }
}