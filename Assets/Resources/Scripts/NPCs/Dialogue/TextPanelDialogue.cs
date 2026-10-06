using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Resources.Scripts.NPCs
{
    public class TextPanelDialogue : MonoBehaviour
    {
        public CanvasGroup canvasGroup;
        public TextMeshProUGUI textDialogue;
        [NonSerialized] public DialogueNode dialogue;

        private bool isDisappearing = false;
        
        public IEnumerator ShowDialogue(DialogueNode dialogueNode)
        {
            dialogue = dialogueNode;
            
            textDialogue.text = dialogueNode.dialogueText.value;

            canvasGroup.DOFade(1, 0.5f);
            transform.DOScale(1, 0.5f);
            transform.DOLocalMoveY(0, 0.5f);

            yield return new WaitForSeconds(6);
            
            Disappear();
        }

        public void Disappear()
        {
            if (isDisappearing) return;
            isDisappearing = true;
            canvasGroup.DOFade(0, 0.5f);
        }
    }
}