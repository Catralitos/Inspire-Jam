using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{

    public Button mouseButton,loadButton ,controlsButton ,exitButton;

    private GameManager _gameManager;

    private void Start()
    {
        _gameManager = GameManager.Instance;
        mouseButton.onClick.AddListener(StartMouseGame);
        controlsButton.onClick.AddListener(ShowControls);
        loadButton.onClick.AddListener(LoadGame);
        exitButton.onClick.AddListener(ExitGame);
    }

    private void StartMouseGame()
    {
        _gameManager.LoadNextScene("PrologueDialogue");
    }

    private void LoadGame()
    {
        string scene = _gameManager.LoadSceneData();
        _gameManager.LoadNextScene(scene);
    }

    private void ShowControls()
    {
        _gameManager.LoadNextScene("Controls");
    }

    private void ExitGame()
    {
        Application.Quit();
    }

}