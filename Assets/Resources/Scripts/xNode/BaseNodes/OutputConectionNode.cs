using UnityEngine;
using XNode;

[CreateNodeMenu("")]
public class OutputConectionNode : BaseNode
{
    [Space(15)]
    [Output] public BaseNode baseOutput;
}