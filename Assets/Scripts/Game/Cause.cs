using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Cause", menuName = "RM/Cause")]
public class Cause : ScriptableObject
{
    public string   displayName;
    public News     newsItems;
    public News     jokeItems;
    public News     startItems;

    [SerializeReference]
    public List<UpkeepFunction> upkeepFunctions;

    public void GetUpkeep(Dictionary<Stat, float> deltaStat, IUpkeepProvider provider)
    {
        foreach (var upkeepFunction in upkeepFunctions)
        {
            upkeepFunction.RunUpkeep(deltaStat, provider);
        }
    }
    public string GetNews(bool proSide, Dictionary<string, string> translator = null)
    {
        string txt = newsItems.GetRandom(proSide);
        if (translator != null)
        {
            foreach (var t in translator)
            {
                txt = txt.Replace(t.Key, t.Value);
            }
        }
        return txt;
    }

    public string GetJoke(Dictionary<string, string> translator = null)
    {
        string txt = jokeItems.GetRandom();
        if (translator != null)
        {
            foreach (var t in translator)
            {
                txt = txt.Replace(t.Key, t.Value);
            }
        }
        return txt;
    }

    public string GetStartText(Dictionary<string, string> translator = null)
    {
        string txt = startItems.GetRandom();
        if (translator != null)
        {
            foreach (var t in translator)
            {
                txt = txt.Replace(t.Key, t.Value);
            }
        }
        return txt;
    }
}
