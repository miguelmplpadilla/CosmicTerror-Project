using UnityEngine;
using XNode;

public class ThemeNode : InputConectionNode
{
    public string themeKey;

    [Space(20)]
    [Output] public DialogueNode introductionDialogues;
    [Space(20)]
    [Output] public DialogueNode developmentDialogues;
    [Space(20)]
    [Output] public DialogueNode outcomeDialogues;
    
    public override object GetValue(NodePort port) { return null; }
}