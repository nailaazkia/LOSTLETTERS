using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

[ExecuteAlways]
public class LevelSelectController : MonoBehaviour
{
    [Header("Visual Settings")]
    public Color unlockedTextColor = new Color(0.15f, 0.15f, 0.15f, 1f);
    public Color lockedTextColor = new Color(0.4f, 0.4f, 0.4f, 0.35f);
    public Sprite customLockSprite;

    private bool hasUpdated = false;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void AutoInjectToLevelSelect()
    {
        EnsureSystemExists();
    }

    public static void EnsureSystemExists()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "LevelSelect")
        {
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas != null)
            {
                LevelSelectController existing = canvas.GetComponentInChildren<LevelSelectController>(true);
                if (existing == null)
                {
                    GameObject obj = new GameObject("LevelSelectLockSystem");
                    obj.transform.SetParent(canvas.transform, false);
                    existing = obj.AddComponent<LevelSelectController>();
                }
                existing.UpdateLevelButtonsState();
            }
        }
    }

    void Awake()
    {
        UpdateLevelButtonsState();
    }

    void Start()
    {
        UpdateLevelButtonsState();
    }

    void OnEnable()
    {
        UpdateLevelButtonsState();
    }

    void Update()
    {
        if (!hasUpdated)
        {
            UpdateLevelButtonsState();
            hasUpdated = true;
        }
    }

    public void UpdateLevelButtonsState()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas == null) canvas = FindObjectOfType<Canvas>();
        if (canvas == null) return;

        // Level 1 selalu terbuka
        SetButtonState(canvas, 1, true);

        // Level 2 terbuka jika Level 1 selesai
        bool l1 = LostLettersJournalController.IsLevelCleared(1);
        SetButtonState(canvas, 2, l1);

        // Level 3 terbuka jika Level 2 selesai
        bool l2 = LostLettersJournalController.IsLevelCleared(2);
        SetButtonState(canvas, 3, l2);

        // Level 4 terbuka jika Level 3 selesai
        bool l3 = LostLettersJournalController.IsLevelCleared(3);
        SetButtonState(canvas, 4, l3);

        // Level 5 terbuka jika Level 4 selesai
        bool l4 = LostLettersJournalController.IsLevelCleared(4);
        SetButtonState(canvas, 5, l4);
    }

    private void SetButtonState(Canvas canvas, int levelNum, bool isUnlocked)
    {
        string[] possibleNames = new string[]
        {
            "Level_" + levelNum,
            "Level" + levelNum,
            "Button_Level" + levelNum,
            "ButtonLevel" + levelNum
        };

        Button btn = null;
        foreach (string name in possibleNames)
        {
            Transform t = canvas.transform.Find(name);
            if (t == null) t = transform.Find(name);
            if (t != null)
            {
                btn = t.GetComponent<Button>();
                if (btn != null) break;
            }
        }

        if (btn == null)
        {
            Button[] allBtns = canvas.GetComponentsInChildren<Button>(true);
            foreach (var b in allBtns)
            {
                string n = b.gameObject.name.ToLower();
                if (n.Contains("level") && n.Contains(levelNum.ToString()))
                {
                    btn = b;
                    break;
                }
            }
        }

        if (btn == null) return;

        // 1. Matikan interaktivitas tombol jika belum terbuka
        btn.interactable = isUnlocked;

        // 2. Ubah warna teks angka (1, 2, 3, 4, 5)
        TextMeshProUGUI tmpText = btn.GetComponentInChildren<TextMeshProUGUI>(true);
        if (tmpText != null)
        {
            tmpText.color = isUnlocked ? unlockedTextColor : lockedTextColor;
        }

        // 3. Tampilkan/Sembunyikan Ikon/Gambar Gembok di atas tombol level
        Transform lockIcon = btn.transform.Find("LockOverlay");
        if (lockIcon == null) lockIcon = btn.transform.Find("kunci_0");
        if (lockIcon == null) lockIcon = btn.transform.Find("Lock");
        if (lockIcon == null) lockIcon = btn.transform.Find("Gembok");

        if (!isUnlocked)
        {
            if (lockIcon == null)
            {
                GameObject lockObj = new GameObject("LockOverlay");
                lockObj.transform.SetParent(btn.transform, false);
                lockIcon = lockObj.transform;
            }

            lockIcon.gameObject.SetActive(true);
            lockIcon.SetAsLastSibling();

            if (customLockSprite != null)
            {
                // Hapus komponen Text (TMP) lama jika sebelumnya terlanjur dibuat
                TextMeshProUGUI oldTmp = lockIcon.GetComponent<TextMeshProUGUI>();
                if (oldTmp != null)
                {
                    if (Application.isPlaying) Destroy(oldTmp);
                    else DestroyImmediate(oldTmp);
                }

                Image lockImg = lockIcon.GetComponent<Image>();
                if (lockImg == null) lockImg = lockIcon.gameObject.AddComponent<Image>();

                lockImg.sprite = customLockSprite;
                lockImg.color = Color.white;
                lockImg.preserveAspect = true;
                lockImg.raycastTarget = false;

                RectTransform rect = lockImg.rectTransform;
                rect.anchorMin = new Vector2(0.5f, 0.5f);
                rect.anchorMax = new Vector2(0.5f, 0.5f);
                rect.pivot = new Vector2(0.5f, 0.5f);
                rect.anchoredPosition = Vector2.zero;
                rect.sizeDelta = new Vector2(55, 55); // Ukuran proporsional di atas awan
            }
            else
            {
                Image oldImg = lockIcon.GetComponent<Image>();
                if (oldImg != null)
                {
                    if (Application.isPlaying) Destroy(oldImg);
                    else DestroyImmediate(oldImg);
                }

                TextMeshProUGUI lockTmp = lockIcon.GetComponent<TextMeshProUGUI>();
                if (lockTmp == null) lockTmp = lockIcon.gameObject.AddComponent<TextMeshProUGUI>();

                lockTmp.text = "🔒";
                lockTmp.fontSize = 42;
                lockTmp.alignment = TextAlignmentOptions.Center;
                lockTmp.color = new Color(1f, 1f, 1f, 0.95f);
                lockTmp.raycastTarget = false;

                RectTransform rect = lockTmp.rectTransform;
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
                rect.offsetMin = Vector2.zero;
                rect.offsetMax = Vector2.zero;
            }
        }
        else
        {
            if (lockIcon != null)
            {
                lockIcon.gameObject.SetActive(false);
            }
        }
    }
}
