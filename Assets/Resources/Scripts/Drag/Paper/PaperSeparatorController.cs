using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Resources.Scripts
{
    public class PaperSeparatorController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public PaperDragManager paperDragManager;
        private PaperDragManager _paperDragManagerClone;
        
        public void OnBeginDrag(PointerEventData eventData)
        {
            GameObject paperClone = Instantiate(paperDragManager.gameObject, GameManager.instance.panelGlobalObjects.transform);
            paperClone.transform.position = paperDragManager.transform.position;
            _paperDragManagerClone = paperClone.GetComponent<PaperDragManager>();
            
            foreach (var imageBackground in _paperDragManagerClone.imagesBackground)
                imageBackground.color = Color.white;
            
            _paperDragManagerClone.canWrite = false;
            
            _paperDragManagerClone.OnBeginDrag(eventData);
            paperDragManager.paperSeparated = true;
        }

        public void OnDrag(PointerEventData eventData)
        {
            _paperDragManagerClone.OnDrag(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _paperDragManagerClone.OnEndDrag(eventData);
        }
    }
}