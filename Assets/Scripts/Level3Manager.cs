using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Level3Manager : MonoBehaviour
{
    [Header("Slots Jawaban")]
    public TextMeshProUGUI[] slots;

    [Header("Popup")]
    public GameObject winPanel;
    public GameObject losePanel;

    [Header("Sound")]
    public ButtonSound buttonSound;
    public AudioSource bgmSource;

    private int currentIndex = 0;

    private string correctAnswer = "DEA";

    void Start()
    {
        winPanel.SetActive(false);
        losePanel.SetActive(false);
    }

    public void AddLetter(string letter)
    {
        if (currentIndex >= slots.Length)
            return;

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
        if (bgmSource != null)
            bgmSource.Pause();

        if (buttonSound != null)
            buttonSound.PlayWinSound();

        winPanel.SetActive(true);

        Time.timeScale = 0f;
    }

    void ShowLose()
    {
        if (bgmSource != null)
            bgmSource.Pause();

        if (buttonSound != null)
            buttonSound.PlayLoseSound();

        losePanel.SetActive(true);

        Time.timeScale = 0f;
    }

    public void ResetSlots()
    {
        foreach (var slot in slots)
        {
            slot.text = "";
        }

        currentIndex = 0;

        if (bgmSource != null)
            bgmSource.UnPause();
    }

    public void TryAgain()
    {
        Time.timeScale = 1f;

        losePanel.SetActive(false);

        ResetSlots();
    }

    public void NextLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Level4");
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
}