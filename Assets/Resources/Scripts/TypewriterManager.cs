using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Resources.Scripts
{
    public class TypewriterManager : MonoBehaviour
    {
        public static TypewriterManager instance;
        
        public PaperDragManager paperDragManager;

        public int maxLettersPerLine = 40;
        public float horizontalSum = -5;
        public float verticalSum = 8;
        
        public RectTransform paperContainerRt;
        public RectTransform paperRT;

        public GameObject paperStamp;

        public GameObject centerPaper;

        private bool keyLocked = false;

        private string finalInput = "";

        public RectTransform maskPaperRt;

        private void Awake()
        {
            instance = this;
        }

        private void Update()
        {
            if (paperDragManager == null) return;
            
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                paperDragManager.inputTextPaper.text += "\n";
                SetJumpLine();
            }
            
            if (!keyLocked && Input.anyKeyDown)
            {
                string input = Input.inputString;

                if (!string.IsNullOrEmpty(input))
                {
                    if (input.Length == 1 && !char.IsControl(input[0]))
                    {
                        finalInput = paperDragManager.inputTextPaper.text + input;
                        keyLocked = true;
                        
                        if (finalInput[finalInput.Length - 1] != ' ')
                            paperStamp.SetActive(true);
                    }
                }
            }

            if (keyLocked && !Input.anyKey)
            {
                keyLocked = false;
                paperDragManager.inputTextPaper.text = ReleaseKey();
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
                if (currentLine.Length > maxLettersPerLine)
                {
                    // Si la línea excede, mover el último carácter a la siguiente línea
                    currentLine = currentLine.Substring(0, currentLine.Length - 1);
                    processedText += currentLine + "\n" + c;
                    currentLine = "";
                }
            }
            processedText += currentLine;
            
            paperDragManager.countLetters++;
                
            CalculateHorizontalPosition();
            if (paperDragManager.countLetters == maxLettersPerLine)
            {
                SetJumpLine();
            }

            return processedText;
        }

        private void SetJumpLine()
        {
            paperDragManager.countLines++;
            CalculateVerticalPosition();
            paperDragManager.countLetters = 0;
        }

        public void CalculateVerticalPosition()
        {
            paperContainerRt.anchoredPosition = Vector2.zero;
            paperRT.anchoredPosition = new Vector2(0, paperDragManager.countLines * verticalSum);
        }
        
        public void CalculateHorizontalPosition()
        {
            paperContainerRt.anchoredPosition = new Vector2(horizontalSum * paperDragManager.countLetters, 0);
        }

        public void ExtendMaskPaper(bool extend, float timeAnimation = 0.6f)
        {
            maskPaperRt.DOKill();
            maskPaperRt.DOSizeDelta(new Vector2(maskPaperRt.rect.size.x, extend ? 400 : 125), timeAnimation);
        }
    }
}