using Resources.Scripts;
using UnityEngine;

public class PaperFile : FileDragController
{
    public string nameText;
    public string motvText;
    
    public override void SetData(DragBaseManager dragBaseManager)
    {
        var document = dragBaseManager as PaperDragManager;

        nameText = document.nameText;
        motvText = document.motvText;
    }
}