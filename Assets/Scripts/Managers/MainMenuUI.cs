using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    [Header("Scenes")]
    [SerializeField] private string gameplaySceneName = "L_Level";

    [Header("Panels")]
    [SerializeField] private GameObject panelMain;
    [SerializeField] private GameObject panelControls;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        ShowMain();
    }


    public void OnPlay()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (SceneLoader.Instance)
            SceneLoader.Instance.LoadSceneAsyncByName(gameplaySceneName);
    }

    public void OnControls()
    {
        ShowControls();
    }

    public void OnBackFromControls()
    {
        ShowMain();
    }

    public void OnQuit()
    {
        if (SceneLoader.Instance)
            SceneLoader.Instance.QuitGame();
    }

    void ShowMain()
    {
        if (panelMain) panelMain.SetActive(true);
        if (panelControls) panelControls.SetActive(false);
    }

    void ShowControls()
    {
        if (panelMain) panelMain.SetActive(false);
        if (panelControls) panelControls.SetActive(true);
    }
}
