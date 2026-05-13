using UnityEngine;
using XNode;

[CreateNodeMenu("")]
public class InputConectionNode : BaseNode
{
    [Space(15)]
    [Input] public BaseNode baseInput;
}