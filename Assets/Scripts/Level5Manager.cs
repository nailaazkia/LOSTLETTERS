using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class Level5Manager : MonoBehaviour
{
    [Header("Answer Slots")]
    public TextMeshProUGUI[] slots;

    [Header("Panels")]
    public GameObject losePanel;

    [Header("Door Buttons")]
    public Button pintuKiriButton;
    public Button pintuKananButton;

    [Header("Door Objects")]
    public RectTransform pintuKiri;
    public RectTransform pintuKanan;

    [Header("Ending Effects")]
    public RectTransform glowingLight; 
    public Image whiteFlash;           
    public AudioSource backgroundMusic; 

    [Header("Scene Navigation (Isi nama scene di sini)")]
    public string mainMenuSceneName = "MainMenu";
    public string levelSelectSceneName = "LevelSelect";

    private int currentIndex = 0;
    private string correctAnswer = "FCAE";
    private bool answerCorrect = false;
    private bool doorOpened = false;

    void Start()
    {
        losePanel.SetActive(false);

        if (glowingLight != null)
        {
            glowingLight.localScale = Vector3.zero;
        }

        if (whiteFlash != null)
        {
            Color c = whiteFlash.color;
            c.a = 0f;
            whiteFlash.color = c;
        }
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
            answerCorrect = true;
            Debug.Log("BENAR");
        }
        else
        {
            Debug.Log("SALAH");
            ShowLose();
        }
    }

    void ShowLose()
    {
        losePanel.SetActive(true);
    }

    public void OpenDoor()
    {
        if (!answerCorrect) 
        {
            Debug.Log("Pintu masih terkunci!");
            return; 
        }

        if (doorOpened) return;
        doorOpened = true;

        StartCoroutine(OpenDoorSequence());
    }

    IEnumerator OpenDoorSequence()
    {
        float duration = 2.5f; 

        Quaternion leftStart = pintuKiri.localRotation;
        Quaternion rightStart = pintuKanan.localRotation;
        Quaternion leftTarget = Quaternion.Euler(0, 90f, 0); 
        Quaternion rightTarget = Quaternion.Euler(0, -90f, 0);

        Vector3 finalLightScale = new Vector3(50f, 50f, 1f); 
        float startVolume = backgroundMusic != null ? backgroundMusic.volume : 0f;

        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            pintuKiri.localRotation = Quaternion.Lerp(leftStart, leftTarget, t);
            pintuKanan.localRotation = Quaternion.Lerp(rightStart, rightTarget, t);

            if (glowingLight != null)
            {
                glowingLight.localScale = Vector3.Lerp(Vector3.zero, finalLightScale, t);
            }

            if (whiteFlash != null)
            {
                Color c = whiteFlash.color;
                c.a = Mathf.Lerp(0f, 1.2f, t); 
                whiteFlash.color = c;
            }

            if (backgroundMusic != null)
            {
                backgroundMusic.volume = Mathf.Lerp(startVolume, 0f, t);
            }

            yield return null;
        }

        if (whiteFlash != null)
        {
            Color finalC = whiteFlash.color;
            finalC.a = 1f;
            whiteFlash.color = finalC;
        }

        if (backgroundMusic != null)
        {
            backgroundMusic.Stop();
        }

        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("Ending");
    }

    // ==========================================
    // FUNGSI-FUNGSI UNTUK TOMBOL DI POP-UP
    // ==========================================

    // 1. Reset / Coba Lagi (Menutup pop-up & menghapus teks jawaban tanpa memuat ulang layar)
    public void ResetSlots()
    {
        foreach (var slot in slots)
        {
            slot.text = "";
        }
        currentIndex = 0;
        losePanel.SetActive(false);
    }

    // 2. Restart Level (Memuat ulang Level 5 dari awal banget)
    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // 3. Kembali ke Level Select
    public void GoToLevelSelect()
    {
        SceneManager.LoadScene(levelSelectSceneName);
    }

    // 4. Kembali ke Main Menu Awal
    public void GoToMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}