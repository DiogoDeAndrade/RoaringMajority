using UC;
using UnityEngine;

public class SupportTooltip : StatTooltip
{
    public override string ModifyText(string baseText)
    {
        var s = baseText;

        s += "\n\n";

        s += $"";

        var pp = GameManager.instance.Get(Globals.statPP);
        var maxPP = GameManager.instance.Get(Globals.statMaxPP);
        var crowdSize = GameManager.instance.GetCrowdSize();

        var cText = $"<color=#{Globals.statMaxPP.color.ToHex()}>";

        s += $"Protester Points (PP) = {cText}{pp}</color>/{cText}{maxPP}</color>\n";
        s += $"Protesters = {cText}{crowdSize}</color>";

        return base.ModifyText(s);
    }
}
