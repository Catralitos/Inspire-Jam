using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public void Dialogue1()
    {
        SceneManager.LoadScene(sceneName: "Dialogue1");
    }
    public void Dialogue2()
    {
        SceneManager.LoadScene(sceneName: "Dialogue2");
    }
    public void Dialogue3()
    {
        SceneManager.LoadScene(sceneName: "Dialogue3");
    }
    public void GameOver1()
    {
        SceneManager.LoadScene(sceneName: "GameOver1");
    }
    public void GameOver2()
    {
        SceneManager.LoadScene(sceneName: "GameOver2");
    }
    public void GameOver3()
    {
        SceneManager.LoadScene(sceneName: "GameOver3");
    }
}
