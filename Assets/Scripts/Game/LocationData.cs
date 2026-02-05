using System.Collections.Generic;

public enum LocationState
{
    Idle,
    Protest,
    Stop,
    End
}

public class LocationData : IUpkeepProvider, IActionProvider
{
    class Cooldown
    {
        public float cooldown;
        public float maxCooldown;
    }

    private Location                        _location;
    private LocationState                   state;
    private Dictionary<Stat, float>         values = new();
    private List<ProtesterData>             _protesters = new();
    private Dictionary<string, Cooldown>    _cooldowns = new();
    private int                             _inactivityTicks;
    private float                           _restartCooldown = float.NegativeInfinity;

    public event OnChangeStat onChangeStat;

    public bool isProtesting => state == LocationState.Protest;
    public float restartCooldown => (state == LocationState.Stop) ? Globals.protestRestartTime : ((state == LocationState.End) ? _restartCooldown : 0.0f);
    public int inactivityTicks => _inactivityTicks;
    public List<ProtesterData> protesters => _protesters;
    public int protesterCount => _protesters.Count;
    public Location location => _location;

    public LocationData(Location location)
    {
        this._location = location;
        state = LocationState.Idle;
    }

    public float Get(Stat stat, float defaultValue = 0.0f)
    {
        if (stat == null) return 0.0f;

        if (values.TryGetValue(stat, out float value)) return value;

        return defaultValue;
    }

    public void Set(Stat stat, float value)
    {
        float oldValue = Get(stat, value);

        value = stat.ClampToLimit(value);
        values[stat] = value;

        onChangeStat?.Invoke(stat, oldValue, value);
    }

    public void StartProtest()
    {
        state = LocationState.Protest;
        Set(Globals.statTension, Globals.startTension);
        Set(Globals.statMorale, Globals.startMorale);
        Set(Globals.statVisibility, Globals.startVisibility);
    }
    public void StopProtest()
    {
        state = LocationState.Stop;
    }

    public void EndProtest()
    {
        state = LocationState.End;
        _restartCooldown = Globals.protestRestartTime;
    }

    public int GetPP()
    {
        int pp = 0;
        foreach (var p in _protesters)
        {
            pp += p.def.cost;
        }
        return pp;
    }

    public void AddProtester(ProtesterData pd)
    {
        _protesters.Add(pd);
    }

    public void GetUpkeep(Dictionary<Stat, float> deltaStat)
    {
        if (isProtesting)
        {
            _location.GetUpkeep(deltaStat, this);

            foreach (var protester in _protesters)
            {
                protester.GetUpkeep(deltaStat);
            }
        }        
    }

    public void ElapseSimulation(float deltaTime)
    {
        foreach (var cd in _cooldowns)
        {
            cd.Value.cooldown -= deltaTime;
        }

        if (state == LocationState.End)
        {
            _restartCooldown -= deltaTime;
            if (_restartCooldown <= 0.0f)
            {
                state = LocationState.Idle;
            }
        }
    }

    public void Tick()
    {
        _inactivityTicks++;
    }

    public LocationData GetLocation()
    {
        return this;
    }

    public void NotifyAction()
    {
        _inactivityTicks = 0;
    }

    public void SetCooldown(string displayName, float v)
    {
        _cooldowns[displayName] = new Cooldown
        {
            cooldown = v,
            maxCooldown = v
        };
    }

    public (float cooldown, float maxCooldown) GetCooldown(string displayName)
    {
        if (_cooldowns.TryGetValue(displayName, out var cooldown))
        {
            return (cooldown.cooldown, cooldown.maxCooldown);
        }

        return (0.0f, 1.0f);
    }

    public void RemoveProtester(ProtesterData protester)
    {
        _protesters.Remove(protester);
    }

    public Protester GetProtester()
    {
        return null;
    }

    public void UpdateDerivedStats()
    {
        Set(Globals.statCrowdSize, protesterCount);
    }
}
