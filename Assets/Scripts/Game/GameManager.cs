using NaughtyAttributes;
using System.Collections.Generic;
using UC;
using UnityEngine;
using static UnityEditor.FilePathAttribute;

public class GameManager : MonoBehaviour, IUpkeepProvider, IActionProvider
{
    public enum GameState { Menu, Started };
    [SerializeField]
    public GameState            state = GameState.Menu;
    [SerializeField]
    private RectTransform       mainUI;
    [SerializeField]
    private RectTransform       actionButtonContainer;
    [Header("Debug")]
    [SerializeField]
    private bool                autoStartGame;
    [SerializeField]
    private Cause               autoStartCause;
    [SerializeField] 
    private Location            debugStartLocation;
    [SerializeField, ShowIf(nameof(hasDebugStartLocation))]
    private bool                autoStartProtest;

    private Cause                               _currentCause;
    private Location                            _currentLocation;
    private LocationData                        _currentLocationData;
    private Dictionary<Stat, float>             values = new();
    private Dictionary<Location, LocationData>  locations = new();
    private float                               tickTimer;
    private float                               recruitCooldown = 0.0f;
    private float                               recruitCooldownMax = 1.0f;
    private DialogBox                           startProtestDialog;
    private DialogBox                           endProtestDialog;
    private float                               newsTimer;
    private bool                                refreshActions;

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
        if (autoStartGame)
        {
            StartGame(autoStartCause);
            _currentLocation = debugStartLocation;
            InitLocation();
        }

