using UnityEngine.UI;

public class PhotoFile : FileDragController
{
    public Image imagePhoto;
    public LocalizableString title;
    
    public override void SetData(DragBaseManager dragBaseManager)
    {
        base.SetData(dragBaseManager);
        
        var photoController = dragBaseManager as PhotoController;
        imagePhoto.sprite = photoController.imagePhoto;
        title = photoController.titlePhoto;
    }
}