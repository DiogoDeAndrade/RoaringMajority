using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UC;
using UnityEngine;
using UnityEngine.Rendering;

[Serializable]
[PolymorphicName("Say")]
public class ESay : ExecutionFunction
{
    public enum Source { Inline, CauseShouts };

    [SerializeField]
    private Source          source = Source.Inline;
    [SerializeField]
    private bool            randomProtester;
    [SerializeField, ShowIf(nameof(randomProtester))]
    private int             protesterCount = 1;
    [SerializeField, ShowIf(nameof(isInline))]
    private StringProbList  sentences;
    [SerializeField]
    private float           textDuration = 2.0f;

    bool isInline => source == Source.Inline;

    public override bool Execute(IActionProvider mainObject)
    {
        if (((sentences != null) && (sentences.Count > 0)) || (source != Source.Inline)) 
        {
            var translator = new Dictionary<string, string>
            {
                ["{LOCATION_NAME}"] = mainObject.GetLocation().location.newsName,
            };

            if (randomProtester)
            {
                List<ProtesterData> protesterList = new(mainObject.GetLocation().protesters);
                for (int i = 0; i < protesterCount; i++)
                {
                    if ((protesterList == null) || (protesterList.Count == 0)) break;

                    var pData = protesterList.Random(false);
                    Say(GameManager.instance.GetProtester(pData), translator);
                }
            }
            else
            {
                Say(mainObject.GetProtester(), translator);
            }
        }

        return true;
    }

    void Say(Protester protester, Dictionary<string, string> translator)
    {
        string text = "";
        switch (source)
        {
            case Source.Inline:
                text = sentences.Get();
                break;
            case Source.CauseShouts:
                {
                    var cause = GameManager.instance.GetCause();
                    text = cause.GetRandomShout(true);
                }
                break;
            default:
                break;
        }        

        if (!string.IsNullOrEmpty(text))
        {
            text = text.FindReplace(translator);

            protester?.Say(text, textDuration);
        }
    }

    public override bool isStatDelta(IActionProvider mainObject)
    {
        return false;
    }

    public override string GetTooltip(IActionProvider mainObject)
    {
        return null;
    }
}
