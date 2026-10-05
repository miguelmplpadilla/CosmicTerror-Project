using Resources.Scripts.NPCs.AskNPC;
using UnityEngine;

public class NPCProcedural : NPCBase
{
    protected override void PlayDialogue()
    {
        var dialogue = DialogueProceduralManager.instance.CreateDialogue();
        if (dialogue == null)
        {
            Debug.LogError("No se ha generado correctamente el dialogo procedural");
            return;
        }

        motiveText = dialogue.motv;
        typeDiagnosis = dialogue.typeDiagnosis;
        
        StartCoroutine(DialogueController.instance.StartDialogue(dialogue.dialogueCreator, this));
    }
    
    protected override void AskNPC(AskQuestionNPCEvent askQuestion)
    {
        
    }
}
