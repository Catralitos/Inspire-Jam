using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ControlsScript : MonoBehaviour
{
    public Button backButton;

    void Start()
    {
        backButton.onClick.AddListener(BackToMainMenu);
    }

    void BackToMainMenu()
    {
        SceneManager.LoadScene(sceneName: "MainMenu");
    }
    
}
