using NaughtyAttributes;
using UC;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuControl : MonoBehaviour
{
    [SerializeField,Scene] private string   sceneName;
    [SerializeField] private CanvasGroup    menuGroup;
    [SerializeField] private CanvasGroup    creditsGroup;

    public void StartGame()
    {
        FullscreenFader.FadeOut(0.5f, Color.black, () =>
        {
            SceneManager.LoadScene(sceneName);
        });
    }

    public void Credits()
    {
        menuGroup.FadeOut(0.5f);
        creditsGroup.FadeIn(0.5f);

        var creditsElement = creditsGroup.GetComponentInChildren<BigTextScroll>();
        creditsElement.Reset();
        creditsElement.onEndScroll += CreditsElement_onEndScroll;
    }

    private void CreditsElement_onEndScroll()
    {
        creditsGroup.FadeOut(0.5f);
        menuGroup.FadeIn(0.5f);

        var creditsElement = creditsGroup.GetComponentInChildren<BigTextScroll>();
        creditsElement.onEndScroll -= CreditsElement_onEndScroll;
    }

    public void Quit()
    {
        FullscreenFader.FadeOut(0.5f, Color.black, () =>
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        });
    }
}
