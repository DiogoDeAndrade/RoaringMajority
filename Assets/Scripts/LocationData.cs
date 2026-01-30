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
    private Dictionary<Stat, float> values;

    public event OnChangeStat onChangeStat;

    public bool isProtesting => state == LocationState.Protest;

    public LocationData(Location location)
    {
        this.location = location;
        values = new Dictionary<Stat, float>();
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
}
