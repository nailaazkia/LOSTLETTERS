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

    void Start()
    {
        winPanel.SetActive(false);
        losePanel.SetActive(false);
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
        bgmSource.Pause();

        buttonSound.PlayWinSound();

        winPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    void ShowLose()
    {
        bgmSource.Pause();

        buttonSound.PlayLoseSound();

        losePanel.SetActive(true);
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

        losePanel.SetActive(false);

        bgmSource.UnPause();

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

    public void TryAgain()
    {
        ResetSlots();
    }
}