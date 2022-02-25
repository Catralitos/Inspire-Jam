using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{

    public Button mouseButton,loadButton ,exitButton;

    private GameManager _gameManager;

    private void Start()
    {
        _gameManager = GameManager.Instance;
        mouseButton.onClick.AddListener(StartMouseGame);
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

    private void ExitGame()
    {
        Application.Quit();
    }

}