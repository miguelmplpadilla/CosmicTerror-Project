using UnityEngine;

public class DialogueDecisionNode : InputConectionNode
{
    [Space(15)]
    [Output] public BaseNode decision1Output;
    public LocalizableString decision1Text;
    [Space(15)]
    [Output] public BaseNode decision2Output;
    public LocalizableString decision2Text;
    
    [Space(15)]
    [Output] public BaseNode decision3Output;
    public LocalizableString decision3Text;
    [Space(15)]
    [Output] public BaseNode decision4Output;
    public LocalizableString decision4Text;
}