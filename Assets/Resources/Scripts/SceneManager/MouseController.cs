using UnityEngine;

namespace Resources.Scripts
{
    public class MouseController : MonoBehaviour
    {
        public static MouseController instance;
    
        public GameObject askIcon;

        private void Awake()
        {
            instance = this;
        }
        
        private void Update()
        {
            transform.position = Input.mousePosition;
        }

        public void ShowAskIcon(bool show)
        {
            askIcon.SetActive(show);
        }
    }
}