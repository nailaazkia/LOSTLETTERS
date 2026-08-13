using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PuzzleManager : MonoBehaviour
{
    [Header("Bars")]
    public RectTransform bar1, bar2, bar3, bar4, bar5, bar6;

    [Header("Slots")]
    public TextMeshProUGUI[] slots; // slot huruf

    [Header("UI Result")]
    public GameObject winPanel;
    public GameObject losePanel;

    [Header("Sound")]
    public ButtonSound buttonSound;
    public AudioSource bgmSource;

    private int currentIndex = 0;
    private string correctAnswer = "FEDG";

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

        // pastikan popup mati di awal
        if (winPanel != null) winPanel.SetActive(false);
        if (losePanel != null) losePanel.SetActive(false);
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

        if (result == correctAnswer)
        {
            Debug.Log("BENAR!");
            LostLettersJournalController.MarkLevelCleared(1);
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
        LostLettersJournalController.MarkLevelCleared(1);
        if (bgmSource != null) bgmSource.Pause();

        if (buttonSound != null) buttonSound.PlayWinSound();

        if (winPanel != null) winPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    void ShowLose()
    {
        EnsurePanelsAssigned();
        if (bgmSource != null) bgmSource.Pause();

        if (buttonSound != null) buttonSound.PlayLoseSound();

        if (losePanel != null) losePanel.SetActive(true);
        Time.timeScale = 0f;

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

    // tombol NEXT (di WinPanel)
    public void NextLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Level2");
    }

    // Back to level select
    public void BackToLevelSelect()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("LevelSelect");
    }

    // back to main menu
    public void BackToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    // tombol TRY AGAIN
    public void TryAgain()
    {
        ResetSlots();
    }
}