using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameState { Menu, Started };
    [SerializeField]
    public GameState            state = GameState.Menu;
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
    private float                               tickTimer;

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

        tickTimer = Time.time;
    }

    void StartGame()
    {
        state = GameState.Started;

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
        float simulationTimeScale = 1.0f;

        if (Input.GetKey(KeyCode.Alpha1)) simulationTimeScale = 2.0f;
        if (Input.GetKey(KeyCode.Alpha2)) simulationTimeScale = 4.0f;
        if (Input.GetKey(KeyCode.Alpha3)) simulationTimeScale = 8.0f;
        if (Input.GetKey(KeyCode.Alpha4)) simulationTimeScale = 16.0f;

        tickTimer -= Time.deltaTime * simulationTimeScale;
        if (tickTimer <= 0.0f)
        {
            GameTick();
            tickTimer = Globals.tickTime;
        }
        UpdateDerivedStats();
        if (isOnLocation)
        {
            UpdateRecruitmentButtons();
        }
    }

    void GameTick()
    {
        // Run upkeep
        var deltaStats = GetUpkeeps();

        foreach (var ds in deltaStats)
        {
            Set(ds.Key, Get(ds.Key) + ds.Value);
        }

        foreach (var location in locations)
        {
            location.Value.Tick();
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

        value = stat.ClampToLimit(value);
        values[stat] = value;

        if (oldValue != value)
            onChangeStat?.Invoke(stat, oldValue, value);
    }

    public bool Spawn(ProtesterDef def)
    {
        // Check if we have enough PP
        if (!def.CanSpawn()) return false;

        // Add logic entity
        ProtesterData pd = new ProtesterData(def, currentLocationData);
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

    public Dictionary<Stat, float> GetUpkeeps()
    {
        Dictionary<Stat, float> deltaStats = new();
        foreach (var location in locations)
        {
            location.Value.GetUpkeep(deltaStats);
        }

        return deltaStats;
    }

    public bool isOnLocation => _currentLocation != null;
    public LocationData currentLocationData => _currentLocationData;
    public int availablePP => Mathf.FloorToInt(Get(Globals.statMaxPP) - Get(Globals.statPP));
}
