using NaughtyAttributes;
using System.Collections.Generic;
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
    [SerializeField] private Stat _statMaxPP;
    [SerializeField] private Stat _statPP;
    [SerializeField] private Stat _statCrowdSize;
    [Header("Data Lists")]
    [SerializeField] private List<ProtesterDef> _protesterTypeList;
    [SerializeField] private List<Stat>         _statList;
    [SerializeField] private List<ProtesterDef> _startProtesters;
    [Header("Prefabs")]
    [SerializeField] private DialogBox      _prefabDialogBox;
    [SerializeField] private ActionButton   _prefabActionButton;
    [Header("Start Values")]
    [SerializeField] private float _startMorale = 50.0f;
    [SerializeField] private float _startTension = 0.0f;
    [SerializeField] private float _startVisibility= 10.0f;
    [SerializeField] private float _startSupport = 5.0f;
    [SerializeField] private float _startVolatility = 0.0f;
    [SerializeField] private float _startAwareness = 10.0f;
    [SerializeField] private float _startMoney = 10.0f;
    [Header("Balancing")]
    [SerializeField] private float      _tickTime = 15.0f;
    [SerializeField] private Vector2    _recruitCooldownRange = new Vector2(2.0f, 20.0f);
    [SerializeField] private Vector2Int _ppRange = new Vector2Int(5, 50);
    [SerializeField] private float      _ppPower = 1.5f;
    [SerializeField] private float      _leaveThreshould = 0.4f;
    [SerializeField] private float      _leaveThreshouldPower = 2.0f;
    [Header("Flavour")]
    [SerializeField] private Vector2        _newsUpdateTimerRange = new Vector2(10.0f, 20.0f);
    [SerializeField] private float          _newsUpdateProbJoke = 0.25f;
    [SerializeField] private float          _newsUpdateProSide = 0.5f;
    [SerializeField] private Vector2        _newsDefaultDurationRange = new Vector2(25.0f, 35.0f);
    [SerializeField] private List<string>   _leaveSentences;
    [SerializeField] private float          _protestRestartTime = 30.0f;

    public static Stat statTension => instance._statTension;
    public static Stat statMorale => instance._statMorale;
    public static Stat statVisibility => instance._statVisibility;
    public static Stat statSupport => instance._statSupport;
    public static Stat statVolatility => instance._statVolatility;
    public static Stat statAwareness => instance._statAwareness;
    public static Stat statMoney => instance._statMoney;
    public static Stat statMaxPP => instance._statMaxPP;
    public static Stat statPP => instance._statPP;
    public static Stat statCrowdSize => instance._statCrowdSize;
    public static float startTension => instance._startTension;
    public static float startMorale => instance._startMorale;
    public static float startVisibility => instance._startVisibility;
    public static float startSupport => instance._startSupport;
    public static float startVolatility => instance._startVolatility;
    public static float startAwareness => instance._startAwareness;
    public static float startMoney => instance._startMoney;
    public static DialogBox prefabDialogBox => instance._prefabDialogBox;
    public static ActionButton prefabActionButton => instance._prefabActionButton;

    public static Vector2Int ppRange => instance._ppRange;
    public static float ppPower => instance._ppPower;
    public static float tickTime => instance._tickTime;
    public static float leaveThreshould => instance._leaveThreshould;
    public static float leaveThreshouldPower => instance._leaveThreshouldPower;
    public static Vector2 recruitCooldownRange => instance._recruitCooldownRange;

    public static List<ProtesterDef> protesterTypeList => instance._protesterTypeList;
    public static List<Stat> statList => instance._statList;
    public static List<ProtesterDef> startProtesters => instance._startProtesters;

    public static Vector2 newsUpdateTimerRange => instance._newsUpdateTimerRange;
    public static float newsUpdateProbJoke => instance._newsUpdateProbJoke;
    public static float newsUpdateProSide => instance._newsUpdateProSide;
    public static Vector2 newsDefaultDurationRange => instance._newsDefaultDurationRange;
    public static List<string> leaveSentences => instance._leaveSentences;
    public static float protestRestartTime => instance._protestRestartTime;


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
