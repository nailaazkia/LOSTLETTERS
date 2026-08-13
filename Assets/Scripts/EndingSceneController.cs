using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndingSceneController : MonoBehaviour
{
    [Header("UI References (Bisa pakai TextMeshPro atau Text biasa)")]
    public TextMeshProUGUI storyTextTMP;    // Jika pakai TextMeshPro
    public Text storyTextLegacy;            // Jika pakai UI Text biasa (Legacy)
    public TextMeshProUGUI titleTextTMP;    // Teks judul ("THE END")
    public Text titleTextLegacy;            // Teks judul biasa ("THE END")

    [Header("Overlay & Buttons")]
    public Image whiteFlashOverlay;         // Panel putih penuh untuk transisi halus dari Level 5
    public Image blackFadeOverlay;          // Panel hitam penuh untuk transisi keluar ke Main Menu
    public Button nextButton;               // Tombol Lanjut
    public Button skipButton;               // Tombol Lewati / Skip ke Main Menu

    [Header("Narrative Settings")]
    [TextArea(3, 6)]
    public string[] storySlides = new string[]
    {
        "Cahaya putih menyilaukan memeluk seluruh tubuhku...\nPerlahan, anak tangga dan awan-awan pastel di sekelilingku mulai memudar.",
        "Satu per satu huruf dan kepingan puzzle yang telah kukumpulkan merangkai sebuah pesan...\nSurat-surat yang dulu hilang kini utuh kembali dalam ingatanku.",
        "Kelopak mataku perlahan terbuka.\nSuara detik jam dinding dan hangatnya sinar matahari pagi menyapaku di kamar nyata...\nAku telah terbangun dari dunia mimpi.",
        "Tak ada lagi surat yang hilang.\nSekarang, waktunya menulis lembaran baru di dunia nyata..."
    };

    [Range(0.01f, 0.1f)]
    public float typewriterSpeed = 0.04f;   // Kecepatan mengetik per karakter
    public float autoAdvanceDelay = 4.0f;   // Jeda otomatis berpindah ke narasi berikutnya (detik)
    public float lastSlideDelay = 6.0f;     // Jeda slide terakhir sebelum kembali ke Main Menu (detik)
    public string mainMenuSceneName = "MainMenu";

    private int currentSlideIndex = 0;
    private bool isTyping = false;
    private bool isTransitioning = false;
    private float lastClickTime = -1f;
    private Coroutine typingCoroutine;
    private Coroutine autoAdvanceCoroutine;

    private void SetNarrativeText(string content)
    {
        if (storyTextTMP != null) storyTextTMP.text = content;
        if (storyTextLegacy != null) storyTextLegacy.text = content;
    }

    private void SetTitleVisible(bool visible)
    {
        if (titleTextTMP != null) titleTextTMP.gameObject.SetActive(visible);
        if (titleTextLegacy != null) titleTextLegacy.gameObject.SetActive(visible);
    }

    void Awake()
    {
        Time.timeScale = 1f;

        Canvas canvas = FindObjectOfType<Canvas>();

        // 1. OTOMATIS CARI DAN PASANG WHITEFLASH DAN BLACKFADE JIKA KOSONG
        if (canvas != null)
        {
            if (whiteFlashOverlay == null && canvas.transform.Find("WhiteFlash") != null)
            {
                whiteFlashOverlay = canvas.transform.Find("WhiteFlash").GetComponent<Image>();
            }
            if (blackFadeOverlay == null && canvas.transform.Find("BlackFade") != null)
            {
                blackFadeOverlay = canvas.transform.Find("BlackFade").GetComponent<Image>();
            }
        }

        // 2. OTOMATIS CARI STORY TEXT JIKA KOSONG DI INSPECTOR
        if (storyTextTMP == null && storyTextLegacy == null && canvas != null)
        {
            Transform st = canvas.transform.Find("StoryText");
            if (st != null)
            {
                storyTextTMP = st.GetComponent<TextMeshProUGUI>();
                if (storyTextTMP == null) storyTextLegacy = st.GetComponent<Text>();
            }
        }

        // 3. OTOMATIS CARI TITLE TEXT ("THE END") JIKA KOSONG
        if (titleTextTMP == null && titleTextLegacy == null && canvas != null)
        {
            Transform tt = canvas.transform.Find("THE END");
            if (tt == null) tt = canvas.transform.Find("TitleText");
            if (tt != null)
            {
                titleTextTMP = tt.GetComponent<TextMeshProUGUI>();
                if (titleTextTMP == null) titleTextLegacy = tt.GetComponent<Text>();
            }
        }

        // Langsung sembunyikan THE END di urutan paling awal sebelum apapun
        SetTitleVisible(false);

        // Pastikan BlackFade mati dan transparan di awal
        if (blackFadeOverlay != null)
        {
            Color bc = blackFadeOverlay.color;
            bc.a = 0f;
            blackFadeOverlay.color = bc;
            blackFadeOverlay.gameObject.SetActive(false);
        }

        try
        {
            SetupAutomaticUI();
        }
        catch { }
    }

    private void SetupAutomaticUI()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null) return;

        // Buat Click Area di belakang agar klik di sembarang tempat layar pasti terdeteksi melalui EventSystem UI
        GameObject clickObj = null;
        Transform existingClick = canvas.transform.Find("ScreenClickArea");
        if (existingClick != null)
        {
            clickObj = existingClick.gameObject;
        }
        else
        {
            clickObj = new GameObject("ScreenClickArea");
            clickObj.transform.SetParent(canvas.transform, false);

            Image img = clickObj.AddComponent<Image>();
            img.color = new Color(0, 0, 0, 0); // Transparan 100%

            RectTransform rect = img.rectTransform;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            Button btn = clickObj.AddComponent<Button>();
            btn.transition = Selectable.Transition.None;
            btn.onClick.AddListener(OnNextClicked);
        }

        // Buat Tombol SKIP secara otomatis jika belum dipasang
        if (skipButton == null)
        {
            Transform existingSkip = canvas.transform.Find("AutoSkipButton");
            if (existingSkip != null)
            {
                skipButton = existingSkip.GetComponent<Button>();
            }
            else
            {
                GameObject skipObj = new GameObject("AutoSkipButton");
                skipObj.transform.SetParent(canvas.transform, false);

                Image skipBg = skipObj.AddComponent<Image>();
                skipBg.color = new Color(0.12f, 0.12f, 0.15f, 0.85f);
                skipBg.raycastTarget = true;

                Button sBtn = skipObj.AddComponent<Button>();
                skipButton = sBtn;
                sBtn.onClick.AddListener(SkipToMainMenu);

                RectTransform rect = skipBg.rectTransform;
                rect.anchorMin = new Vector2(1, 1);
                rect.anchorMax = new Vector2(1, 1);
                rect.pivot = new Vector2(1, 1);
                rect.anchoredPosition = new Vector2(-25, -25);
                rect.sizeDelta = new Vector2(120, 42);

                GameObject txtObj = new GameObject("Text");
                txtObj.transform.SetParent(skipObj.transform, false);
                Text legTxt = txtObj.AddComponent<Text>();
                legTxt.text = "SKIP >>";
                legTxt.fontSize = 16;
                legTxt.fontStyle = FontStyle.Bold;
                legTxt.alignment = TextAnchor.MiddleCenter;
                legTxt.color = Color.white;
                legTxt.raycastTarget = false;

                try
                {
                    Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                    if (font != null) legTxt.font = font;
                }
                catch { }

                RectTransform txtRect = legTxt.rectTransform;
                txtRect.anchorMin = Vector2.zero;
                txtRect.anchorMax = Vector2.one;
                txtRect.offsetMin = Vector2.zero;
                txtRect.offsetMax = Vector2.zero;
            }
        }

        if (clickObj != null) clickObj.transform.SetAsFirstSibling();
        if (skipButton != null) skipButton.transform.SetAsLastSibling();

        // 4. PASTIKAN GRAPHIC DEKORATIF TIDAK MENGHALANGI KLIK ScreenClickArea
        foreach (Graphic g in canvas.GetComponentsInChildren<Graphic>(true))
        {
            if (clickObj != null && (g.gameObject == clickObj || g.transform.IsChildOf(clickObj.transform)))
            {
                g.raycastTarget = true;
                continue;
            }
            if (skipButton != null && (g.gameObject == skipButton.gameObject || g.transform.IsChildOf(skipButton.transform)))
            {
                g.raycastTarget = true;
                continue;
            }
            if (nextButton != null && (g.gameObject == nextButton.gameObject || g.transform.IsChildOf(nextButton.transform)))
            {
                g.raycastTarget = true;
                continue;
            }

            g.raycastTarget = false;
        }
    }

    void Start()
    {
        if (nextButton != null && nextButton.onClick.GetPersistentEventCount() == 0) nextButton.onClick.AddListener(OnNextClicked);
        if (skipButton != null && skipButton.onClick.GetPersistentEventCount() == 0) skipButton.onClick.AddListener(SkipToMainMenu);

        StartCoroutine(StartEndingSequence());
    }

    IEnumerator StartEndingSequence()
    {
        isTransitioning = true;
        SetTitleVisible(false);
        SetNarrativeText("");

        // Kembalikan animasi transisi putih persis seperti awal
        if (whiteFlashOverlay != null)
        {
            whiteFlashOverlay.gameObject.SetActive(true);
            float elapsed = 0f;
            float fadeTime = 1.5f;
            while (elapsed < fadeTime)
            {
                elapsed += Time.unscaledDeltaTime;
                Color c = whiteFlashOverlay.color;
                c.a = Mathf.Lerp(1f, 0f, elapsed / fadeTime);
                whiteFlashOverlay.color = c;
                yield return null;
            }
            whiteFlashOverlay.gameObject.SetActive(false);
        }
        else
        {
            yield return new WaitForSecondsRealtime(0.3f);
        }

        isTransitioning = false;
        ShowSlide(0);
    }

    void Update()
    {
        // Proteksi try-catch agar tidak spam error jika Player Settings memakai New Input System Package
        try
        {
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
            {
                OnNextClicked();
                return;
            }
        }
        catch { }

#if ENABLE_INPUT_SYSTEM
        try
        {
            if (UnityEngine.InputSystem.Keyboard.current != null)
            {
                if (UnityEngine.InputSystem.Keyboard.current.spaceKey.wasPressedThisFrame ||
                    UnityEngine.InputSystem.Keyboard.current.enterKey.wasPressedThisFrame)
                {
                    OnNextClicked();
                }
            }
        }
        catch { }
#endif
    }

    public void OnNextClicked()
    {
        if (Time.unscaledTime - lastClickTime < 0.15f) return;
        lastClickTime = Time.unscaledTime;

        // 1. Jika diklik saat animasi putih (StartEndingSequence) masih berjalan, langsung matikan putihnya agar narasi pertama jalan!
        if (isTransitioning)
        {
            if (whiteFlashOverlay != null) whiteFlashOverlay.gameObject.SetActive(false);
            isTransitioning = false;
            ShowSlide(0);
            return;
        }

        // 2. Jika sedang mengetik, langsung munculkan seluruh kalimat slide saat ini seketika (Skip typing)
        if (isTyping)
        {
            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
            if (currentSlideIndex < storySlides.Length)
            {
                SetNarrativeText(storySlides[currentSlideIndex]);
            }
            isTyping = false;

            // Mulai timer auto-advance setelah teks ditampilkan penuh
            if (autoAdvanceCoroutine != null) StopCoroutine(autoAdvanceCoroutine);
            autoAdvanceCoroutine = StartCoroutine(AutoAdvanceRoutine());
            return;
        }

        // 3. Jika sedang di fase "THE END" (setelah seluruh narasi selesai), langsung transisi ke Main Menu
        if (currentSlideIndex >= storySlides.Length)
        {
            StartCoroutine(FinishAndReturnToMenu());
            return;
        }

        // 4. Lanjut ke slide berikutnya
        AdvanceToNextSlide();
    }

    private void AdvanceToNextSlide()
    {
        if (autoAdvanceCoroutine != null)
        {
            StopCoroutine(autoAdvanceCoroutine);
            autoAdvanceCoroutine = null;
        }

        currentSlideIndex++;
        if (currentSlideIndex < storySlides.Length)
        {
            ShowSlide(currentSlideIndex);
        }
        else
        {
            // Seluruh slide narasi selesai: Bersihkan teks narasi dan tampilkan "THE END" secara terpisah
            ShowEndTitlePhase();
        }
    }

    private void ShowEndTitlePhase()
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        if (autoAdvanceCoroutine != null) StopCoroutine(autoAdvanceCoroutine);

        SetNarrativeText("");   // Bersihkan teks narasi agar tidak menabrak THE END
        SetTitleVisible(true);  // Munculkan teks "THE END"

        autoAdvanceCoroutine = StartCoroutine(AutoAdvanceRoutine());
    }

    void ShowSlide(int index)
    {
        if ((storyTextTMP == null && storyTextLegacy == null) || index >= storySlides.Length)
        {
            return;
        }

        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        if (autoAdvanceCoroutine != null) StopCoroutine(autoAdvanceCoroutine);

        SetTitleVisible(false); // Pastikan THE END selalu mati saat narasi berjalan

        typingCoroutine = StartCoroutine(TypewriterEffect(storySlides[index]));
    }

    IEnumerator TypewriterEffect(string fullText)
    {
        isTyping = true;
        SetNarrativeText("");

        foreach (char c in fullText)
        {
            if (storyTextTMP != null) storyTextTMP.text += c;
            if (storyTextLegacy != null) storyTextLegacy.text += c;
            yield return new WaitForSecondsRealtime(typewriterSpeed);
        }

        isTyping = false;

        // Setelah selesai mengetik, mulai coroutine auto-advance
        if (autoAdvanceCoroutine != null) StopCoroutine(autoAdvanceCoroutine);
        autoAdvanceCoroutine = StartCoroutine(AutoAdvanceRoutine());
    }

    IEnumerator AutoAdvanceRoutine()
    {
        float delay = (currentSlideIndex >= storySlides.Length) ? lastSlideDelay : autoAdvanceDelay;
        yield return new WaitForSecondsRealtime(delay);

        if (currentSlideIndex >= storySlides.Length)
        {
            StartCoroutine(FinishAndReturnToMenu());
        }
        else
        {
            AdvanceToNextSlide();
        }
    }

    public void SkipToMainMenu()
    {
        if (Time.unscaledTime - lastClickTime < 0.2f) return;
        lastClickTime = Time.unscaledTime;

        if (autoAdvanceCoroutine != null) StopCoroutine(autoAdvanceCoroutine);
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);

        StartCoroutine(FinishAndReturnToMenu());
    }

    IEnumerator FinishAndReturnToMenu()
    {
        if (isTransitioning) yield break;
        isTransitioning = true;
        isTyping = false;

        if (autoAdvanceCoroutine != null) StopCoroutine(autoAdvanceCoroutine);
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);

        if (blackFadeOverlay != null)
        {
            blackFadeOverlay.gameObject.SetActive(true);
            blackFadeOverlay.raycastTarget = true;
            float elapsed = 0f;
            float fadeTime = 1.2f;
            while (elapsed < fadeTime)
            {
                elapsed += Time.unscaledDeltaTime;
                Color c = blackFadeOverlay.color;
                c.a = Mathf.Lerp(0f, 1f, elapsed / fadeTime);
                blackFadeOverlay.color = c;
                yield return null;
            }
        }
        else
        {
            yield return new WaitForSecondsRealtime(0.5f);
        }

        SceneManager.LoadScene(mainMenuSceneName);
    }
}
