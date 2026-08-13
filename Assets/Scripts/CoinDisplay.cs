using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class CoinDisplay : MonoBehaviour
{
    private TextMeshProUGUI coinText;

    void Awake()
    {
        coinText = GetComponent<TextMeshProUGUI>();
    }

    void Start()
    {
        if (transform.parent != null)
        {
            RectTransform parentRt = transform.parent.GetComponent<RectTransform>();
            if (parentRt != null)
            {
                // Paksa posisi Z jadi 0 agar tidak tenggelam
                Vector3 pos = parentRt.localPosition;
                pos.z = 0;
                parentRt.localPosition = pos;

                // Paksa gambar jadi 100% terlihat
                UnityEngine.UI.Image img = transform.parent.GetComponent<UnityEngine.UI.Image>();
                if (img != null)
                {
                    img.enabled = true;
                    Color c = img.color;
                    c.a = 1f;
                    img.color = c;
                }
                
                // Pindahkan ke paling depan
                transform.parent.SetAsLastSibling();
            }
        }
    }

    void OnEnable()
    {
        CoinManager.OnCoinsChanged += UpdateCoinUI;
        CoinManager.OnCoinsDeducted += ShowDeductionText;
        // Update pertama kali saat aktif
        UpdateCoinUI(CoinManager.GetCoins());
    }

    void OnDisable()
    {
        CoinManager.OnCoinsChanged -= UpdateCoinUI;
        CoinManager.OnCoinsDeducted -= ShowDeductionText;
    }

    private void UpdateCoinUI(int totalCoins)
    {
        if (coinText != null)
        {
            coinText.text = totalCoins.ToString();
        }
    }

    private void ShowDeductionText(int amount)
    {
        StartCoroutine(AnimateDeductionText(amount));
    }

    private System.Collections.IEnumerator AnimateDeductionText(int amount)
    {
        // Buat objek teks baru
        GameObject textObj = new GameObject("DeductionText");
        textObj.transform.SetParent(transform, false);
        
        TextMeshProUGUI floatingText = textObj.AddComponent<TextMeshProUGUI>();
        floatingText.text = "-" + amount;
        floatingText.color = Color.red;
        floatingText.fontSize = coinText != null ? coinText.fontSize * 0.8f : 30f;
        floatingText.fontStyle = FontStyles.Bold;
        floatingText.alignment = TextAlignmentOptions.Center;

        RectTransform rt = floatingText.rectTransform;
        // Posisikan sedikit di bawah teks koin utama
        rt.anchoredPosition = new Vector2(0, -40f);
        
        float duration = 1.0f;
        float elapsed = 0f;
        Vector2 startPos = rt.anchoredPosition;
        // Gerak turun sedikit
        Vector2 endPos = startPos + new Vector2(0, -30f);
        Color startColor = floatingText.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime; // Gunakan unscaled agar jalan meskipun di-pause
            float t = elapsed / duration;
            
            rt.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            floatingText.color = Color.Lerp(startColor, endColor, t);
            
            yield return null;
        }

        Destroy(textObj);
    }
}
