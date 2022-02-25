using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]

public class SaveData 
{
    public string _scene;

    public SaveData(Scene scene)
    {
        _scene = scene.name;
    }
}
