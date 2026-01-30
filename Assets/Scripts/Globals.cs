using NaughtyAttributes;
using UC;
using UnityEngine;

[CreateAssetMenu(fileName = "Globals", menuName = "RM/Globals")]
public class Globals : GlobalsBase
{
    [HorizontalLine(color: EColor.Green)]
    [Header("Stats")]
    [SerializeField] private Stat _statMorale;
    [SerializeField] private Stat _statTension;
    [SerializeField] private Stat _statVisibility;
    [SerializeField] private Stat _statSupport;
    [SerializeField] private Stat _statVolatility;
    [SerializeField] private Stat _statAwareness;
    [SerializeField] private Stat _statMoney;
    [Header("Tags")]
    [SerializeField] 
    private Hypertag _tagBackground;
    [Header("Start Values")]
    [SerializeField] private float _startMorale = 50.0f;
    [SerializeField] private float _startTension = 0.0f;
    [SerializeField] private float _startVisibility= 10.0f;
    [SerializeField] private float _startSupport = 5.0f;
    [SerializeField] private float _startVolatility = 0.0f;
    [SerializeField] private float _startAwareness = 10.0f;
    [SerializeField] private float _startMoney = 10.0f;

    public static Stat statTension => instance._statTension;
    public static Stat statMorale => instance._statMorale;
    public static Stat statVisibility => instance._statVisibility;
    public static Stat statSupport => instance._statSupport;
    public static Stat statVolatility => instance._statVolatility;
    public static Stat statAwareness => instance._statAwareness;
    public static Stat statMoney => instance._statMoney;
    public static float startTension => instance._startTension;
    public static float startMorale => instance._startMorale;
    public static float startVisibility => instance._startVisibility;
    public static float startSupport => instance._startSupport;
    public static float startVolatility => instance._startVolatility;
    public static float startAwareness => instance._startAwareness;
    public static float startMoney => instance._startMoney;
    public static Hypertag tagBackground => instance._tagBackground;

    protected static Globals _instance = null;

    public static Globals instance
    {
        get
        {
            if (_instance) return _instance;

            Debug.Log("Globals not loaded, loading...");

            var allConfigs = Resources.LoadAll<Globals>("");
            if (allConfigs.Length == 1)
            {
                _instance = allConfigs[0];
            }
            else
            {
                Debug.LogError("Globals not found, check Resources folder!");
            }

            return _instance;
        }
    }

}
