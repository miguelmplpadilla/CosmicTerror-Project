using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "DocumentData", menuName = "Objects/DocumentData")]
public class DocumentData : ScriptableObject
{
    public bool allPeopleKnowDocument = false;
    
    public DialogueProceduralCreator proceduralDialoguesKnow;

    public GameObject obj;

    public DialogueCreator GetDialogue(NPCBase npc)
    {
        var currentDialogue = allPeopleKnowDocument
            ? DialogueProceduralManager.instance.CreateDialogue(proceduralDialoguesKnow)
            : DialogueProceduralManager.instance.CreateDialogue(DialogueProceduralManager.instance.globalDialoguesNotKnowDocument);

        foreach (var documentKnow in npc.knownDocuments)
            if (documentKnow.document.name.Trim().Replace("(Clone)", "").Equals(obj.name))
                return documentKnow.dialogueDocument;

        return currentDialogue;
    }
}