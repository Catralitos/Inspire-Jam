using Audio;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{

    public Button mouseButton,loadButton ,controlsButton ,exitButton;

    private GameManager _gameManager;
    private AudioManager _audioManager;

    private void Start()
    {
        _audioManager = AudioManager.Instance;
        _gameManager = GameManager.Instance;
        mouseButton.onClick.AddListener(StartMouseGame);
        controlsButton.onClick.AddListener(ShowControls);
        loadButton.onClick.AddListener(LoadGame);
        exitButton.onClick.AddListener(ExitGame);
    }

    private void StartMouseGame()
    {
        _audioManager.Play("ButtonClick");
        _gameManager.LoadNextScene("PrologueDialogue");
    }

    private void LoadGame()
    {
        string scene = _gameManager.LoadSceneData();
        _audioManager.Play("ButtonClick");
        _gameManager.LoadNextScene(scene);
    }

    private void ShowControls()
    {
        _audioManager.Play("ButtonClick");
        _gameManager.LoadNextScene("Controls");
    }

    private void ExitGame()
    {
        _audioManager.Play("ButtonClick");
        Application.Quit();
    }

}