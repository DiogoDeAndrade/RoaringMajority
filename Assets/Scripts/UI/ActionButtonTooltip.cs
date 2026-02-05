using UC;
using UnityEngine;

public class ActionButtonTooltip : TooltipDynamicText
{
    ActionButton actionButton;

    private void Start()
    {
        actionButton = GetComponent<ActionButton>();
    }

    public override string ModifyText(string baseText)
    {
        var function = actionButton.actionFunction;
        var provider = actionButton.actionProvider;

        var s = $"{function.tooltipName}\n{baseText}\n\n";

        string resultString = function.GetDefaultResultTooltip(provider);
        if (!string.IsNullOrEmpty(resultString))
        {
            s += $"{resultString}\n";
        }
        string cooldownString = function.GetDefaultCooldownTooltip(provider);
        if (!string.IsNullOrEmpty(cooldownString))
        {
            var location = provider.GetLocation();
            var cd = location.GetCooldown(function.displayName);
            if (cd.cooldown <= 0.0f)
                s += $"Cooldown: {cooldownString}s\n";
            else
                s += $"Cooldown: {Mathf.FloorToInt(cd.cooldown)}/{Mathf.FloorToInt(cd.maxCooldown)}\n";
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
