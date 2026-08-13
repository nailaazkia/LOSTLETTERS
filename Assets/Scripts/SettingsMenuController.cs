using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuController : MonoBehaviour
{
    [Header("Panels")]
    public GameObject settingsPanel;
    public GameObject creditsPanel;

    [Header("Volume Control")]
    public Slider volumeSlider;

    void Start()
    {
        // Pastikan panel pengaturan & kredit memiliki efek blur latar belakang otomatis
        if (settingsPanel != null && settingsPanel.GetComponent<PopupBlurOverlay>() == null)
        {
            settingsPanel.AddComponent<PopupBlurOverlay>();
        }
        if (creditsPanel != null && creditsPanel.GetComponent<PopupBlurOverlay>() == null)
        {
            creditsPanel.AddComponent<PopupBlurOverlay>();
        }

        // Pastikan panel tertutup di awal
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (creditsPanel != null) creditsPanel.SetActive(false);

        // Load volume yang tersimpan dan hubungkan slider
        float savedVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        AudioListener.volume = savedVolume;

        if (volumeSlider != null)
        {
            volumeSlider.value = savedVolume;
            volumeSlider.onValueChanged.AddListener(OnVolumeSliderChanged);
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

    // Tombol PENGATURAN di Main Menu
    public void OpenSettings()
    {
        EnsureResetButtonWired();
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Settings Panel belum dimasukkan di Inspector!");
        }
    }

    // Tombol TUTUP / X pada panel Pengaturan
    public void CloseSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }

    // Tombol KREDIT pada panel Pengaturan
    public void OpenCredits()
    {
        if (creditsPanel != null)
        {
            creditsPanel.SetActive(true);
            if (settingsPanel != null) settingsPanel.SetActive(false); // Sembunyikan panel pengaturan saat kredit terbuka
        }
        else
        {
            Debug.LogWarning("Credits Panel belum dimasukkan di Inspector!");
        }
    }

    // Tombol TUTUP / KEMBALI pada panel Kredit
    public void CloseCredits()
    {
        if (creditsPanel != null)
        {
            creditsPanel.SetActive(false);
            if (settingsPanel != null) settingsPanel.SetActive(true); // Kembali ke panel pengaturan
        }
    }

    // Dipanggil otomatis saat slider digeser
    public void OnVolumeSliderChanged(float value)
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

    // Tombol KELUAR di Main Menu
    public void ExitGame()
    {
        Debug.Log("Keluar dari game...");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
