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

    [Header("Door & Lock Visuals")]
    public GameObject lockIcon;

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

    private LevelRewardController rewardController;
    private int hintUsageCount = 0;

    private void EnsurePanelsAssigned()
    {
        Transform canvas = transform.Find("Canvas");
        if (canvas == null && GameObject.Find("Canvas") != null) canvas = GameObject.Find("Canvas").transform;
        if (canvas == null && FindObjectOfType<Canvas>(true) != null) canvas = FindObjectOfType<Canvas>(true).transform;

        if (canvas != null)
        {
            if (losePanel == null)
            {
                Transform l = canvas.Find("Popup Salah");
                if (l == null) l = canvas.Find("losePanel");
                if (l == null) l = canvas.Find("LosePanel");
                if (l != null) losePanel = l.gameObject;
            }
        }

        if (lockIcon == null)
        {
            GameObject k = GameObject.Find("kunci_0");
            if (k == null) k = GameObject.Find("kunci");
            if (k == null && transform.Find("kunci_0") != null) k = transform.Find("kunci_0").gameObject;
            if (k == null && GameObject.Find("PuzzleArea") != null && GameObject.Find("PuzzleArea").transform.Find("kunci_0") != null)
                k = GameObject.Find("PuzzleArea").transform.Find("kunci_0").gameObject;
            if (k == null && canvas != null && canvas.Find("kunci_0") != null)
                k = canvas.Find("kunci_0").gameObject;
            if (k != null) lockIcon = k;
        }
    }

    void Awake()
    {
        EnsurePanelsAssigned();
        if (lockIcon != null) lockIcon.SetActive(true);

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

        if (losePanel != null && losePanel.GetComponent<PopupBlurOverlay>() == null) losePanel.AddComponent<PopupBlurOverlay>();
        if (losePanel != null) losePanel.SetActive(false);

        if (pintuKiriButton != null) pintuKiriButton.onClick.AddListener(OpenDoor);
        if (pintuKananButton != null) pintuKananButton.onClick.AddListener(OpenDoor);

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

        // Tambahkan LevelRewardController
        rewardController = gameObject.AddComponent<LevelRewardController>();
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
            Debug.Log("BENAR - Kode berhasil dibuka!");
            LostLettersJournalController.MarkLevelCleared(5);
            if (lockIcon != null) lockIcon.SetActive(false);
            DoorController dc = FindObjectOfType<DoorController>();
            if (dc != null) dc.UnlockDoor();

            // Hitung dan berikan reward (tidak ditampilkan secara visual karena masuk ke cutscene)
            if (rewardController != null)
            {
                rewardController.CalculateReward();
            }
        }
        else
        {
            Debug.Log("SALAH");
            ShowLose();
        }
    }

    void ShowLose()
    {
        EnsurePanelsAssigned();
        if (losePanel != null) losePanel.SetActive(true);
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
        answerCorrect = false; // Reset status jawaban menjadi belum benar
        if (lockIcon != null) lockIcon.SetActive(true); // Munculkan kembali visual gembok
        DoorController dc = FindObjectOfType<DoorController>();
        if (dc != null) dc.LockDoor();

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
    }

    // 2. Restart Level (Memuat ulang Level 5 dari awal banget)
    public void RestartLevel()
    {
        Time.timeScale = 1f;
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
}