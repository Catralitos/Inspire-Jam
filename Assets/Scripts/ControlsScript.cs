using Audio;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ControlsScript : MonoBehaviour
{
    public Button backButton;
    private AudioManager _audioManager;

    void Start()
    {
        _audioManager = GetComponent<AudioManager>();
        backButton.onClick.AddListener(BackToMainMenu);
    }

    void BackToMainMenu()
    {
        _audioManager.Play("ButtonClick");
        SceneManager.LoadScene(sceneName: "MainMenu");
    }
    
}
