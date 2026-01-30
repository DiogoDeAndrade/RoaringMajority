using UnityEngine;

[CreateAssetMenu(fileName = "Stat", menuName = "RM/Stat")]
public class Stat : ScriptableObject
{
    public enum StatType { Local, Global };

    public StatType type = StatType.Local;
    public string   displayName;
    public Color    color;
    public Sprite   icon;
    [TextArea]
    public string   tooltipText;

    public bool isLocal => type == StatType.Local;
}
