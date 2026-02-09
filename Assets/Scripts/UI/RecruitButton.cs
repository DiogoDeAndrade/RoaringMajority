using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UC;

public class RecruitButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] 
    private ProtesterDef    def;
    [SerializeField]
    private Image           border;
    [SerializeField]
    private Image           icon;
    [SerializeField]
    private Image           filler;
    [SerializeField]
    private Color           colorNormal = Color.white;
    [SerializeField]
    private Color           colorHighlight = Color.white;
    [SerializeField]
    private Color           colorClick = Color.white;
    [SerializeField]
    private Color           colorClickFail = Color.red;

    Tooltip             tooltip;
    TooltipDynamicText  dynamicText;

    public ProtesterDef protesterType => def;

    void Start()
    {
        dynamicText = GetComponent<TooltipDynamicText>();

        icon.sprite = def.icon;
        border.color = colorNormal;
    }

    void Update()
    {
        filler.fillAmount = 1.0f - GameManager.instance.SpawnAvailabilityPercentage(def);

        if ((filler.fillAmount < 1.0f) && (tooltip))
        {
            var s = def.tooltipText;
            if (dynamicText) s = dynamicText.ModifyText(s);
            tooltip.SetText(s);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (GameManager.instance.SpawnAvailabilityPercentage(def) >= 1.0f)
        {
            if (GameManager.instance.Spawn(def))
            {
                border.color = colorClick;
                Globals.sndSelect?.Play();
            }
            else
            {
                border.color = colorClickFail;
                Globals.sndFail?.Play();
            }

            if (GameManager.instance.SpawnAvailabilityPercentage(def) >= 1.0f)
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
        if (GameManager.instance.SpawnAvailabilityPercentage(def) >= 1.0f)
        {
            border.FadeTo(colorHighlight, 0.1f);
        }
        else
        {
            border.FadeTo(colorNormal, 0.1f);
        }

        Globals.sndHover?.Play();

        tooltip = TooltipManager.CreateTooltip();

        var s = def.tooltipText;
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
