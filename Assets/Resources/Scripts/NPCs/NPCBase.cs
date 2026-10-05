using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Resources.Scripts;
using Resources.Scripts.CorkBoard;
using Resources.Scripts.NPCs.AskNPC;
using UnityEngine;
using Random = UnityEngine.Random;

public class NPCBase : InterBaseController
{
    public GameObject parent;
    
    public Animator animator;
    public CanvasGroup canvasGroup;

    public string npcName;
    public LocalizableString motiveText;

    public bool isDocumentHandled = false;
    
    public enum Mood
    {
        NORMAL, MAD, HAPPY
    }
    
    public Mood npcMood = Mood.NORMAL;
    public StampController.TypeStamp typeDiagnosis;
    
    public KnownDocument[] knownDocuments;

    private void Awake()
    {
        EventBus<AskQuestionNPCEvent>.Register(new EventBinding<AskQuestionNPCEvent>(AskNPC, gameObject));
    }

    private void OnDestroy()
    {
        EventBus<AskQuestionNPCEvent>.Deregister(new EventBinding<AskQuestionNPCEvent>(AskNPC, gameObject));
    }

    protected virtual void Start()
    {
        npcName = GameManager.instance.npcName.namesList[Random.Range(0, GameManager.instance.npcName.namesList.Count)] + " " + 
                  GameManager.instance.npcName.lastNamesList[Random.Range(0, GameManager.instance.npcName.lastNamesList.Count)];
        
        EventBus<ShowHideButtonCorkBoard>.Raise(new ShowHideButtonCorkBoard { show = false });
        
        PlayDialogue();
        
        GameManager.instance.currentName = npcName;
        GameManager.instance.currentMotv = motiveText.value;

        canvasGroup.alpha = 0;
    }

    public override void Inter(DocumentBaseController documentBaseController)
    {
        if (DialogueController.instance.isPlayingDialogue) return;

        var dialogue = documentBaseController.documentData.GetDialogue(this);

        if (documentBaseController is PaperDragManager paperDragManager)
        {
            List<DialogueCreator> dialogueRandom = new List<DialogueCreator>();
            
            if (paperDragManager.paperSeparated)
            {
                if (typeDiagnosis == paperDragManager.typeStamp)
                {
                    dialogueRandom =
                        new List<DialogueCreator>(DialogueController.instance.dialogueDiagnosisPaperNormal);
                }
            
                if (typeDiagnosis != paperDragManager.typeStamp && paperDragManager.typeStamp != StampController.TypeStamp.NONE)
                {
                    List<DialogueCreator> dialogueWrong = new List<DialogueCreator>(DialogueController.instance.dialogueDiagnosisPaperWrong);
                    if (npcMood == Mood.MAD)
                        dialogueWrong = new List<DialogueCreator>(DialogueController.instance.dialogueDiagnosisPaperMad);

                    dialogueRandom = new List<DialogueCreator>(dialogueWrong);
                }

                isDocumentHandled = (typeDiagnosis == paperDragManager.typeStamp ||
                                     typeDiagnosis != paperDragManager.typeStamp) && paperDragManager.typeStamp !=
                                        StampController.TypeStamp.NONE;
                
                if (paperDragManager.typeStamp == StampController.TypeStamp.NONE)
                    dialogueRandom = new List<DialogueCreator>(DialogueController.instance.dialogueDiagnosisPaperNoStamp);
            }
            else
            {
                dialogueRandom = new List<DialogueCreator>(DialogueController.instance.dialogueDiagnosisPaperNoSeparated);
            }
            
            dialogue = dialogueRandom[
                Random.Range(0, dialogueRandom.Count)];
            
            if (paperDragManager.paperSeparated) Destroy(paperDragManager.gameObject);
        }

        StartCoroutine(PlayDialogue(dialogue));
    }

    private IEnumerator PlayDialogue(DialogueCreator dialogue)
    {
        yield return DialogueController.instance.StartDialogue(dialogue, this);
        if (isDocumentHandled) StartCoroutine(HideNPC());
    }

    public override void ShowIcon(bool show)
    {
        if (show && DialogueController.instance.isPlayingDialogue) return;
        base.ShowIcon(show);
    }

    protected virtual void PlayDialogue()
    {
        
    }

    protected virtual void AskNPC(AskQuestionNPCEvent askQuestion)
    {
        
    }

    public IEnumerator HideNPC()
    {
        GameManager.instance.isNPCShowed = false;
        yield return HideNPCAnimation();
    }

    public virtual IEnumerator HideNPCAnimation()
    {
        canvasGroup.DOFade(0, 1);
        yield return new WaitForSeconds(1);
            
        EventBus<ShowHideButtonCorkBoard>.Raise(new ShowHideButtonCorkBoard { show = true });
            
        Destroy(parent);
    }
    
    public IEnumerator ShowNPC()
    {
        yield return ShowNPCAnimation();
        GameManager.instance.isNPCShowed = true;
    }
    
    public virtual IEnumerator ShowNPCAnimation()
    {
        canvasGroup.DOFade(1, 1);
        yield return new WaitForSeconds(1);
    }

    public override bool CanInteractWith(DragBaseManager objInter)
    {
        if (isDocumentHandled) return false;
        
        return base.CanInteractWith(objInter);
    }
}

[Serializable]
public class KnownDocument
{
    public GameObject document;
    public DialogueCreator dialogueDocument;
}
