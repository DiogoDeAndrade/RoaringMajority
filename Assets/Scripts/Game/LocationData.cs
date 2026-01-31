using System;
using System.Collections.Generic;
using UnityEngine;

public enum LocationState
{
    Idle,
    Protest
}

public class LocationData
{
    private Location                location;
    private LocationState           state;
    private Dictionary<Stat, float> values = new();
    private List<ProtesterData>     protesters = new();
    private int                     _inactivityTicks;

    public event OnChangeStat onChangeStat;

    public bool isProtesting => state == LocationState.Protest;
    public int inactivityTicks => _inactivityTicks;

    public LocationData(Location location)
    {
        this.location = location;
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
        foreach (var p in protesters)
        {
            pp += p.def.cost;
        }
        return pp;
    }

    public void AddProtester(ProtesterData pd)
    {
        protesters.Add(pd);
    }

    public void GetUpkeep(Dictionary<Stat, float> deltaStat)
    {
        if (isProtesting)
        {
            location.GetUpkeep(deltaStat, this);

            foreach (var protester in protesters)
            {
                protester.GetUpkeep(deltaStat);
            }
        }        
    }

    public void Tick()
    {
        _inactivityTicks++;
    }
}
