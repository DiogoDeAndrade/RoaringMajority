using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
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

        onChangeStat?.Invoke(stat, oldValue, value);
    }

    public bool isOnLocation => _currentLocation != null;
    public LocationData currentLocationData => _currentLocationData;
}
