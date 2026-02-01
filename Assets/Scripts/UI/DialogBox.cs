using TMPro;
using UC;
using UnityEngine;
using UnityEngine.UI;

public class DialogBox : MonoBehaviour
{
    public delegate bool ButtonFunction(DialogBox dialog);

    [SerializeField]
    private TextMeshProUGUI textElement;
    [SerializeField]
    private Button          yesButton;
    [SerializeField]
    private Button          noButton;
    [SerializeField]
    private Image           blockerElement;

    ButtonFunction  yesAction;
    ButtonFunction  noAction;
    ButtonFunction  yesCondition;
    ButtonFunction  noCondition;
    TextMeshProUGUI yesTextElement;
    TextMeshProUGUI noTextElement;

    void Start()
    {
    }

    private void Update()
    {
        if (yesCondition != null)
        {
            yesButton.interactable = yesCondition(this);
        }
        if (noCondition != null)
        {
            noButton.interactable = noCondition(this);
        }
    }

    public void OnYes()
    {
        if (yesAction == null) return;

        if (yesAction(this))
            HideAndDestroy();
    }

    public void OnNo()
    {
        if (noAction == null) return;

        if (noAction(this))
            HideAndDestroy();
    }

    void HideAndDestroy()
    {
        var buttons = GetComponentsInChildren<Button>();
        foreach (var button in buttons) button.enabled = false;

        CanvasGroup cg = GetComponent<CanvasGroup>();
        if (cg)
        {
            cg.alpha = 1.0f;
            cg.FadeOut(0.25f).Done(() => Destroy(gameObject));
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Init(string textString, bool isModal, ButtonFunction yesAction, ButtonFunction noAction, string yesText, string noText, ButtonFunction yesCondition, ButtonFunction noCondition)
    {
        blockerElement.gameObject.SetActive(isModal);
        textElement.text = textString;

        this.yesAction = yesAction;
        this.noAction = noAction;
        this.yesCondition = yesCondition;
        this.noCondition = noCondition;

        yesTextElement = yesButton.GetComponentInChildren<TextMeshProUGUI>();
        if (yesTextElement) yesTextElement.text = yesText;
        if (string.IsNullOrEmpty(yesTextElement.text)) yesButton.gameObject.SetActive(false);
        
        noTextElement = noButton.GetComponentInChildren<TextMeshProUGUI>();
        if (noTextElement) noTextElement.text = noText;
        if (string.IsNullOrEmpty(noTextElement.text)) noButton.gameObject.SetActive(false);
    }

    public static DialogBox CreateBox(string textString, bool isModal, DialogBox dialogPrefab, RectTransform container, 
                                      ButtonFunction yesAction, ButtonFunction noAction,
                                      string yesText = "YES", string noText = "NO", 
                                      ButtonFunction yesCondition = null, ButtonFunction noCondition = null)
    {
        var dialogBox = Instantiate(dialogPrefab, container);
        dialogBox.Init(textString, isModal, yesAction, noAction, yesText, noText, yesCondition, noCondition);        

        var rt = dialogBox.transform as RectTransform;
        LayoutRebuilder.ForceRebuildLayoutImmediate(rt);

        CanvasGroup cg = dialogBox.GetComponent<CanvasGroup>();
        if (cg)
        {
            cg.alpha = 0.0f;
            cg.FadeIn(0.25f);
        }

        return dialogBox;
    }
}
