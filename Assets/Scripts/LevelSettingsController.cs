using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSettingsController : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject settingsPanel;

    [Header("Volume Control")]
    public Slider volumeSlider;

    [Header("Scene Settings")]
    public string mainMenuSceneName = "MainMenu";

    private void EnsureSettingsPanelAssigned()
    {
        if (settingsPanel == null)
        {
            Transform canvas = transform.Find("Canvas");
            if (canvas == null && GameObject.Find("Canvas") != null) canvas = GameObject.Find("Canvas").transform;
            if (canvas == null && FindObjectOfType<Canvas>(true) != null) canvas = FindObjectOfType<Canvas>(true).transform;

            if (canvas != null)
            {
                Transform found = canvas.Find("SettingsPanel");
                if (found == null) found = canvas.Find("settingsPanel");
                if (found != null) settingsPanel = found.gameObject;
            }
        }
    }

    void Awake()
    {
        EnsureSettingsPanelAssigned();

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }

        // Pastikan SEMUA popup ("Popup Salah", "Popup Benar", "SettingsPanel", "losePanel", "winPanel") di bawah Canvas tertutup seketika
        if (GameObject.Find("Canvas") != null)
        {
            Transform c = GameObject.Find("Canvas").transform;
            foreach (Transform child in c)
            {
                if (child.name.Contains("Popup Salah") || child.name.Contains("Popup Benar") || child.name.Contains("SettingsPanel") || child.name.Contains("losePanel") || child.name.Contains("winPanel"))
                {
                    child.gameObject.SetActive(false);
                }
            }
        }

        Time.timeScale = 1f; // Pastikan waktu berjalan normal (tidak beku) setiap kali scene diulang/dibuat
    }

    void Start()
    {
        EnsureSettingsPanelAssigned();

        // Pastikan pop up pengaturan memiliki efek blur latar belakang otomatis (PopupBlurOverlay)
        if (settingsPanel != null && settingsPanel.GetComponent<PopupBlurOverlay>() == null)
        {
            settingsPanel.AddComponent<PopupBlurOverlay>();
        }

        // Pastikan pop up tertutup di awal permainan
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }

        // Load volume yang tersimpan dan hubungkan slider
        float savedVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        AudioListener.volume = savedVolume;

        if (volumeSlider != null)
        {
            volumeSlider.value = savedVolume;
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }

        EnsureResetButtonWired();
    }

    private void EnsureResetButtonWired()
    {
        if (settingsPanel == null) return;
        Button[] btns = settingsPanel.GetComponentsInChildren<Button>(true);
        foreach (Button b in btns)
        {
            string n = b.gameObject.name.ToLower();
            if (n.Contains("reset"))
            {
                b.onClick.RemoveListener(ResetProgress);
                b.onClick.AddListener(ResetProgress);
            }
        }
    }

    // Tombol RESET GAME pada Pengaturan
    public void ResetProgress()
    {
        Debug.Log("Mereset seluruh progres game...");

        PlayerPrefs.DeleteKey("Level1_Cleared");
        PlayerPrefs.DeleteKey("Level2_Cleared");
        PlayerPrefs.DeleteKey("Level3_Cleared");
        PlayerPrefs.DeleteKey("Level4_Cleared");
        PlayerPrefs.DeleteKey("Level5_Cleared");
        PlayerPrefs.Save();

        // Reset koin juga
        CoinManager.ResetCoins();

        // Update tampilan Surat secara langsung
        LostLettersJournalController journal = FindObjectOfType<LostLettersJournalController>();
        if (journal != null)
        {
            journal.UpdateJournalText();
        }

        // Update tampilan Kunci Level secara langsung
        LevelSelectController levelSelect = FindObjectOfType<LevelSelectController>();
        if (levelSelect != null)
        {
            levelSelect.UpdateLevelButtonsState();
        }

        Debug.Log("Progres game & surat berhasil direset!");
    }

    // 1. Membuka pop up pengaturan (tombol gear ⚙️ di pojok atas)
    public void OpenSettings()
    {
        EnsureSettingsPanelAssigned();
        EnsureResetButtonWired();
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
            Time.timeScale = 0f; // Jeda waktu (pause) permainan saat pengaturan terbuka
        }
        else
        {
            Debug.LogWarning("Settings Panel belum dimasukkan di Inspector!");
        }
    }

    // 2. Menutup pop up pengaturan / Lanjutkan (tombol X / Lanjutkan)
    public void CloseSettings()
    {
        EnsureSettingsPanelAssigned();
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
            Time.timeScale = 1f; // Lanjutkan waktu permainan
        }
    }

    // 3. Mengubah volume secara real-time dari slider
    public void OnVolumeChanged(float value)
    {
        if (DontDestroyMusic.Instance != null)
        {
            DontDestroyMusic.Instance.SetVolume(value);
        }
        else
        {
            AudioListener.volume = value;
            PlayerPrefs.SetFloat("MusicVolume", value);
            PlayerPrefs.Save();
        }
    }

    // 4. Mengulang level saat ini (tombol Ulang Level / Retry)
    public void RestartLevel()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
        Time.timeScale = 1f; // PENTING: kembalikan timeScale sebelum load scene agar game tidak beku
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    // 5. Kembali ke menu utama (tombol Kembali ke Menu / Home)
    public void ReturnToMenu()
    {
        Time.timeScale = 1f; // PENTING: kembalikan timeScale sebelum load scene
        SceneManager.LoadScene(mainMenuSceneName);
    }

    // 6. Keluar dari game (tombol Keluar Game / Exit)
    public void ExitGame()
    {
        Time.timeScale = 1f;
        Debug.Log("Keluar dari game...");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
