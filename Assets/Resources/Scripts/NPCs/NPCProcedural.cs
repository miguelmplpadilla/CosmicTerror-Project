using UnityEngine;

public class NPCProcedural : NPCBase
{
    protected override void Start()
    {
        base.Start();

        var dialogue = DialogueProceduralManager.instance.CreateDialogue();
        if (dialogue == null)
        {
            Debug.LogError("No se ha generado correctamente el dialogo procedural");
            return;
        }

        motiveText = dialogue.motv;
        
        DialogueController.instance.StartDialogue(dialogue.dialogueCreator, this);
    }
}
