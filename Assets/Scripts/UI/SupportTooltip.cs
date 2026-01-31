using UC;
using UnityEngine;

public class SupportTooltip : UpkeepTooltip
{
    public override string ModifyText(string baseText)
    {
        var s = baseText;

        s += "\n\n";

        s += $"<color=#{Globals.statMaxPP.color.ToHex()}>";

        var pp = GameManager.instance.Get(Globals.statPP);
        var maxPP = GameManager.instance.Get(Globals.statMaxPP);

        s += $"Protester Points (PP) = {pp}/{maxPP}";

        s += "</color>";

        return base.ModifyText(s);
    }
}
