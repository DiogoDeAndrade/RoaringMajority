using NaughtyAttributes;
using System;
using TMPro;
using UC;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StatDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] 
    private Stat        stat;
    [SerializeField] 
    private Image       iconImage;
    [SerializeField] 
    private Image       colorBar;
    [SerializeField]
    private TextMeshProUGUI labelText;
    [SerializeField]
    private TextMeshProUGUI valueText;

    Tooltip tooltip;
    string  baseText;

    void Start()
    {
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
        tooltip = TooltipManager.CreateTooltip();
        tooltip.SetText(stat.tooltipText);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip?.Remove();
        tooltip = null;
    }
}
