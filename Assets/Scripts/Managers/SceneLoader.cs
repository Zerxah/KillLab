using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    [Header("Opcional UI")]
    [SerializeField] private CanvasGroup loadingGroup;
    [SerializeField] private UnityEngine.UI.Slider loadingBar;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (loadingGroup) loadingGroup.alpha = 0f;
    }

    public void LoadSceneAsyncByName(string sceneName)
    {
        StartCoroutine(LoadRoutine(sceneName));
    }

    IEnumerator LoadRoutine(string sceneName)
    {
        if (loadingGroup) loadingGroup.alpha = 1f;

        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        while (!op.isDone)
        {
            float p = Mathf.Clamp01(op.progress / 0.9f);
            if (loadingBar) loadingBar.value = p;

            if (op.progress >= 0.9f)
            {
                op.allowSceneActivation = true;
            }

            yield return null;
        }

        if (loadingGroup) loadingGroup.alpha = 0f;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

