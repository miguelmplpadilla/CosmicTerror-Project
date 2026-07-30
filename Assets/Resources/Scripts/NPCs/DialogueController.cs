using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Resources.Scripts.NPCs;
using UnityEngine;
using XNode;

public class DialogueController : MonoBehaviour
    {
        public static DialogueController instance;

        public DialogueCreator dialogueCreatorTest;

        public GameObject prefabDialogueText;

        public CanvasGroup panelOptionsDialogue;

        public List<SpeakerDialogue> speakersDialogue = new List<SpeakerDialogue>();
        
        public List<TextPanelDialogue>  dialoguesInstantiated = new List<TextPanelDialogue>();

        public NPCBase currentNPCSpeaking;

        private void Awake()
        {
            instance = this;
        }

        public void StartDialogue(DialogueCreator dialogue, NPCBase npcSpeaking)
        {
            currentNPCSpeaking = npcSpeaking;
            StartCoroutine(PlayDialogue(dialogue));
        }

        public IEnumerator PlayDialogue(DialogueCreator dialogueCreator)
        {
            StartDialogueNode startDialogueNode = dialogueCreator.nodes.Find(node => node is StartDialogueNode) as StartDialogueNode;
            NodePort outputPort = startDialogueNode.GetOutputPort(nameof(startDialogueNode.baseOutput));
            BaseNode firstDialogueNode = outputPort?.Connection?.node as BaseNode;

            if (firstDialogueNode == null) yield break;

            currentNPCSpeaking.canvasGroup.DOFade(1, 1);
            yield return new WaitForSeconds(1.2f);

            StartCoroutine(PlayNode(firstDialogueNode));
        }

        private IEnumerator PlayNode(BaseNode node)
        {
            switch (node)
            {
                case DialogueNode dialogueNode:
                    yield return ShowText(dialogueNode);
                    break;
                case AnimationDialogueNode animationDialogueNode:
                    yield return AnimateNpc(animationDialogueNode);
                    break;
                case ObjectDialogueNode objectDialogueNode:
                    //TODO: Create object in scene
                    break;
                case DialogueDecisionNode dialogueDecisionNode:
                    CreateDecisionButtons(dialogueDecisionNode);
                    yield break;
                case ExitNpc exitNpc:
                    yield return ExitNpc();
                    yield break;
            }

            var nextNode = GetOutput(node);

            if (nextNode == null)
            {
                Debug.Log("End Dialogue");
                yield break;
            }
            
            StartCoroutine(PlayNode(nextNode));
        }

        private IEnumerator ShowText(DialogueNode dialogueNode)
        {
            MoveDialogues(dialogueNode.speaker);
            
            SpeakerDialogue speakerDialogue = speakersDialogue.Find(it => it.speaker == dialogueNode.speaker);
            GameObject dialogueInstance = Instantiate(prefabDialogueText, speakerDialogue.parentDialogue.transform);
            dialogueInstance.transform.localPosition = Vector3.zero + new Vector3(0, speakerDialogue.startSumYPosition, 0);
            
            dialoguesInstantiated.Add(dialogueInstance.GetComponent<TextPanelDialogue>());

            dialogueInstance.transform.localScale = Vector3.one * 0.5f;
            var canvasGroup = dialogueInstance.GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0;

            StartCoroutine(dialogueInstance.GetComponent<TextPanelDialogue>().ShowDialogue(dialogueNode));

            yield return new WaitForSeconds(2);
        }
        
        private IEnumerator AnimateNpc(AnimationDialogueNode animationDialogueNode)
        {
            string animatorName = animationDialogueNode.animationType.ToString().ToLower();
            currentNPCSpeaking.animator.Play(animationDialogueNode.animationType.ToString().ToLower(), 0, 0);

            yield return null;

            AnimatorStateInfo info = currentNPCSpeaking.animator.GetCurrentAnimatorStateInfo(0);
            
            while (info.IsName(animatorName) && info.normalizedTime < 1f)
            {
                yield return null;
                info = currentNPCSpeaking.animator.GetCurrentAnimatorStateInfo(0);
            }
            
            currentNPCSpeaking.animator.Play("idle", 0, 0);
        }

        private IEnumerator ExitNpc()
        {
            currentNPCSpeaking.canvasGroup.DOFade(0, 1);
            yield return new WaitForSeconds(1);
            Destroy(currentNPCSpeaking.gameObject);
        }

        public void FollowDialogueOption(BaseNode nextNode)
        {
            panelOptionsDialogue.DOFade(0, 0.5f);
            panelOptionsDialogue.transform.DOScale(0, 0.5f);
            StartCoroutine(PlayNode(nextNode));
        }
        
        private void CreateDecisionButtons(DialogueDecisionNode dialogueDecisionNode)
        {
            MoveDialogues(DialogueNode.Speaker.PLAYER);
            
            panelOptionsDialogue.DOFade(1, 0.5f);
            panelOptionsDialogue.transform.DOScale(1, 0.5f);
            if (panelOptionsDialogue.transform.GetChild(0)
                .TryGetComponent(out OptionDialogueController optionDialogueController1))
            {
                NodePort outputPort =
                    dialogueDecisionNode.GetOutputPort(nameof(dialogueDecisionNode.decision1Output));
                var nextNode = outputPort?.Connection?.node as BaseNode;
                    
                optionDialogueController1.SetOption(nextNode, dialogueDecisionNode.decision1Text.value);
            }
            
            if (panelOptionsDialogue.transform.GetChild(1)
                .TryGetComponent(out OptionDialogueController optionDialogueController2))
            {
                NodePort outputPort =
                    dialogueDecisionNode.GetOutputPort(nameof(dialogueDecisionNode.decision2Output));
                var nextNode = outputPort?.Connection?.node as BaseNode;
                    
                optionDialogueController2.SetOption(nextNode, dialogueDecisionNode.decision2Text.value);
            }
        }

        private void MoveDialogues(DialogueNode.Speaker speaker)
        {
            var dialoguesCleaned =
                dialoguesInstantiated.Where(it => it.dialogue.speaker == speaker).ToList();

            foreach (var dialogueCleaned in dialoguesCleaned)
                dialogueCleaned.transform.DOLocalMoveY(dialogueCleaned.transform.localPosition.y + 20, 0.5f);
            
            if (dialoguesCleaned.Count >= 2)
                dialoguesCleaned[0].Disappear();
        }

        private BaseNode GetOutput(BaseNode node)
        {
            var nodeOutput = node as OutputConectionNode;
            NodePort outputPort = node.GetOutputPort(nameof(nodeOutput.baseOutput));
            return outputPort?.Connection?.node as BaseNode;
        }
    }

    [Serializable]
    public class SpeakerDialogue
    {
        public DialogueNode.Speaker speaker;
        public GameObject parentDialogue;
        public float startSumYPosition = -20;
    }