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
        
        DialogueController.instance.StartDialogue(dialogue, this);
    }
}
