using TMPro;
using UC;
using UnityEngine;
using UnityEngine.UI;

public class Speech : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI    textElement;
    [SerializeField] private float              fadeTime = 0.1f;

    RectTransform   textRT;
    CanvasGroup     canvasGroup;
    float           timer;

    void Start()
    {
        textRT = textElement.GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0.0f;
    }

    private void Update()
    {
        if (canvasGroup.alpha >= 1.0f)
        { 
            timer -= Time.deltaTime;
            if (timer <= 0.0f)
            {
                canvasGroup.FadeOut(fadeTime);
            }
        }
    }

    public void Say(string text, float duration)
    {
        timer = duration;
        textElement.text = text;
        canvasGroup.FadeIn(fadeTime);

        LayoutRebuilder.ForceRebuildLayoutImmediate(textRT);
    }
}
