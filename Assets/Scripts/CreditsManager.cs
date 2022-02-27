using System.Collections;
using System.Collections.Generic;
using Audio;
using UnityEngine;
using UnityEngine.UI;

public class CreditsManager : MonoBehaviour
{
    public Button backButton, exitButton;
    
    private GameManager _gameManager;
    private AudioManager _audioManager;
    
    // Start is called before the first frame update
    private void Start()
    {
        _audioManager = GetComponent<AudioManager>();
        _gameManager = GameManager.Instance;
        backButton.onClick.AddListener(LoadTitle);
        exitButton.onClick.AddListener(ExitGame);
        _audioManager.Play("MenuMusic");
    }

    private void LoadTitle()
    {
        _audioManager.Play("ButtonClick");
        _gameManager.LoadNextScene("MainMenu");
    }

    private void ExitGame()
    {
        _audioManager.Play("ButtonClick");
        Application.Quit();
    }
   
}
