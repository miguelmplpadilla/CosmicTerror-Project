using UnityEngine;

namespace Resources.Scripts
{
    public class MouseController : MonoBehaviour
    {
        private void Update()
        {
            transform.position = Input.mousePosition;
        }
    }
}