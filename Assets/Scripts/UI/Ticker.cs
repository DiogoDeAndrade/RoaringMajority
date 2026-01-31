using NaughtyAttributes;
using System.Collections.Generic;
using TMPro;
using UC;
using UnityEngine;

public class Ticker : MonoBehaviour
{
    class News
    {
        public string   text;
        public float    duration;
        public int      count;
        public float    width;
    }

    [SerializeField] private TextMeshProUGUI    mainText;
    [SerializeField] private float              moveSpeed = 100.0f;
    [SerializeField] private float              baseX = 0.0f;

    private List<News>      news = new();
    private News            current;
    private CanvasGroup     canvasGroup;
    private RectTransform   textRT;

    private static Ticker instance;

    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        canvasGroup = GetComponent<CanvasGroup>();
        textRT = mainText.transform as RectTransform;
    }

    void Update()
    {
        if (current == null)
        {
            GetNextNewsItems();
        }
        else
        {
            textRT.anchoredPosition = textRT.anchoredPosition - Vector2.right * moveSpeed * Time.deltaTime;
            if (textRT.anchoredPosition.x < -current.width)
            {
                // Done with this one
                current = null;
            }
        }

        foreach (var n in news)
        {
            n.duration -= Time.deltaTime;
        }

        news.RemoveAll((n) => (n.duration < 0.0f) && (n != current));
    }

    void GetNextNewsItems()
    {
        if ((news == null) || (news.Count == 0))
        {
            current = null;
            canvasGroup.FadeOut(0.5f);
            return;
        }

        // 1) Find minimum count
        int minCount = int.MaxValue;
        for (int i = 0; i < news.Count; i++)
        {
            if (news[i].count < minCount)
            {
                minCount = news[i].count;
            }
        }

        // 2) Among those, find minimum duration
        float minDuration = float.MaxValue;
        for (int i = 0; i < news.Count; i++)
        {
            if (news[i].count == minCount && news[i].duration < minDuration)
            {
                minDuration = news[i].duration;
            }
        }

        // 3) Collect all ties (same min count + min duration), pick random
        // (avoid allocations if you care later; this is simple and fine for most cases)
        List<News> candidates = new();
        for (int i = 0; i < news.Count; i++)
        {
            if ((news[i].count == minCount) && Mathf.Approximately(news[i].duration, minDuration))
            {
                candidates.Add(news[i]);
            }
        }

        current = candidates.Random();
        current.count++;

        if (mainText != null)
        {
            textRT.anchoredPosition = new Vector2(baseX, 0);
            mainText.text = current.text;
            mainText.ForceMeshUpdate();
            current.width = mainText.textBounds.size.x;
        }

        canvasGroup.FadeIn(0.5f);
    }

    void _AddNews(string text, float duration)
    {
        news.Add(new News { text = text, duration = duration });
    }

    public static void AddNews(string text, float duration)
    {
        instance?._AddNews(text, duration);
    }
}
