using DG.Tweening;
using Resources.Scripts.NPCs.AskNPC;
using UnityEngine;
using UnityEngine.UI;

public class AskNPCController : MonoBehaviour
{
    public CanvasGroup panelQuestions;

    public Button[] buttonsQuestions;

    public Button askButton;

    public bool isButtonsShowed = false;

    private void Awake()
    {
        EventBus<RestartButtonsAskEvent>.Register(new EventBinding<RestartButtonsAskEvent>(RestartButtons, gameObject));
    }

    private void OnDestroy()
    {
        EventBus<RestartButtonsAskEvent>.Deregister(new EventBinding<RestartButtonsAskEvent>(RestartButtons, gameObject));
    }

    private void Start()
    {
        askButton.onClick.AddListener(() => ShowHideAllQuestions(true));
        
        foreach (var buttonsQuestion in buttonsQuestions)
            buttonsQuestion.onClick.AddListener(() => AskNpcQuestion(buttonsQuestion));
    }

    private void LateUpdate()
    {
        askButton.transform.localScale =
            GameManager.instance.isNPCShowed && !DialogueController.instance.isPlayingDialogue && !isButtonsShowed
                ? Vector3.one
                : Vector3.zero;
    }

    public void AskNpcQuestion(Button button)
    {
        if (!button.name.ToLower().Equals("close"))
        {
            button.interactable = false;
            EventBus<AskQuestionNPCEvent>.Raise(new AskQuestionNPCEvent { idQuestion = button.name.ToLower() });
        }
        ShowHideAllQuestions(false);
    }
    
    public void ShowHideAllQuestions(bool show)
    {
        isButtonsShowed = show;
        
        panelQuestions.blocksRaycasts = show;
        panelQuestions.DOFade(show ? 1 : 0, 0.3f);
    }

    public void RestartButtons()
    {
        foreach (var buttonsQuestion in buttonsQuestions)
            buttonsQuestion.interactable = true;
    }
}
