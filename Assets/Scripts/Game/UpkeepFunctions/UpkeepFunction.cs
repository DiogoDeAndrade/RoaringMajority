using System;
using System.Collections.Generic;

public interface IUpkeepProvider
{

}

[Serializable]
public abstract class UpkeepFunction
{
    public abstract void RunUpkeep(Dictionary<Stat, float> deltaStat, IUpkeepProvider mainObject);
    public abstract float GetUpkeep(Stat stat, IUpkeepProvider mainObject);
}
