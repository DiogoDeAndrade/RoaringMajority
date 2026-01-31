using NaughtyAttributes;
using TMPro;
using UC;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StatDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] 
    public  Stat        stat;
    [SerializeField] 
    private Image       iconImage;
    [SerializeField] 
    private Image       colorBar;
    [SerializeField]
    private TextMeshProUGUI labelText;
    [SerializeField]
    private TextMeshProUGUI valueText;

    Tooltip             tooltip;
    string              baseText;
    TooltipDynamicText  dynamicText;

    void Start()
    {
        dynamicText = GetComponent<TooltipDynamicText>();

        GameManager.instance.onChangeStat += UpdateStat;
        
        baseText = valueText.text;

        ForceUpdate();
    }

    private void UpdateStat(Stat stat, float oldValue, float newValue)
    {
        if ((stat != null) && (this.stat != stat)) return;

        if (stat == null)
        {
            stat = this.stat;
            oldValue = newValue = GameManager.instance.Get(stat);
        }

        if (stat.isLocal)
        {
            if (GameManager.instance.isOnLocation)
            {
                var locationData = GameManager.instance.currentLocationData;                
                gameObject.SetActive(locationData.isProtesting);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
        else
        {
            gameObject.SetActive(true);
        }

        iconImage.sprite = stat.icon;
        colorBar.color = stat.color;
        labelText.text = stat.displayName;
        if (!string.IsNullOrEmpty(baseText))
            valueText.text = string.Format(baseText, newValue);
    }

    [Button("Force Update")]
    void ForceUpdate()
    {
        UpdateStat(null, 0.0f, 0.0f);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        var s = stat.tooltipText;
        if (dynamicText) s = dynamicText.ModifyText(s);

        tooltip = TooltipManager.CreateTooltip();
        tooltip.SetText(s);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip?.Remove();
        tooltip = null;
    }
}
