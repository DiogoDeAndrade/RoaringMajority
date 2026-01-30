using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;
using UC;

public class GameManager : MonoBehaviour
{
    [SerializeField] 
    private Protester           protesterPrefab;
    [Header("Debug")]
    [SerializeField]
    private bool                autoStartGame; 
    [SerializeField] 
    private Location            debugStartLocation;
    [SerializeField, ShowIf(nameof(hasDebugStartLocation))]
    private bool                autoStartProtest;

    private Location                            _currentLocation;
    private LocationData                        _currentLocationData;
    private Dictionary<Stat, float>             values = new();
    private Dictionary<Location, LocationData>  locations = new();

    public event OnChangeStat onChangeStat;

    private static GameManager _instance;
    public static GameManager instance => _instance;

    bool hasDebugStartLocation => debugStartLocation != null;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // Define locations

#if UNITY_EDITOR
        if (autoStartGame)
        {
            StartGame();
        }
        if (debugStartLocation != null)
        {
            _currentLocation = debugStartLocation;
            InitLocation();
        }
#endif
    }

    void StartGame()
    {
        Set(Globals.statSupport, Globals.startSupport);
        Set(Globals.statAwareness, Globals.startAwareness);
        Set(Globals.statMoney, Globals.startMoney);
        Set(Globals.statVolatility, Globals.startVolatility);
    }

    void InitLocation()
    {
        if (!locations.TryGetValue(_currentLocation, out _currentLocationData))
        {
            _currentLocationData = new LocationData(_currentLocation);
            _currentLocationData.onChangeStat += (stat, oldValue, newValue) => onChangeStat?.Invoke(stat, oldValue, newValue);
            locations[_currentLocation] = _currentLocationData;

            if (autoStartProtest)
            {
                _currentLocationData.StartProtest();
            }
        }

        var backgroundSprite = Globals.tagBackground.FindFirst<SpriteRenderer>();
        if (backgroundSprite)
        {
            backgroundSprite.sprite = _currentLocation.backgroundImage;
        }
    }

    void Update()
    {
        UpdateDerivedStats();
        if (isOnLocation)
        {
            UpdateRecruitmentButtons();
        }
    }

    void UpdateDerivedStats()
    {
        // Max. Protester Points
        float support = Mathf.Clamp01(Get(Globals.statSupport) * 0.01f);
        float maxPP = Mathf.Floor(Globals.ppRange.x + (Globals.ppRange.y - Globals.ppRange.x) * Mathf.Pow(support, Globals.ppPower));
        Set(Globals.statMaxPP, maxPP);

        // Current Protester Points
        int pp = 0;
        foreach (var location in locations.Values)
        {
            pp += location.GetPP();
        }
        Set(Globals.statPP, pp);
    }

    void UpdateRecruitmentButtons()
    {
        var buttons = FindObjectsByType<RecruitButton>(FindObjectsSortMode.None);
        foreach (var button in buttons)
        {
            button.gameObject.SetActive(IsAvailable(button.protesterType));
        }
    }

    public bool IsAvailable(ProtesterDef type)
    {
        return true;
    }

    public float Get(Stat stat)
    {
        if (stat.isLocal) return _currentLocationData?.Get(stat) ?? 0.0f;

        if (values.TryGetValue(stat, out float value)) return value;

        return 0.0f;
    }

    public void Set(Stat stat, float value)
    {
        if (stat.isLocal)
        {
            _currentLocationData.Set(stat, value);
            return;
        }

        float oldValue = Get(stat);

        values[stat] = value;

        if (oldValue != value)
            onChangeStat?.Invoke(stat, oldValue, value);
    }

    public bool Spawn(ProtesterDef def)
    {
        // Check if we have enough PP
        if (!def.CanSpawn()) return false;

        // Add logic entity
        ProtesterData pd = new ProtesterData(def);
        currentLocationData.AddProtester(pd);

        Spawn(pd, true, true);

        return true;
    }

    public void Spawn(ProtesterData pd, bool leftSide, bool animate)
    {
        var protester = Instantiate(protesterPrefab);
        protester.protesterData = pd;

        var stagingAreaTag = (leftSide) ? (Globals.tagProtestArea) : (Globals.tagOppositeArea);
        var targetPos = stagingAreaTag.FindFirst<PolygonCollider2D>().Random();

        if (animate)
        {
            var spawnAreaTag = (leftSide) ? (Globals.tagProtestSpawnArea) : (Globals.tagOppositeSpawnArea);
            var spawnPos = spawnAreaTag.FindFirst<PolygonCollider2D>().Random();

            protester.transform.position = spawnPos;
            protester.MoveTo(targetPos);
        }
        else
        {
            protester.transform.position = targetPos;
        }
    }

    public bool isOnLocation => _currentLocation != null;
    public LocationData currentLocationData => _currentLocationData;
    public int availablePP => Mathf.FloorToInt(Get(Globals.statMaxPP) - Get(Globals.statPP));
}
