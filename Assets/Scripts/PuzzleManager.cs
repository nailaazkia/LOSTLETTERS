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

    void Start()
    {
        // pastikan popup mati di awal
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