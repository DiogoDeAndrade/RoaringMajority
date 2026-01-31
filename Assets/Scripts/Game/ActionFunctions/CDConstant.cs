using System;
using UC;
using UnityEngine;

[Serializable]
[PolymorphicName("Constant")]
public class CDConstant : CooldownFunction
{
    [SerializeField] private float constantValue;

    public override float GetCooldown(IActionProvider mainObject)
    {
        return constantValue;
    }
}
