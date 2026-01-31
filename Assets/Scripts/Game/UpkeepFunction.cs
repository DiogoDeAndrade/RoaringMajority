using System;
using System.Collections.Generic;

[Serializable]
public abstract class UpkeepFunction
{
    public abstract void RunUpkeep(Dictionary<Stat, float> deltaStat, object mainObject);    
}
