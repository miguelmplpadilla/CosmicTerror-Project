using System;
using TMPro;
using UnityEngine;

namespace Resources.Scripts.Tools
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class LocalizableController : MonoBehaviour
    {
        [NonSerialized] public TextMeshProUGUI text;
        public LocalizableString localizableString;

        private void Awake()
        {
            text = GetComponent<TextMeshProUGUI>();
        }

        private void Update()
        {
            if (localizableString != null) 
                text.text = localizableString.value;
        }

        public void SetText(LocalizableString localizable)
        {
            localizableString = localizable;
        }
    }
}