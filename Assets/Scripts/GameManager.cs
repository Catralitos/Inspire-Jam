using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void LoadNextScene(string scene)
    {
        SceneManager.LoadScene(sceneName: scene);
    }

    public static void SaveScene(Scene scene)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/Scene.txt";
        FileStream stream = new FileStream(path, FileMode.Create);

        SaveData data = new SaveData(scene);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static SaveData LoadData()
    {
        string path = Application.persistentDataPath + "/Scene.txt";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            SaveData data = formatter.Deserialize(stream) as SaveData;
            stream.Close();
            return data;
        }
        else
        {
            Debug.LogError("Save file not found");
            return null;
        }
    }

    public void SaveScene()
    {
        SaveScene(SceneManager.GetActiveScene());
    }

    public string LoadSceneData()
    {
        return LoadData()._scene;
    }

}

