using NaughtyAttributes;
using UC;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClickToChangeScene : MonoBehaviour
{
    [Scene] public string sceneName;

    void Update()
    {
        if (Input.anyKeyDown)
        {
            FullscreenFader.FadeOut(0.5f, Color.black, () =>
            {
                SceneManager.LoadScene(sceneName);
            });
        }
    }
}
