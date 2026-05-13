using UnityEngine;
using XNode;

[CreateNodeMenu("")]
public class ConectionsNode : OutputConectionNode
{
    [Space(15)]
    [Input] public BaseNode baseInput;
}