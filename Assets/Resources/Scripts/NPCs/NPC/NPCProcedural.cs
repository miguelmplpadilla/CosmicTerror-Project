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
        switch (askQuestion.idQuestion)
        {
            case "askhospital":
                PlayProceduralDialogue(typeDiagnosis == StampController.TypeStamp.HOSPITAL
                    ? DialogueProceduralManager.instance.globalAskNpcHospitalCorrect
                    : DialogueProceduralManager.instance.globalAskNpcHospitalIncorrect);
                break;
            
            case "askjustice":
                PlayProceduralDialogue(typeDiagnosis == StampController.TypeStamp.JUSTICE
                    ? DialogueProceduralManager.instance.globalAskNpcJusticeCorrect
                    : DialogueProceduralManager.instance.globalAskNpcJusticeIncorrect);
                break;
            
            case "askpsyco":
                PlayProceduralDialogue(typeDiagnosis == StampController.TypeStamp.PSYCO
                    ? DialogueProceduralManager.instance.globalAskNpcPsycoCorrect
                    : DialogueProceduralManager.instance.globalAskNpcPsycoIncorrect);
                break;
        }
    }

    private void PlayProceduralDialogue(DialogueProceduralCreator dialogueProcedural)
    {
        var dialogue = DialogueProceduralManager.instance.CreateDialogue();
        if (dialogue == null)
        {
            Debug.LogError("No se ha generado correctamente el dialogo procedural");
            return;
        }
        
        StartCoroutine(DialogueController.instance.StartDialogue(dialogue.dialogueCreator, this));
    }
}
