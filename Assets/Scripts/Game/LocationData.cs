using System;
using System.Collections.Generic;
using UC;
using Unity.VisualScripting;
using UnityEngine;

public enum LocationState
{
    Idle,
    Protest
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

    public event OnChangeStat onChangeStat;

    public bool isProtesting => state == LocationState.Protest;
    public int inactivityTicks => _inactivityTicks;
    public List<ProtesterData> protesters => _protesters;
    public int protesterCount => _protesters.Count;
    public Location location => _location;

    public LocationData(Location location)
    {
        this._location = location;
        state = LocationState.Idle;
    }

    public float Get(Stat stat)
    {
        if (stat == null) return 0.0f;

        if (values.TryGetValue(stat, out float value)) return value;

        return 0.0f;
    }

    public void Set(Stat stat, float value)
    {
        float oldValue = Get(stat);

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
}
