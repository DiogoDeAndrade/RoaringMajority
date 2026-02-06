using UC;
using UnityEngine;

public class StatTooltip : TooltipDynamicText
{
    protected StatDisplay statDisplay;
    protected Stat        stat;

    private void Start()
    {
        statDisplay = GetComponent<StatDisplay>();
        stat = statDisplay.stat;
    }

    public override string ModifyText(string baseText)
    {
        var txt = GameManager.instance.GetStatTooltip(stat);
        if (!string.IsNullOrEmpty(txt))
        {
            baseText += "\n\n" + txt;
        }

        return baseText;
    }
}
