using Audio;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public Button newGameButton, loadButton, controlsButton, exitButton, backButton;

    public GameObject mainScreen, tutorialScreen;

    private GameManager _gameManager;
    private AudioManager _audioManager;

    private void Start()
    {
        _audioManager = GetComponent<AudioManager>();
        _gameManager = GameManager.Instance;
        newGameButton.onClick.AddListener(NewGame);
        controlsButton.onClick.AddListener(ShowControls);
        loadButton.onClick.AddListener(LoadGame);
        exitButton.onClick.AddListener(ExitGame);
        backButton.onClick.AddListener(ShowMain);
    }

    private void NewGame()
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
        mainScreen.SetActive(false);
        tutorialScreen.SetActive(true);
    }

    private void ShowMain()
    {
        _audioManager.Play("ButtonClick");
        tutorialScreen.SetActive(false);
        mainScreen.SetActive(true);
    }

    private void ExitGame()
    {
        _audioManager.Play("ButtonClick");
        Application.Quit();
    }
}