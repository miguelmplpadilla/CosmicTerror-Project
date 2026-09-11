using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class NPCBase : InterBaseController
{
    public Animator animator;
    public CanvasGroup canvasGroup;

    public string npcName;
    public LocalizableString motiveText;
    
    public KnownDocument[] knownDocuments;

    protected virtual void Start()
    {
        npcName = GameManager.instance.npcName.namesList[Random.Range(0, GameManager.instance.npcName.namesList.Count)] + " " + 
                  GameManager.instance.npcName.lastNamesList[Random.Range(0, GameManager.instance.npcName.lastNamesList.Count)];
        
        GameManager.instance.currentName = npcName;
        GameManager.instance.currentMotv = motiveText.value; //TODO: Modificar motivos mediante DialogueNode
    }

    public override void Inter(DocumentData documentData)
    {
        if (DialogueController.instance.isPlayingDialogue) return;
        
        DialogueController.instance.StartDialogue(documentData.GetDialogue(this), this, 0);
    }

    public override void ShowIcon(bool show)
    {
        if (show && DialogueController.instance.isPlayingDialogue) return;
        base.ShowIcon(show);
    }
}

[Serializable]
public class KnownDocument
{
    public GameObject document;
    public DialogueCreator dialogueDocument;
}
