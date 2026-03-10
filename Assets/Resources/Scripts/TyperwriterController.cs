using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TyperwriterController : MonoBehaviour
{
    public TMP_InputField inputField;

    private string previousValue = "";

    public RectTransform paperContainerRt;
    public RectTransform paperRT;

    public GameObject paperStamp;

    private int countLetters = 0;
    private int countLines = 0;

    public string currentKeyDown = "";

    private void Start()
    {
        inputField.Select();
        inputField.onValueChanged.AddListener(OnInputValueChanged);
        
        inputField.caretPosition = inputField.text.Length;
        inputField.navigation = new UnityEngine.UI.Navigation
        {
            mode = UnityEngine.UI.Navigation.Mode.None
        };
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) ||
            Input.GetKeyDown(KeyCode.RightArrow) ||
            Input.GetKeyDown(KeyCode.UpArrow) ||
            Input.GetKeyDown(KeyCode.DownArrow) ||
            Input.GetKeyDown(KeyCode.KeypadEnter) ||
            Input.GetKeyDown(KeyCode.Return)
           )
        {
            inputField.caretPosition = inputField.text.Length;
            inputField.selectionAnchorPosition = inputField.caretPosition;
            inputField.selectionFocusPosition = inputField.caretPosition;
        }
        
        foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyUp(key))
            {
                Debug.Log(Input.inputString);
                inputField.interactable = true;
                inputField.Select();

                if (key != KeyCode.Space && key != KeyCode.Return && key != KeyCode.KeypadEnter &&
                    !key.ToString().Contains("Mouse"))
                {
                    paperStamp.SetActive(false);
                }
                
                if (key == KeyCode.Return || key == KeyCode.KeypadEnter || key.ToString().Contains("Mouse")) break;
                
                countLetters++;
                
                paperContainerRt.anchoredPosition += new Vector2(-6.2f, 0);
                if (countLetters == 29)
                {
                    countLines++;
                    paperContainerRt.anchoredPosition = Vector2.zero;
                    paperRT.anchoredPosition = new Vector2(0, countLines * 10);
                    countLetters = 0;
                }
                break;
            }
        }
    }
    
    void LateUpdate()
    {
        if (!inputField.isFocused) return;

        int end = inputField.text.Length;

        inputField.caretPosition = end;
        inputField.selectionAnchorPosition = end;
        inputField.selectionFocusPosition = end;
    }

    private void OnInputValueChanged(string value)
    {
        inputField.onValueChanged.RemoveAllListeners();
        inputField.interactable = false;
        
        if (value[value.Length - 1] == '\n')
        {
            value = value.Substring(0, value.Length - 1);
            inputField.text = value;
        }
        
        if (value.Length < previousValue.Length)
        {
            inputField.text = previousValue;
        }
        else if (value.Length > previousValue.Length)
        {
            if (inputField.text[inputField.text.Length - 1] != ' ')
                paperStamp.SetActive(true);
            
            string processedText = "";
            string currentLine = "";
        
            foreach (char c in value)
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

            // Actualizar el texto del input
            inputField.text = processedText;
            
            previousValue = inputField.text;
        }
        
        inputField.caretPosition = inputField.text.Length;
        inputField.selectionAnchorPosition = inputField.caretPosition;
        inputField.selectionFocusPosition = inputField.caretPosition;

        currentKeyDown = inputField.text[inputField.text.Length - 1].ToString();
        
        inputField.onValueChanged.AddListener(OnInputValueChanged);
    }
}
