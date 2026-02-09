using TMPro;
using UC;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ActionButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] 
    private Image border;
    [SerializeField]
    private Image icon;
    [SerializeField]
    private TextMeshProUGUI text;
    [SerializeField]
    private Image filler;
    [SerializeField]
    private Color colorNormal = Color.white;
    [SerializeField]
    private Color colorHighlight = Color.yellow;
    [SerializeField]
    private Color colorClick = Color.red;

    private ActionFunction      _actionFunction;
    private IActionProvider     _actionProvider;
    private Tooltip             tooltip;
    private TooltipDynamicText  dynamicText;
    
    public ActionFunction actionFunction => _actionFunction;
    public IActionProvider actionProvider => _actionProvider;

    void Awake()
    {
        dynamicText = GetComponent<TooltipDynamicText>();
    }

    void Update()
    {
        filler.fillAmount = 1.0f - actionFunction.CanRun(actionProvider);

        if (tooltip != null)
        {
            if (actionFunction.HasCooldown())
            {
                UpdateTooltip();
            }
        }
    }

    public void Set(ActionFunction actionFunction, IActionProvider actionProvider)
    {
        this._actionFunction = actionFunction;
        this._actionProvider = actionProvider;

        gameObject.SetActive(true);
        icon.sprite = actionFunction.displayIcon;
        text.text = actionFunction.displayName;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_actionFunction.CanRun(_actionProvider) >= 1.0f)
        {
            Globals.sndSelect?.Play();

            _actionFunction.RunAction(_actionProvider);
            border.color = colorClick;

            if (_actionFunction.CanRun(_actionProvider) <= 0.0f)
                border.FadeTo(colorHighlight, 0.2f);
            else
                border.FadeTo(colorNormal, 0.2f);
        }
        else
        {
            Globals.sndFail?.Play();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_actionFunction.CanRun(_actionProvider) >= 1.0f)
        {
            border.FadeTo(colorHighlight, 0.1f);
        }
        else
        {
            border.FadeTo(colorNormal, 0.1f);
        }

        Globals.sndHover?.Play();

        tooltip = TooltipManager.CreateTooltip();
        UpdateTooltip();
    }

    void UpdateTooltip()
    {
        var s = _actionFunction.tooltipText;
        if (dynamicText) s = dynamicText.ModifyText(s);

        tooltip.SetText(s);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        border.FadeTo(colorNormal, 0.1f);

        Globals.sndUnhover?.Play();

        tooltip?.Remove();
        tooltip = null;
    }
}
