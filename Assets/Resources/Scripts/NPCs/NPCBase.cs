using UnityEngine;

public class NPCBase : MonoBehaviour
{
    public Animator animator;
    public CanvasGroup canvasGroup;

    public string npcName;
    public LocalizableString motiveText;

    protected virtual void Awake()
    {
        canvasGroup.alpha = 0;
    }

    protected virtual void Start()
    {
        npcName = GameManager.instance.npcName.namesList[Random.Range(0, GameManager.instance.npcName.namesList.Count)] + " " + 
                  GameManager.instance.npcName.lastNamesList[Random.Range(0, GameManager.instance.npcName.lastNamesList.Count)];
        
        GameManager.instance.currentName = npcName;
        GameManager.instance.currentMotv = motiveText.value; //TODO: Modificar motivos mediante DialogueNode
    }
}
