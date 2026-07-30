using Resources.Scripts.NPCs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionDialogueController : MonoBehaviour
{
    public Button buttonOption;
    public BaseNode nextNode;
    
    public TextMeshProUGUI optionText;

    private void Start()
    {
        buttonOption.onClick.AddListener(SendOptionDialogue);
    }

    private void OnDestroy()
    {
        buttonOption.onClick.RemoveListener(SendOptionDialogue);
    }

    public void SetOption(BaseNode node, string text)
    {
        nextNode = node;
        optionText.text = text;
    }

    private void SendOptionDialogue()
    {
        DialogueController.instance.FollowDialogueOption(nextNode);
    }
}
