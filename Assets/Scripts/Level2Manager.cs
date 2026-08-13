using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Level2Manager : MonoBehaviour
{
    [Header("Slots")]
    public TextMeshProUGUI[] slots;

    [Header("UI Result")]
    public GameObject winPanel;
    public GameObject losePanel;

    [Header("Sound")]
    public ButtonSound buttonSound;
    public AudioSource bgmSource;

    private int currentIndex = 0;
    private string correctAnswer = "BEC";

    private LevelRewardController rewardController;
    private int hintUsageCount = 0;

    private void EnsurePanelsAssigned()
    {
        Transform canvas = transform.Find("Canvas");
        if (canvas == null && GameObject.Find("Canvas") != null) canvas = GameObject.Find("Canvas").transform;
        if (canvas == null && FindObjectOfType<Canvas>(true) != null) canvas = FindObjectOfType<Canvas>(true).transform;

        if (canvas != null)
        {
            if (winPanel == null)
            {
                Transform w = canvas.Find("Popup Benar");
                if (w == null) w = canvas.Find("winPanel");
                if (w == null) w = canvas.Find("WinPanel");
                if (w != null) winPanel = w.gameObject;
            }

            if (losePanel == null)
            {
                Transform l = canvas.Find("Popup Salah");
                if (l == null) l = canvas.Find("losePanel");
                if (l == null) l = canvas.Find("LosePanel");
                if (l != null) losePanel = l.gameObject;
            }
        }
    }

    void Awake()
    {
        EnsurePanelsAssigned();

        if (winPanel != null) winPanel.SetActive(false);
        if (losePanel != null) losePanel.SetActive(false);

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

        Time.timeScale = 1f;
    }

    void Start()
    {
        EnsurePanelsAssigned();

        if (winPanel != null && winPanel.GetComponent<PopupBlurOverlay>() == null) winPanel.AddComponent<PopupBlurOverlay>();
        if (losePanel != null && losePanel.GetComponent<PopupBlurOverlay>() == null) losePanel.AddComponent<PopupBlurOverlay>();

        if (winPanel != null) winPanel.SetActive(false);
        if (losePanel != null) losePanel.SetActive(false);

        // Tambahkan LevelRewardController
        rewardController = gameObject.AddComponent<LevelRewardController>();
    }

    public void AddLetter(string letter)
    {
        if (currentIndex >= slots.Length) return;

        slots[currentIndex].text = letter;
        currentIndex++;

        if (currentIndex == slots.Length)
        {
            CheckAnswer();
        }
    }

    void CheckAnswer()
    {
        string result = "";

        foreach (var slot in slots)
        {
            result += slot.text;
        }

        Debug.Log("Jawaban: " + result);

        if (result == correctAnswer)
        {
            Debug.Log("BENAR!");
            LostLettersJournalController.MarkLevelCleared(2);
            ShowWin();
        }
        else
        {
            Debug.Log("SALAH!");
            ShowLose();
        }
    }

    void ShowWin()
    {
        EnsurePanelsAssigned();
        if (bgmSource != null) bgmSource.Pause();

        if (buttonSound != null) buttonSound.PlayWinSound();

        // Hitung dan tampilkan reward (Koin & Bintang)
        if (rewardController != null)
        {
            LevelRewardResult reward = rewardController.CalculateReward();
            if (winPanel != null)
            {
                WinPopupController popup = winPanel.GetComponent<WinPopupController>();
                if (popup != null)
                {
                    popup.ShowResult(reward); // Menggunakan controller baru
                }
                else
                {
                    LevelRewardController.DisplayRewardOnPanel(winPanel, reward);
                    winPanel.SetActive(true);
                }
            }
        }
        else
        {
            if (winPanel != null) winPanel.SetActive(true);
        }
        Time.timeScale = 0f;
    }

    void ShowLose()
    {
        EnsurePanelsAssigned();
        if (bgmSource != null) bgmSource.Pause();

        if (buttonSound != null) buttonSound.PlayLoseSound();

        if (losePanel != null) losePanel.SetActive(true);
        Time.timeScale = 0f;

        // SAMA seperti level 1
        Invoke("ResetSlots", 1.5f);
    }

    public void ResetSlots()
    {
        foreach (var slot in slots)
        {
            slot.text = "";
        }

        currentIndex = 0;

        if (losePanel != null) losePanel.SetActive(false);
        if (winPanel != null) winPanel.SetActive(false);

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

        if (bgmSource != null) bgmSource.UnPause();

        Time.timeScale = 1f;
    }

    // =========================
    // BUTTONS
    // =========================

    public void NextLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Level3");
    }

    public void BackToLevelSelect()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("LevelSelect");
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void UseHint()
    {
        if (currentIndex < correctAnswer.Length)
        {
            string correctLetter = correctAnswer[currentIndex].ToString();
            AddLetter(correctLetter);

            if (rewardController != null)
            {
                rewardController.MarkHintUsed();
            }
        }
    }

    public void TryAgain()
    {
        ResetSlots();
    }
}