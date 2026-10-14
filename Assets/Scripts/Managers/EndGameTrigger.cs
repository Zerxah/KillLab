using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameTrigger : MonoBehaviour
{
    [SerializeField] private string menuSceneName = "L_MainMenu";
    [SerializeField] private float delay = 0.5f;

    bool used;

    void OnTriggerEnter(Collider other)
    {
        if (used) return;
        if (!other.CompareTag("Player")) return;

        used = true;
        Invoke(nameof(LoadMenu), delay);
    }

    void LoadMenu()
    {
        SceneManager.LoadScene(menuSceneName);
    }
}

