using System.Collections;
using TMPro;
using UC;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class StatDeltaDisplay : MonoBehaviour
{
    [SerializeField] private float              displayDuration = 2.0f;  
    [SerializeField] private float              fadeDuration = 0.1f;
    [SerializeField] private float              speed = 10.0f;
    [SerializeField] private Sprite             iconPositive;
    [SerializeField] private Color              colorPositive = Color.white;
    [SerializeField] private Sprite             iconNegative;
    [SerializeField] private Color              colorNegative = Color.white;
    [SerializeField] private Image              imageElement;
    [SerializeField] private TextMeshProUGUI    textElement;

    private void Start()
    {
        StartCoroutine(MoveCR());
    }

    IEnumerator MoveCR()
    {
        var rt = transform as RectTransform;

        rt.Move(Vector3.up * displayDuration * speed, displayDuration).EaseFunction(Ease.Sqr);
        yield return new WaitForSeconds(displayDuration - fadeDuration);

        var canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.FadeOut(fadeDuration);

        yield return new WaitForSeconds(fadeDuration);

        Destroy(gameObject);
    }

    public void Set(Stat stat, float deltaValue)
    {
        textElement.color = stat.color;
        textElement.text = $"{Mathf.Abs(deltaValue):0}";
        if (deltaValue > 0.0f)
        {
            imageElement.sprite = iconPositive;
            imageElement.color = colorPositive;
        }
        else
        {
            imageElement.sprite = iconNegative;
            imageElement.color = colorNegative;
        }

        var rt = transform as RectTransform;
        LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
    }
}
