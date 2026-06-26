using UnityEngine;
using UnityEngine.SceneManagement;

public class DontDestroyMusic : MonoBehaviour
{
    private static DontDestroyMusic instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // kalau masuk level gameplay
        if (scene.name == "MainScene" || scene.name == "Level2")
        {
            Destroy(gameObject);
        }
    }
}