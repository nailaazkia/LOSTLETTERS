using UnityEngine;
using UnityEngine.SceneManagement;

public class DontDestroyMusic : MonoBehaviour
{
    private static DontDestroyMusic instance;
    public static DontDestroyMusic Instance => instance;
    private AudioSource audioSource;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            audioSource = GetComponent<AudioSource>();
            float savedVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
            SetVolume(savedVolume);

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        if (instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            instance = null;
        }
    }

    public void SetVolume(float volume)
    {
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        if (audioSource != null)
        {
            audioSource.volume = volume;
        }
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Kalau masuk ke scene apa pun selain MainMenu dan LevelSelect (misal: Level1, Level2, Level3, Level4, Level5, Ending, MainScene)
        // Hancurkan musik menu ini agar lagu tidak bertabrakan dengan lagu level tersebut!
        if (scene.name != "MainMenu" && scene.name != "LevelSelect")
        {
            Destroy(gameObject);
        }
    }
}