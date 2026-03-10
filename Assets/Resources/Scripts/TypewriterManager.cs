using System;
using TMPro;
using UnityEngine;

namespace Resources.Scripts
{
    public class TypewriterManager : MonoBehaviour
    {
        public static TypewriterManager instance;
        
        public TextMeshProUGUI inputField;
        
        private int countLetters = 0;
        public int countLines = 0;
        
        public RectTransform paperContainerRt;
        public RectTransform paperRT;

        public GameObject paperStamp;

        public GameObject centerPaper;

        private bool keyLocked = false;

        private string finalInput = "";

        private void Awake()
        {
            instance = this;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                inputField.text += "\n";
                SetJumpLine();
            }
            
            if (!keyLocked && Input.anyKeyDown)
            {
                string input = Input.inputString;

                if (!string.IsNullOrEmpty(input))
                {
                    if (input.Length == 1 && !char.IsControl(input[0]))
                    {
                        finalInput = inputField.text + input;
                        keyLocked = true;
                        
                        if (finalInput[finalInput.Length - 1] != ' ')
                            paperStamp.SetActive(true);
                    }
                }
            }

            if (keyLocked && !Input.anyKey)
            {
                keyLocked = false;
                inputField.text = ReleaseKey();
            }
        }

        private string ReleaseKey()
        {
            paperStamp.SetActive(false);
            
            string processedText = "";
            string currentLine = "";
        
            foreach (char c in finalInput)
            {
                currentLine += c;

                if (c == '\n')
                {
                    processedText += currentLine;
                    currentLine = "";
                    continue;
                }

                // Medir ancho de la línea
                if (currentLine.Length > 29)
                {
                    // Si la línea excede, mover el último carácter a la siguiente línea
                    currentLine = currentLine.Substring(0, currentLine.Length - 1);
                    processedText += currentLine + "\n" + c;
                    currentLine = "";
                }
            }
            processedText += currentLine;
            
            countLetters++;
                
            paperContainerRt.anchoredPosition += new Vector2(-4.67f, 0);
            if (countLetters == 29)
            {
                SetJumpLine();
            }

            return processedText;
        }

        private void SetJumpLine()
        {
            countLines++;
            paperContainerRt.anchoredPosition = Vector2.zero;
            paperRT.anchoredPosition = new Vector2(0, countLines * 7.6f);
            countLetters = 0;
        }
    }
}