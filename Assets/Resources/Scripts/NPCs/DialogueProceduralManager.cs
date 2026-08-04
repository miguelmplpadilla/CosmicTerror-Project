using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XNode;
using Random = UnityEngine.Random;

public class DialogueProceduralManager : MonoBehaviour
{
    public static DialogueProceduralManager instance;
    
    public DialogueProceduralCreator globalDialoguesProcedural;
    public DialogueProceduralCreator globalDialoguesNotKnowDocument;

    [Range(1, 10)]
    public int stressThresholdBase = 1;

    private void Awake()
    {
        instance = this;
    }

    public DialogueCreator CreateDialogue(DialogueProceduralCreator dialogueProcedural = null)
    {
        var currentDialogue = dialogueProcedural == null ? globalDialoguesProcedural :  dialogueProcedural;
        
        List<CategoryNode> categories = new List<CategoryNode>(currentDialogue.nodes
            .Where(it => it is CategoryNode && (it as CategoryNode).stressThreshold <= stressThresholdBase)
            .OfType<CategoryNode>());

        if (categories.Count == 0) return null; 
        
        var selectedCategory = categories[Random.Range(0, categories.Count)];
        
        NodePort themesPort = selectedCategory.GetOutputPort(nameof(selectedCategory.themes));

        if (themesPort == null || !themesPort.IsConnected)
        {
            return null;
        }

        var themesNode = themesPort.GetConnection(0).node as ThemeNode;

        if (themesNode == null) return null;

        List<DialogueNode> introduction = GetRandomDialogues(themesNode, nameof(themesNode.introductionDialogues));
        List<DialogueNode> development = GetRandomDialogues(themesNode, nameof(themesNode.developmentDialogues));
        List<DialogueNode> outcome = GetRandomDialogues(themesNode, nameof(themesNode.outcomeDialogues));
        
        DialogueCreator dialogue = ScriptableObject.CreateInstance<DialogueCreator>();

        var startDialogue = dialogue.AddNode<StartDialogueNode>();

        List<DialogueNode> introductionCopy = new List<DialogueNode>();
        if (introduction != null && introduction.Count > 0) introductionCopy = ConnectDialogues(introduction, startDialogue, dialogue);
        
        List<DialogueNode> developmentCopy = new List<DialogueNode>();
        if (development != null && development.Count > 0) developmentCopy = ConnectDialogues(development, introductionCopy[introductionCopy.Count-1], dialogue);
        
        List<DialogueNode> outcomeCopy = new List<DialogueNode>();
        if (outcome != null && outcome.Count > 0) outcomeCopy = ConnectDialogues(outcome, developmentCopy[developmentCopy.Count-1], dialogue);

        return dialogue;
    }

    private List<DialogueNode> ConnectDialogues(List<DialogueNode> dialogues, OutputConectionNode startConnectNode, DialogueCreator dialogue)
    {
        List<DialogueNode> nodes = new List<DialogueNode>();
        
        var firstNode = dialogue.CopyNode(dialogues[0]) as DialogueNode;
        nodes.Add(firstNode);
        
        startConnectNode.GetOutputPort(nameof(startConnectNode.baseOutput))
            .Connect(firstNode.GetInputPort(nameof(firstNode.baseInput)));

        for (int i = 1; i < dialogues.Count; i++)
        {
            var nodeOutputIntro = nodes[i-1];
            
            var dialogueNodeCopy = dialogue.CopyNode(dialogues[i]) as DialogueNode;
            nodes.Add(dialogueNodeCopy);
            
            nodeOutputIntro.GetOutputPort(nameof(nodeOutputIntro.baseOutput))
                .Connect(dialogueNodeCopy.GetInputPort(nameof(dialogueNodeCopy.baseInput)));
        }

        return nodes;
    }

    private List<DialogueNode> GetRandomDialogues(Node node, string namePort)
    {
        List<DialogueNode> nodes = new List<DialogueNode>();
        
        NodePort port = node.GetOutputPort(namePort);

        if (port == null || !port.IsConnected)
        {
            return null;
        }

        Node randomNode = port.GetConnection(Random.Range(0, port.ConnectionCount)).node;

        while (true)
        {
            while (randomNode is JointNode)
            {
                var jointNodeOutput = randomNode as JointNode;
                randomNode = GetRandomDialogue(randomNode, nameof(jointNodeOutput.baseOutput));
            }
            
            if (randomNode == null) break;
            
            nodes.Add(randomNode as DialogueNode);

            bool followPhrase = Random.Range(0, 2) == 0;

            if (!followPhrase && nodes.Where(it => it.speaker == DialogueNode.Speaker.NPC).ToList().Count >= 1) break;
            
            var randomNodeOutput = randomNode as ConectionsNode;
            randomNode = GetRandomDialogue(randomNode, nameof(randomNodeOutput.baseOutput));

            if (randomNode == null)
                break;
        }

        return nodes;
    }

    public DialogueNode GetRandomDialogue(Node node, string namePort)
    {
        NodePort port = node.GetOutputPort(namePort);

        if (port == null || !port.IsConnected)
        {
            return null;
        }
        
        var randomNode = port.GetConnection(Random.Range(0, port.ConnectionCount)).node;

        if (randomNode is JointNode)
        {
            var randomNodeOutput = (randomNode as ConectionsNode);
            randomNode = GetRandomDialogue(randomNode, nameof(randomNodeOutput.baseOutput));
        }
        
        return randomNode as DialogueNode;
    }
}