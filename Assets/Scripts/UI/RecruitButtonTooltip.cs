using UC;
using UnityEngine;

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

        // Check if we have a {REQUIREMENTS} tag, and if so, replace it with the actual requirements
        if (s.IndexOf("{REQUIREMENTS}") >= 0)
        {
            string reqString = def.GetRequirementsTooltip(GameManager.instance);
            if (!string.IsNullOrEmpty(reqString))
            {
                s = s.Replace("{REQUIREMENTS}", $"Requires: {reqString}");
            }
        }

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
