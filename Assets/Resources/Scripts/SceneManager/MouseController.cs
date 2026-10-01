using System;
using UnityEngine;

namespace Resources.Scripts
{
    public class MouseController : MonoBehaviour
    {
        public static MouseController instance;
    
        public RectTransform iconsRt;
        public GameObject askIcon;
        public GameObject addIcon;
        public GameObject removeIcon;
        
        private void Awake()
        {
            instance = this;
        }
        
        private void Update()
        {
            transform.position = Input.mousePosition;
        }

        private void LateUpdate()
        {
            if (GameManager.instance.currentDraggingObject != null)
            {
                var rtDraggingObject = GameManager.instance.currentDraggingObject.currentObjectSize
                    .GetComponent<RectTransform>();
                iconsRt.sizeDelta = rtDraggingObject.sizeDelta *
                                    GameManager.instance.currentDraggingObject.transform.localScale;
            }
        }

        public void ShowAskIcon(bool show)
        {
            askIcon.SetActive(show);
        }
        
        public void ShowAddIcon(bool show)
        {
            addIcon.SetActive(show);
        }
        
        public void ShowRemoveIcon(bool show)
        {
            removeIcon.SetActive(show);
        }
    }
}