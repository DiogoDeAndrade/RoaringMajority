using System;
using System.Collections.Generic;
using UnityEngine;

public class ProtesterData
{
    public ProtesterDef def;
    public LocationData location;
    
    public ProtesterData(ProtesterDef def, LocationData location)
    {
        this.def = def;
        this.location = location;
    }

    public void GetUpkeep(Dictionary<Stat, float> deltaStat)
    {
        def.GetUpkeep(deltaStat, this);
    }
}
