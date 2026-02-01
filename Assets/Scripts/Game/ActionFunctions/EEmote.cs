using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UC;
using UnityEngine;

[Serializable]
[PolymorphicName("Emote")]
public class EEmote : ExecutionFunction
{
    [Serializable]
    struct Emote
    {
        public List<Sprite> sprites;
        public int emoteCount;
    }

    [SerializeField]
    private bool        randomProtester;
    [SerializeField, ShowIf(nameof(randomProtester))]
    private int         protesterCount = 1;
    [SerializeField]
    private List<Emote> emotes;

    public override bool Execute(IActionProvider mainObject)
    {
        if ((emotes != null) && (emotes.Count > 0))
        {
            if (randomProtester)
            {
                List<ProtesterData> protesterList = new(mainObject.GetLocation().protesters);
                for (int i = 0; i < protesterCount; i++)
                {
                    if ((protesterList == null) || (protesterList.Count == 0)) break;

                    var pData = protesterList.Random(false);
                    RunEmote(GameManager.instance.GetProtester(pData));
                }
            }
            else
            {
                RunEmote(mainObject.GetProtester());
            }
        }

        return true;
    }

    void RunEmote(Protester protester)
    {
        var emote = emotes.Random();
        if ((emote.sprites != null) && (emote.sprites.Count > 0) && (emote.emoteCount > 0))
        {
            protester?.Emote(emote.sprites, emote.emoteCount);
        }
    }
}
