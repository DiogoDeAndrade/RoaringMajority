using UnityEditor;
using UnityEngine;
using UC.Editor;

[CustomPropertyDrawer(typeof(CostFunction), true)]
public class CostFunctionDrawer : BaseFunctionDrawer<CostFunction>
{
}

[CustomPropertyDrawer(typeof(ConditionFunction), true)]
public class ConditionFunctionDrawer : BaseFunctionDrawer<ConditionFunction>
{
}

[CustomPropertyDrawer(typeof(CooldownFunction), true)]
public class CooldownFunctionDrawer : BaseFunctionDrawer<CooldownFunction>
{
}


[CustomPropertyDrawer(typeof(ActionFunction), true)]
public class ActionFunctionDrawer : BaseFunctionDrawer<ActionFunction>
{
}
