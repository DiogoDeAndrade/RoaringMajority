using System;
using System.Collections.Generic;
using UC;
using UnityEngine;

[Serializable]
[PolymorphicName("Ticker Text")]
public class ETickerText : ExecutionFunction
{
    [SerializeField]
    private StringProbList  _tickerText;
    [SerializeField]
    private float           duration = 15.0f;

    public override bool Execute(IActionProvider mainObject)
    {
        if ((_tickerText != null) && (_tickerText.Count > 0))
        {
            var str = _tickerText.Get();
            if (!string.IsNullOrEmpty(str))
            {
                var translator = new Dictionary<string, string>
                {
                    ["{LOCATION_NAME}"] = mainObject.GetLocation().location.newsName,
                };
                str = str.FindReplace(translator);

                Ticker.AddNews(str, duration);
            }
        }
        return true;
    }
}
