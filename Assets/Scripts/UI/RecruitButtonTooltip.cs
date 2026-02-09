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

        if (s.IndexOf("{COST}") >= 0)
        {
            var def = rb.protesterType;
            s = s.Replace("{COST}", $"<color=#{Globals.statMaxPP.color.ToHex()}>{def.cost}</color>");
        }

        // Check if we have a {REQUIREMENTS} tag, and if so, replace it with the actual requirements
        if (s.IndexOf("{REQUIREMENTS}") >= 0)
        {
            var def = rb.protesterType;
            string reqString = def.GetRequirementsTooltip(GameManager.instance);
            if (!string.IsNullOrEmpty(reqString))
            {
                s = s.Replace("{REQUIREMENTS}", $"Requires: {reqString}");
            }
        }

        // Add cooldown 
        var cd = GameManager.instance.GetRecruitmentCooldownTime();
        if (cd.cooldown > 0.0f)
        {
            s += $"\n\n<color=#4694A8>Cooldown</color>: {Mathf.CeilToInt(cd.cooldown)}/{Mathf.CeilToInt(cd.maxCooldown)}";
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
