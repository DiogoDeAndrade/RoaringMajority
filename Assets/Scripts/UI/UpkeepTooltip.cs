using UC;
using UnityEngine;

public class UpkeepTooltip : TooltipDynamicText
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
        var upkeeps = GameManager.instance.GetUpkeeps();
        if (upkeeps.ContainsKey(stat))
        {
            int v = Mathf.RoundToInt(upkeeps[stat]);
            if (v != 0)
            {
                baseText += "\n\n";

                baseText += $"<color=#{stat.color.ToHex()}>";

                baseText += $"Upkeep = {v}/tick";

                baseText += "</color>";
            }
        }
        return baseText;
    }
}
