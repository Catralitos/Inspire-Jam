using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
   public void LoseGame()
    {
        SceneManager.LoadScene(sceneName: "LoseDialogue");
    }
    public void WinGame()
    {
        SceneManager.LoadScene(sceneName: "WinDialogue");
    }
    public void WinGameTwice()
    {
        SceneManager.LoadScene(sceneName: "WinWinDialogue");
    }
    public void LoseGameTwice()
    {
        SceneManager.LoadScene(sceneName: "LoseLoseDialogue");
    }
    public void WinLoseGame()
    {
        SceneManager.LoadScene(sceneName: "WinLoseDialogue");
    }
    public void LoseWinLose()
    {
        SceneManager.LoadScene(sceneName: "LoseWinDialogue");
    }
}
