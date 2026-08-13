using System;
using System.Linq;
using System.Text;
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
            
            if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) && paperDragManager.isDefaultWritten)
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
                        if (!paperDragManager.isDefaultWritten)
                        {
                            var newInput = new string(input.Normalize(System.Text.NormalizationForm.FormD)
                                .Where(c => System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c) !=
                                            System.Globalization.UnicodeCategory.NonSpacingMark)
                                .ToArray());

                            newInput = RemoveAccents(newInput);

                            var defaultText = RemoveAccents(GameManager.instance
                                .currentDefault[GameManager.instance.currentDefaultIndex]
                                .ToString().ToLower());

                            Debug.Log("Final default text: "+defaultText);
                            
                            if (!newInput.ToLower().Equals(defaultText)) return;
                           
                            input = GameManager.instance.currentDefault[GameManager.instance.currentDefaultIndex]
                                .ToString();
                            GameManager.instance.currentDefaultIndex++;
                        }
                        
                        finalInput = paperDragManager.inputTextPaper.text + input;
                        keyLocked = true;
                        
                        if (finalInput[finalInput.Length - 1] != ' ')
                            paperStamp.SetActive(true);
                    }
                }
            }

            //TODO: Arreglar que no se levanta la tecla cuando se pulsa la primera letra
            if (keyLocked && !Input.anyKey)
            {
                keyLocked = false;
                paperDragManager.inputTextPaper.text = ReleaseKey();
                
                if (GameManager.instance.currentDefaultIndex == GameManager.instance.currentDefault.Length)
                {
                    paperDragManager.inputTextPaper.text += "\n";
                    SetJumpLine();

                    GameManager.instance.currentDefaultIndex = 0;

                    if (GameManager.instance.currentDefault.Equals(GameManager.instance.currentMotv))
                    {
                        paperDragManager.isDefaultWritten = true;
                    }

                    GameManager.instance.currentDefault = GameManager.instance.currentMotv;
                }
            }
        }
        
        public static string RemoveAccents(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            // Normaliza el texto a NFD (descompone acentos)
            string normalizedText = text.Normalize(NormalizationForm.FormD);

            // Filtra los caracteres que no son letras o dígitos (como los acentos)
            StringBuilder finalText = new StringBuilder();
            foreach (char c in normalizedText)
            {
                if (char.GetUnicodeCategory(c) != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    finalText.Append(c);
                }
            }

            return finalText.ToString();
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

        public void SetPaper()
        {
            GameManager.instance.currentDefault = GameManager.instance.currentName;
        }
    }
}