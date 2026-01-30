using UC;
using UnityEngine;
using UnityMeshSimplifier.Internal;

public class RecruitButtonTooltip : TooltipDynamicText
{
    RecruitButton rb;

    private void Start()
    {
        rb = GetComponent<RecruitButton>();
    }

    public override string ModifyText(string baseText)
    {
        var s = baseText;

        var def = rb.protesterType;
        s = s.Replace("{COST}", $"<color=#{Globals.statMaxPP.color.ToHex()}>{def.cost}</color>");

        var protesterTypes = Globals.protesterTypeList;
        foreach (var pt in protesterTypes)
        {
            s = s.Replace(pt.displayName, $"<color=#{pt.color.ToHex()}>{pt.displayName}</color>");
        }

        var stats = Globals.statList;
        foreach (var stat in stats)
        {
            s = s.Replace(stat.displayName, $"<color=#{stat.color.ToHex()}>{stat.displayName}</color>");
        }

        return s;
    }
}
