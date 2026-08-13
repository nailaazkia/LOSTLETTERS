using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LostLettersJournalController : MonoBehaviour
{
    [Header("UI References (Bisa diassign di Inspector atau dibuat otomatis)")]
    public Button openJournalButton;
    public GameObject journalPanel;
    public TextMeshProUGUI journalContentTMP;
    public Text journalContentLegacy;
    public TextMeshProUGUI progressTextTMP;
    public Text progressTextLegacy;

    [Header("Graceful Sway & Float Animation Settings")]
    public bool enableGracefulSway = true;
    public float floatSpeed = 2.0f;       // Kecepatan melayang naik-turun
    public float floatAmplitude = 6.0f;   // Jarak melayang naik-turun (pixel)
    public float swaySpeed = 1.5f;        // Kecepatan kemiringan berayun
    public float swayAngle = 5.0f;        // Sudut kemiringan berayun (derajat)

    private Vector3 initialButtonPos;
    private Quaternion initialButtonRot;
    private Vector3 initialButtonScale = Vector3.one;
    private RectTransform buttonRectTransform;

    public static void MarkLevelCleared(int levelNumber)
    {
        PlayerPrefs.SetInt("Level" + levelNumber + "_Cleared", 1);
        PlayerPrefs.Save();
    }

    public static bool IsLevelCleared(int levelNumber)
    {
        return PlayerPrefs.GetInt("Level" + levelNumber + "_Cleared", 0) == 1;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void InitializeAutoInject()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
        // Coba injek untuk scene yang pertama kali dibuka (misal MainMenu)
        AutoInjectToSceneCanvas(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    private static void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        AutoInjectToSceneCanvas(scene.name);
    }

    private static void AutoInjectToSceneCanvas(string sceneName)
    {
        // HANYA injek sistem jurnal di MainMenu dan LevelSelect
        if (sceneName != "MainMenu" && sceneName != "LevelSelect") 
        {
            return;
        }

        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas != null && FindObjectOfType<LostLettersJournalController>() == null)
        {
            GameObject journalObj = new GameObject("LostLettersJournalSystem");
            journalObj.transform.SetParent(canvas.transform, false);
            journalObj.AddComponent<LostLettersJournalController>();
        }
    }

    void Awake()
    {
        SetupAutomaticUI();
    }

    void Start()
    {
        UpdateJournalText();
        CacheInitialButtonTransform();
    }

    private void CacheInitialButtonTransform()
    {
        if (openJournalButton != null)
        {
            buttonRectTransform = openJournalButton.GetComponent<RectTransform>();
            if (buttonRectTransform != null)
            {
                initialButtonPos = buttonRectTransform.anchoredPosition;
                initialButtonRot = buttonRectTransform.localRotation;
                initialButtonScale = buttonRectTransform.localScale;
            }
        }
    }

    void Update()
    {
        AnimateGracefulSway();
    }

    private void AnimateGracefulSway()
    {
        if (!enableGracefulSway || openJournalButton == null) return;
        if (journalPanel != null && journalPanel.activeSelf) return;

        if (buttonRectTransform == null)
        {
            CacheInitialButtonTransform();
            if (buttonRectTransform == null) return;
        }

        float time = Time.unscaledTime;

        // 1. Melayang naik-turun yang sangat lembut (Sine wave Y offset)
        float newY = initialButtonPos.y + Mathf.Sin(time * floatSpeed) * floatAmplitude;
        buttonRectTransform.anchoredPosition = new Vector2(initialButtonPos.x, newY);

        // 2. Kemiringan anggun berayun perlahan (Sine wave Z rotation)
        float zRot = Mathf.Sin(time * swaySpeed) * swayAngle;
        buttonRectTransform.localRotation = Quaternion.Euler(0, 0, zRot);

        // 3. Denyut lembut bernapas (Subtle scale pulse)
        float scalePulse = 1.0f + Mathf.Sin(time * floatSpeed * 0.8f) * 0.04f;
        buttonRectTransform.localScale = initialButtonScale * scalePulse;
    }

    public void SetupAutomaticUI()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas == null) canvas = FindObjectOfType<Canvas>();
        if (canvas == null) return;

        // Jika objek ini berada di luar Canvas (root Hierarchy), pindahkan otomatis ke dalam Canvas
        if (transform.parent != canvas.transform && GetComponentInParent<Canvas>() == null)
        {
            transform.SetParent(canvas.transform, false);
        }

        // 1. CARI ATAU BUAT TOMBOL OPEN JOURNAL (IKON SURAT)
        if (openJournalButton == null)
        {
            Transform existingBtn = transform.Find("JournalButton");
            if (existingBtn == null) existingBtn = canvas.transform.Find("JournalButton");

            if (existingBtn != null)
            {
                openJournalButton = existingBtn.GetComponent<Button>();
            }
            else
            {
                GameObject btnObj = new GameObject("JournalButton");
                btnObj.transform.SetParent(canvas.transform, false);

                Image btnBg = btnObj.AddComponent<Image>();
                btnBg.color = Color.white; // Warna netral untuk mendukung Sprite custom

                openJournalButton = btnObj.AddComponent<Button>();

                RectTransform rect = btnBg.rectTransform;
                rect.anchorMin = new Vector2(0, 1);
                rect.anchorMax = new Vector2(0, 1);
                rect.pivot = new Vector2(0, 1);
                
                // Geser ke bawah jika di LevelSelect agar tidak menabrak tombol 'Kembali'
                float startY = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "LevelSelect" ? -120f : -25f;
                rect.anchoredPosition = new Vector2(25, startY);
                rect.sizeDelta = new Vector2(100, 75);

                GameObject txtObj = new GameObject("Text");
                txtObj.transform.SetParent(btnObj.transform, false);
                TextMeshProUGUI tmp = txtObj.AddComponent<TextMeshProUGUI>();
                tmp.text = "✉ SURAT";
                tmp.fontSize = 16;
                tmp.fontStyle = FontStyles.Bold;
                tmp.alignment = TextAlignmentOptions.Center;
                tmp.color = new Color(0.25f, 0.20f, 0.15f, 1f);
                tmp.raycastTarget = false;

                RectTransform txtRect = tmp.rectTransform;
                txtRect.anchorMin = Vector2.zero;
                txtRect.anchorMax = Vector2.one;
                txtRect.offsetMin = Vector2.zero;
                txtRect.offsetMax = Vector2.zero;
            }
        }

        if (openJournalButton != null)
        {
            openJournalButton.onClick.RemoveListener(OpenJournal);
            openJournalButton.onClick.AddListener(OpenJournal);
            openJournalButton.transform.SetAsLastSibling();
        }

        // 2. CARI ATAU BUAT POPUP PANEL JOURNAL
        if (journalPanel == null)
        {
            Transform existingPanel = transform.Find("JournalPopupPanel");
            if (existingPanel == null) existingPanel = canvas.transform.Find("JournalPopupPanel");

            if (existingPanel != null)
            {
                journalPanel = existingPanel.gameObject;
            }
            else
            {
                // Panel Overlay semi-transparan
                GameObject panelObj = new GameObject("JournalPopupPanel");
                panelObj.transform.SetParent(canvas.transform, false);
                journalPanel = panelObj;

                Image overlay = panelObj.AddComponent<Image>();
                overlay.color = new Color(0, 0, 0, 0.7f);

                RectTransform pRect = overlay.rectTransform;
                pRect.anchorMin = Vector2.zero;
                pRect.anchorMax = Vector2.one;
                pRect.offsetMin = Vector2.zero;
                pRect.offsetMax = Vector2.zero;

                // Box Konten Kertas Surat
                GameObject boxObj = new GameObject("ParchmentBox");
                boxObj.transform.SetParent(panelObj.transform, false);

                Image boxBg = boxObj.AddComponent<Image>();
                boxBg.color = new Color(0.96f, 0.93f, 0.86f, 0.98f); // Krem pastel tebal

                RectTransform bRect = boxBg.rectTransform;
                bRect.anchorMin = new Vector2(0.5f, 0.5f);
                bRect.anchorMax = new Vector2(0.5f, 0.5f);
                bRect.pivot = new Vector2(0.5f, 0.5f);
                bRect.sizeDelta = new Vector2(650, 440);

                // Title Teks
                GameObject titleObj = new GameObject("Title");
                titleObj.transform.SetParent(boxObj.transform, false);
                TextMeshProUGUI titleTMP = titleObj.AddComponent<TextMeshProUGUI>();
                titleTMP.text = "📜 SURAT YANG HILANG";
                titleTMP.fontSize = 22;
                titleTMP.fontStyle = FontStyles.Bold;
                titleTMP.alignment = TextAlignmentOptions.Center;
                titleTMP.color = new Color(0.35f, 0.22f, 0.15f, 1f);

                RectTransform titleRect = titleTMP.rectTransform;
                titleRect.anchorMin = new Vector2(0, 1);
                titleRect.anchorMax = new Vector2(1, 1);
                titleRect.pivot = new Vector2(0.5f, 1);
                titleRect.anchoredPosition = new Vector2(0, -20);
                titleRect.sizeDelta = new Vector2(-40, 40);

                // Progress Text
                GameObject progObj = new GameObject("ProgressText");
                progObj.transform.SetParent(boxObj.transform, false);
                progressTextTMP = progObj.AddComponent<TextMeshProUGUI>();
                progressTextTMP.fontSize = 14;
                progressTextTMP.alignment = TextAlignmentOptions.Center;
                progressTextTMP.color = new Color(0.5f, 0.4f, 0.3f, 1f);

                RectTransform progRect = progressTextTMP.rectTransform;
                progRect.anchorMin = new Vector2(0, 1);
                progRect.anchorMax = new Vector2(1, 1);
                progRect.pivot = new Vector2(0.5f, 1);
                progRect.anchoredPosition = new Vector2(0, -55);
                progRect.sizeDelta = new Vector2(-40, 25);

                // Body Text Surat Content
                GameObject contentObj = new GameObject("ContentText");
                contentObj.transform.SetParent(boxObj.transform, false);
                journalContentTMP = contentObj.AddComponent<TextMeshProUGUI>();
                journalContentTMP.fontSize = 17;
                journalContentTMP.lineSpacing = 12;
                journalContentTMP.alignment = TextAlignmentOptions.Left;
                journalContentTMP.color = new Color(0.20f, 0.18f, 0.16f, 1f);
                journalContentTMP.richText = true;

                RectTransform contentRect = journalContentTMP.rectTransform;
                contentRect.anchorMin = new Vector2(0, 0);
                contentRect.anchorMax = new Vector2(1, 1);
                contentRect.offsetMin = new Vector2(40, 30);
                contentRect.offsetMax = new Vector2(-40, -90);

                // Tombol Close (X)
                GameObject closeBtnObj = new GameObject("CloseButton");
                closeBtnObj.transform.SetParent(boxObj.transform, false);

                Image closeBg = closeBtnObj.AddComponent<Image>();
                closeBg.color = new Color(0.85f, 0.3f, 0.3f, 1f);

                Button cBtn = closeBtnObj.AddComponent<Button>();
                cBtn.onClick.AddListener(CloseJournal);

                RectTransform cRect = closeBg.rectTransform;
                cRect.anchorMin = new Vector2(1, 1);
                cRect.anchorMax = new Vector2(1, 1);
                cRect.pivot = new Vector2(1, 1);
                cRect.anchoredPosition = new Vector2(-15, -15);
                cRect.sizeDelta = new Vector2(36, 36);

                GameObject cTxtObj = new GameObject("Text");
                cTxtObj.transform.SetParent(closeBtnObj.transform, false);
                TextMeshProUGUI cTmp = cTxtObj.AddComponent<TextMeshProUGUI>();
                cTmp.text = "X";
                cTmp.fontSize = 18;
                cTmp.fontStyle = FontStyles.Bold;
                cTmp.alignment = TextAlignmentOptions.Center;
                cTmp.color = Color.white;
                cTmp.raycastTarget = false;

                RectTransform cTxtRect = cTmp.rectTransform;
                cTxtRect.anchorMin = Vector2.zero;
                cTxtRect.anchorMax = Vector2.one;
                cTxtRect.offsetMin = Vector2.zero;
                cTxtRect.offsetMax = Vector2.zero;

                if (journalPanel.GetComponent<PopupBlurOverlay>() == null)
                {
                    journalPanel.AddComponent<PopupBlurOverlay>();
                }
            }
        }

        if (journalPanel != null)
        {
            WireUpCloseButtons();
            journalPanel.SetActive(false);
        }
    }

    private void WireUpCloseButtons()
    {
        if (journalPanel == null) return;

        // 1. Hubungkan seluruh Tombol Close yang ada di dalam journalPanel
        Button[] buttons = journalPanel.GetComponentsInChildren<Button>(true);
        foreach (Button btn in buttons)
        {
            if (btn != openJournalButton)
            {
                btn.onClick.RemoveListener(CloseJournal);
                btn.onClick.AddListener(CloseJournal);
            }
        }

        // 2. Buat background overlay hitam semi-transparan bisa diklik untuk menutup surat
        Image overlayImg = journalPanel.GetComponent<Image>();
        if (overlayImg != null)
        {
            overlayImg.raycastTarget = true;
            Button overlayBtn = journalPanel.GetComponent<Button>();
            if (overlayBtn == null) overlayBtn = journalPanel.AddComponent<Button>();
            overlayBtn.transition = Selectable.Transition.None;
            overlayBtn.onClick.RemoveListener(CloseJournal);
            overlayBtn.onClick.AddListener(CloseJournal);
        }

        // 3. Dorong Tombol Close (X) ke urutan z-index paling depan agar tidak tertutup teks atau gambar
        Transform closeBtnTrans = journalPanel.transform.Find("ParchmentBox/CloseButton");
        if (closeBtnTrans == null) closeBtnTrans = journalPanel.transform.Find("CloseButton");
        if (closeBtnTrans != null)
        {
            closeBtnTrans.SetAsLastSibling();
            Image cImg = closeBtnTrans.GetComponent<Image>();
            if (cImg != null) cImg.raycastTarget = true;

            Button cBtn = closeBtnTrans.GetComponent<Button>();
            if (cBtn != null)
            {
                cBtn.onClick.RemoveListener(CloseJournal);
                cBtn.onClick.AddListener(CloseJournal);
            }
        }
    }

    public void OpenJournal()
    {
        UpdateJournalText();
        if (journalPanel != null)
        {
            journalPanel.SetActive(true);
            journalPanel.transform.SetAsLastSibling();
            WireUpCloseButtons();
        }
    }

    public void CloseJournal()
    {
        if (journalPanel != null)
        {
            journalPanel.SetActive(false);
        }
    }

    private void EnsureReferencesAssigned()
    {
        if (journalPanel == null) return;

        // Cari Text Component Surat (TMP / Text biasa)
        if (journalContentTMP == null && journalContentLegacy == null)
        {
            Transform content = journalPanel.transform.Find("ParchmentBox/ContentText");
            if (content == null) content = journalPanel.transform.Find("ContentText");
            if (content != null)
            {
                journalContentTMP = content.GetComponent<TextMeshProUGUI>();
                if (journalContentTMP == null) journalContentLegacy = content.GetComponent<Text>();
            }

            if (journalContentTMP == null && journalContentLegacy == null)
            {
                TextMeshProUGUI[] tmps = journalPanel.GetComponentsInChildren<TextMeshProUGUI>(true);
                foreach (var t in tmps)
                {
                    if (t.gameObject.name.Contains("Content") || t.gameObject.name.Contains("Text") || t.gameObject.name.Contains("Body"))
                    {
                        journalContentTMP = t;
                        break;
                    }
                }
                if (journalContentTMP == null)
                {
                    Text[] txts = journalPanel.GetComponentsInChildren<Text>(true);
                    foreach (var t in txts)
                    {
                        if (t.gameObject.name.Contains("Content") || t.gameObject.name.Contains("Text") || t.gameObject.name.Contains("Body"))
                        {
                            journalContentLegacy = t;
                            break;
                        }
                    }
                }
            }
        }

        // Cari Progress Text Component (TMP / Text biasa)
        if (progressTextTMP == null && progressTextLegacy == null)
        {
            Transform prog = journalPanel.transform.Find("ParchmentBox/ProgressText");
            if (prog == null) prog = journalPanel.transform.Find("ProgressText");
            if (prog != null)
            {
                progressTextTMP = prog.GetComponent<TextMeshProUGUI>();
                if (progressTextTMP == null) progressTextLegacy = prog.GetComponent<Text>();
            }

            if (progressTextTMP == null && progressTextLegacy == null)
            {
                TextMeshProUGUI[] tmps = journalPanel.GetComponentsInChildren<TextMeshProUGUI>(true);
                foreach (var t in tmps)
                {
                    if (t.gameObject.name.Contains("Progress"))
                    {
                        progressTextTMP = t;
                        break;
                    }
                }
                if (progressTextTMP == null)
                {
                    Text[] txts = journalPanel.GetComponentsInChildren<Text>(true);
                    foreach (var t in txts)
                    {
                        if (t.gameObject.name.Contains("Progress"))
                        {
                            progressTextLegacy = t;
                            break;
                        }
                    }
                }
            }
        }
    }

    public void UpdateJournalText()
    {
        EnsureReferencesAssigned();

        bool l1 = IsLevelCleared(1);
        bool l2 = IsLevelCleared(2);
        bool l3 = IsLevelCleared(3);
        bool l4 = IsLevelCleared(4);
        bool l5 = IsLevelCleared(5);

        int clearedCount = (l1 ? 1 : 0) + (l2 ? 1 : 0) + (l3 ? 1 : 0) + (l4 ? 1 : 0) + (l5 ? 1 : 0);
        int percentage = clearedCount * 20;

        string goldStart = "<color=#D4AF37><b>"; // Emas bercahaya
        string goldEnd = "</b></color>";

        // Level 1: F E D G
        string line1 = l1
            ? $"{goldStart}F{goldEnd}irasatku m{goldStart}E{goldEnd}ngatakan tempat ini aneh. Suara {goldStart}D{goldEnd}etik jam terdengar {goldStart}G{goldEnd}anjil."
            : "<b>_</b>irasatku m<b>_</b>ngatakan tempat ini aneh. Suara <b>_</b>etik jam terdengar <b>_</b>anjil.";

        // Level 2: B E C
        string line2 = l2
            ? $"{goldStart}B{goldEnd}ayangannya m{goldStart}E{goldEnd}rampat di dinding {goldStart}C{goldEnd}ermin."
            : "<b>_</b>ayangannya m<b>_</b>rampat di dinding <b>_</b>ermin.";

        // Level 3: D E A
        string line3 = l3
            ? $"{goldStart}D{goldEnd}ua kepingan memori t{goldStart}E{goldEnd}lah ter{goldStart}A{goldEnd}ngkai."
            : "<b>_</b>ua kepingan memori t<b>_</b>lah ter<b>_</b>ngkai.";

        // Level 4: C G I
        string line4 = l4
            ? $"{goldStart}C{goldEnd}ahaya {goldStart}G{goldEnd}elap mulai mengh{goldStart}I{goldEnd}lang."
            : "<b>_</b>ahaya <b>_</b>elap mulai mengh<b>_</b>lang.";

        // Level 5: F C A E
        string line5 = l5
            ? $"{goldStart}F{goldEnd}ajar {goldStart}C{goldEnd}erah {goldStart}A{goldEnd}da di d{goldStart}E{goldEnd}pan mata..."
            : "<b>_</b>ajar <b>_</b>erah <b>_</b>da di d<b>_</b>pan mata...";

        string fullContent = $"{line1}\n\n{line2}\n\n{line3}\n\n{line4}\n\n{line5}";

        if (journalContentTMP != null) journalContentTMP.text = fullContent;
        if (journalContentLegacy != null) journalContentLegacy.text = fullContent;

        string progString = $"Progres Surat: {clearedCount} / 5 Terbuka ({percentage}%)";
        if (progressTextTMP != null) progressTextTMP.text = progString;
        if (progressTextLegacy != null) progressTextLegacy.text = progString;
    }
}