        tickTimer = Time.time;
    }

    void StartGame(Cause cause)
    {
        _currentCause = cause;
        state = GameState.Started;

        Set(Globals.statSupport, Globals.startSupport);
        Set(Globals.statAwareness, Globals.startAwareness);
        Set(Globals.statMoney, Globals.startMoney);
        Set(Globals.statVolatility, Globals.startVolatility);

        recruitCooldown = 0.0f;
        newsTimer = Globals.newsUpdateTimerRange.Random();
    }

    void InitLocation()
    {
        if (!locations.TryGetValue(_currentLocation, out _currentLocationData))
        {
            _currentLocationData = new LocationData(_currentLocation);
            _currentLocationData.onChangeStat += (stat, oldValue, newValue) => onChangeStat?.Invoke(stat, oldValue, newValue);
            locations[_currentLocation] = _currentLocationData;
        }

        var obj = FindFirstObjectByType<LocationObject>();
        if (obj)
        {
            Destroy(obj.gameObject);
        }

        Instantiate(_currentLocation.prefab, Vector3.zero, Quaternion.identity);

        if (autoStartProtest)
        {
            StartProtest();
        }
    }

    void Update()
    {
        if (state == GameState.Started)
        {
            UpdateTicker();

            float simulationTimeScale = 1.0f;

            if (Input.GetKey(KeyCode.Alpha1)) simulationTimeScale = 2.0f;
            if (Input.GetKey(KeyCode.Alpha2)) simulationTimeScale = 4.0f;
            if (Input.GetKey(KeyCode.Alpha3)) simulationTimeScale = 8.0f;
            if (Input.GetKey(KeyCode.Alpha4)) simulationTimeScale = 16.0f;

            if (Input.GetKeyDown(KeyCode.F1)) Cheat(Globals.statMorale, 10.0f);
            if (Input.GetKeyDown(KeyCode.F2)) Cheat(Globals.statTension, 10.0f);
            if (Input.GetKeyDown(KeyCode.F3)) Cheat(Globals.statVisibility, 10.0f);
            if (Input.GetKeyDown(KeyCode.F4)) Cheat(Globals.statSupport, 10.0f);
            if (Input.GetKeyDown(KeyCode.F5)) Cheat(Globals.statVolatility, 10.0f);
            if (Input.GetKeyDown(KeyCode.F6)) Cheat(Globals.statAwareness, 10.0f);
            if (Input.GetKeyDown(KeyCode.F7)) Cheat(Globals.statMoney, 50.0f);

            ElapseSimulation(Time.deltaTime * simulationTimeScale);
        }
    }

    void Cheat(Stat stat, float deltaValue)
    {
        Set(stat, (Input.GetKey(KeyCode.LeftShift) ? (-1.0f) : (1.0f)) * deltaValue + Get(stat));
    }

    void UpdateTicker()
    {
        newsTimer -= Time.deltaTime;
        if (newsTimer <= 0.0f)
        {
            string txt = "";
            if (Random.Range(0.0f, 1.0f) < Globals.newsUpdateProbJoke)
            {
                txt = _currentCause.GetJoke();
            }
            else
            {
                txt = _currentCause.GetNews(Random.Range(0.0f, 1.0f) < Globals.newsUpdateProSide);
            }
            float duration = Globals.newsDefaultDurationRange.Random();
            Ticker.AddNews(txt, duration);
            newsTimer = duration + Globals.newsUpdateTimerRange.Random();
        }
    }

    void ElapseSimulation(float deltaTime)
    {
        recruitCooldown -= deltaTime;
        tickTimer -= deltaTime;
        if (tickTimer <= 0.0f)
        {
            GameTick();
            tickTimer = Globals.tickTime;
        }
        UpdateDerivedStats();

        currentLocationData.ElapseSimulation(deltaTime);

        if (isOnLocation)
        {
            UpdateRecruitmentButtons();
            if (refreshActions)
            {
                UpdateActionButtons();
            }

            if (_currentLocationData.isProtesting)
            {
                if (endProtestDialog == null)
                {
                    var victory = _currentLocation.GetVictoryCondition(_currentLocationData);
                    if (victory)
                    {
                        victory.RunAction();

                        endProtestDialog = DialogBox.CreateBox(victory.text, true, Globals.prefabDialogBox, mainUI,
                            yesAction: (dialogBox) =>
                            {
                                _currentLocationData.EndProtest();
                                endProtestDialog = null;

                                EverybodyLeaves();

                                return true;
                            },
                            noAction: (dialogBox) =>
                            {
                                endProtestDialog = null;
                                return true;
                            },
                            yesText: "CONTINUE",
                            noText: "");
                        
                        _currentLocationData.StopProtest();
                        refreshActions = true;
                    }
                }
            }
            else
            {
                if (_currentLocationData.restartCooldown <= 0.0f)
                {
                    if (startProtestDialog == null)
                    {
                        int requireCount = 0;
                        foreach (var p in Globals.startProtesters)
                        {
                            requireCount += p.cost;
                        }

                        var message = "START PROTEST?";
                        message += "\n\n";
                        message += $"Requires <color=#{Globals.statPP.color.ToHex()}>{requireCount} PP</color>";
                        startProtestDialog = DialogBox.CreateBox(message, true, Globals.prefabDialogBox, mainUI,
                            yesAction: (dialogBox) =>
                            {
                                if (StartProtest())
                                {
                                    dialogBox = null;
                                    return true;
                                }
                                return false;
                            },
                            noAction: (dialogBox) =>
                            {
                                dialogBox = null;
                                return true;
                            },
                            yesCondition: (dialogBox) =>
                            {
                                return (Get(Globals.statPP) + requireCount) <= Get(Globals.statMaxPP);
                            });
                    }
                }
            }
        }
    }

    void GameTick()
    {
        // Location tick
        foreach (var location in locations)
        {
            var deltaStats = new Dictionary<Stat, float>();
            location.Value.GetUpkeep(deltaStats);

            foreach (var ds in deltaStats)
            {
                Set(ds.Key, Get(ds.Key) + ds.Value, location.Value);
            }

            location.Value.Tick();

            // Check for leaving
            float morale = location.Value.Get(Globals.statMorale);
            if (morale < Globals.leaveThreshould)
            {
                float t = (Globals.leaveThreshould - morale) / Globals.leaveThreshould; // 0 at threshold, 1 at 0
                t = Mathf.Clamp01(t);

                float p = Mathf.Pow(t, Globals.leaveThreshouldPower);

                for (int i = 0; i < location.Value.protesterCount; i++)
                {
                    if (Random.Range(0.0f, 1.0f) < p)
                    {
                        // One person leaves!
                        var protester = location.Value.protesters.Random();
                        location.Value.RemoveProtester(protester);

                        var protesterObject = GetProtester(protester);
                        if (protesterObject)
                        {
                            protesterObject.Say(Globals.leaveSentences.Random(), 2.0f);
                            protesterObject.MoveTo(GetSpawnPos(true), () =>
                            {
                                Destroy(protesterObject.gameObject);
                            });
                        }
                    }
                }
            }
        }
    }

    void EverybodyLeaves()
    {
        // One person leaves!
        List<ProtesterData> protesters = new(_currentLocationData.protesters);
        foreach (var protester in protesters)
        {
            _currentLocationData.RemoveProtester(protester);

            var protesterObject = GetProtester(protester);
            if (protesterObject)
            {
                protesterObject.MoveTo(GetSpawnPos(true), () =>
                {
                    Destroy(protesterObject.gameObject);
                });
            }
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

        // Evaluate local stats
        foreach (var location in locations.Values)
        {
            location.UpdateDerivedStats();
        }   
    }

    void UpdateRecruitmentButtons()
    {
        var buttons = FindObjectsByType<RecruitButton>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var button in buttons)
        {
            button.gameObject.SetActive((_currentLocationData.isProtesting) && (IsAvailable(button.protesterType)));
        }
    }

    void UpdateActionButtons()
    {
        // Delete old buttons
        var buttons = FindObjectsByType<ActionButton>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var button in buttons)
        {
            Destroy(button.gameObject);
        }

        if (_currentLocationData.isProtesting)
        {
            // Get action from cause
            Dictionary<string, (ActionFunction function, IActionProvider provider)> actions = new();

            foreach (var action in _currentCause.actions)
            {
                if (actions.ContainsKey(action.displayName)) continue;

                actions.Add(action.displayName, (action, _currentCause));
            }

            // Get action from location
            foreach (var action in _currentLocationData.location.actions)
            {
                if (actions.ContainsKey(action.displayName)) continue;

                actions.Add(action.displayName, (action, _currentLocationData));
            }

            // Get actions from protesters
            var protesters = _currentLocationData.protesters;
            foreach (var protester in protesters)
            {
                foreach (var action in protester.def.actions)
                {
                    if (actions.ContainsKey(action.displayName)) continue;

                    actions.Add(action.displayName, (action, protester));
                }
            }

            // Add action buttons
            foreach (var action in actions)
            {
                var actionButton = Instantiate(Globals.prefabActionButton, actionButtonContainer);
                actionButton.Set(action.Value.function, action.Value.provider);
            }
        }

        refreshActions = false;
    }

    public bool IsAvailable(ProtesterDef type, LocationData location = null)
    {
        var loc = (location == null) ? (_currentLocationData) : (location);
        if (!loc.isProtesting) return false;

        return true;
    }

    public float Get(Stat stat, LocationData location = null, float defaultValue = 0.0f)
    {
        var loc = (location == null) ? (_currentLocationData) : (location);
        if (stat.isLocal) return loc?.Get(stat, defaultValue) ?? 0.0f;

        if (values.TryGetValue(stat, out float value)) return value;

        return defaultValue;
    }

    public void Set(Stat stat, float value, LocationData location = null)
    {
        if (stat.isLocal)
        {
            var loc = (location == null) ? (_currentLocationData) : (location);
            loc.Set(stat, value);
            return;
        }

        float oldValue = Get(stat, defaultValue : value);

        value = stat.ClampToLimit(value);
        values[stat] = value;

        if (oldValue != value)
            onChangeStat?.Invoke(stat, oldValue, value);
    }

    public bool Spawn(ProtesterDef def, LocationData location = null)
    {
        var loc = (location == null) ? (_currentLocationData) : (location);

        // Check if we have enough PP
        if (!def.CanSpawn(loc)) return false;

        // Add logic entity
        ProtesterData pd = new ProtesterData(def, currentLocationData);

        loc.AddProtester(pd);

        Spawn(pd, true, true);

        recruitCooldownMax = recruitCooldown = GetRecruitmentCooldown();

        refreshActions = true;

        return true;
    }

    public float SpawnAvailabilityPercentage(ProtesterDef def, LocationData location = null)
    {
        var loc = (location == null) ? (_currentLocationData) : (location);
        if (!def.CanSpawn(loc)) return 0.0f;

        float cd = GetRecruitmentCooldown();
        float t = 1.0f - Mathf.Clamp01(recruitCooldown / recruitCooldownMax);

        return t;
    }

    public void Spawn(ProtesterData pd, bool leftSide, bool animate)
    {
        var protester = Instantiate(Globals.prefabProtester, LocationObject.instance.transform);
        protester.protesterData = pd;

        var stagingArea = (leftSide) ? (LocationObject.leftProtestArea) : (LocationObject.rightProtestArea);
        var targetPos = stagingArea.Random();

        if (animate)
        {
            var spawnPos = GetSpawnPos(leftSide);

            protester.transform.position = spawnPos;
            protester.MoveTo(targetPos);
        }
        else
        {
            protester.transform.position = targetPos;
        }

        refreshActions = true;
    }

    Vector3 GetSpawnPos(bool leftSide)
    {
        var spawnArea = (leftSide) ? (LocationObject.leftSpawnArea) : (LocationObject.rightSpawnArea);
        var spawnPos = spawnArea.Random();

        return spawnPos;
    }

    float GetUpkeep(Stat stat, LocationData location = null)
    {
        var loc = (location == null) ? (_currentLocationData) : (location);

        return loc.GetUpkeep(stat);
    }

    public float GetRecruitmentCooldown()
    {
        float awareness = Get(Globals.statAwareness);
        float t = Mathf.Clamp01(awareness / 100f);
        float easeOut = 1f - Mathf.Pow(t, 1.5f);
        var range = Globals.recruitCooldownRange;
        float cooldown = range.x + (range.y - range.x) * easeOut;

        return cooldown;
    }

    public bool isOnLocation => _currentLocation != null;
    public LocationData currentLocationData => _currentLocationData;
    public int availablePP => Mathf.FloorToInt(Get(Globals.statMaxPP) - Get(Globals.statPP));

    bool StartProtest()
    {
        if (_currentLocationData == null) return false;

        _currentLocationData.StartProtest();

        UpdateDerivedStats();

        for (int i = 0; i < Globals.startProtesters.Count; i++)
        {
            Spawn(Globals.startProtesters[i]);
        }

        var translator = new Dictionary<string, string>
        {
            ["{LOCATION_NAME}"] = _currentLocation.newsName,
        };
        string item = _currentCause.GetStartText(translator);

        Ticker.AddNews(item, 30.0f);

        return true;
    }

    public Protester GetProtester(ProtesterData data)
    {
        if (data != null)
        {
            var allProtesters = FindObjectsByType<Protester>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var protester in allProtesters)
            {
                if (protester.protesterData == data) return protester;
            }
        }

        return null;
    }

    public LocationData GetLocation()
    {
        return _currentLocationData;
    }

    public Protester GetProtester()
    {
        return GetProtester(_currentLocationData.protesters.Random());
    }

    public string GetStatTooltip(Stat stat)
    {
        string  txt = "";
        var     upkeep = GetUpkeep(stat);
        int     v = Mathf.RoundToInt(upkeep);
        if (v != 0)
        {
            txt = $"\nUpkeep: <color=#{stat.color.ToHex()}>";

            txt += ((v > 0.0f) ? "+" : "") + $"{v}</color>/tick";
        }

        var localTooltip = GetBuffTooltip(stat);
        if (!string.IsNullOrEmpty(localTooltip))
        {
            txt += $"{localTooltip}";
        }

        return txt;
    }

    private string GetBuffTooltip(Stat stat)
    {
        return _currentLocationData.GetBuffTooltip(stat);
    }
}
