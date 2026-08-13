using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class WinPopupController : MonoBehaviour
{
    [Header("Stars UI (Harus berurutan 1 sampai 3)")]
    public GameObject[] stars; // Tarik objek Image/Teks bintang ke sini
    
    [Header("Reward Texts")]
    public TextMeshProUGUI baseCoinsText;
    public TextMeshProUGUI timeBonusText;
    public TextMeshProUGUI hintBonusText;
    public TextMeshProUGUI totalCoinsText;

    [Header("Animation Settings")]
    public float starDelay = 0.5f;

    [Header("Action Buttons (Opsional)")]
    public GameObject[] actionButtons; // Tarik BtnMenu, BtnNext, BtnHome ke sini

    public void ShowResult(LevelRewardResult result)
    {
        // Pastikan panel aktif
        gameObject.SetActive(true);

        // Matikan tombol agar bisa dianimasikan muncul
        if (actionButtons != null)
        {
            foreach (var btn in actionButtons)
            {
                if (btn != null) btn.SetActive(false);
            }
        }

        // Animasi pop-up untuk latar belakang panel pink
        Transform popupWindow = transform.Find("Background popup");
        if (popupWindow != null)
        {
            popupWindow.localScale = Vector3.zero;
            StartCoroutine(PopAnimation(popupWindow, 0.4f, 1.15f)); // Lebih lambat & membal halus
        }

        // Animasi Tombol Muncul (Bersamaan dengan panel)
        if (actionButtons != null)
        {
            foreach (var btn in actionButtons)
            {
                if (btn != null)
                {
                    btn.SetActive(true);
                    btn.transform.localScale = Vector3.zero;
                    StartCoroutine(PopAnimation(btn.transform, 0.4f, 1.15f));
                }
            }
        }

        // Matikan semua bintang di awal
        foreach (var star in stars)
        {
            if (star != null) star.SetActive(false);
        }

        // Tampilkan teks breakdown secara instan
        if (baseCoinsText != null) baseCoinsText.text = "Menang: +" + result.baseCoins + " Koin";
        
        if (timeBonusText != null)
        {
            if (result.gotTimeBonus)
            {
                timeBonusText.text = "Waktu Cepat: +" + result.timeBonusCoins + " Koin";
                timeBonusText.color = new Color(0.1f, 0.8f, 0.1f); // Hijau
            }
            else
            {
                timeBonusText.text = "Waktu Habis: +0 Koin";
                timeBonusText.color = Color.gray; // Abu-abu jika tidak dapat bonus
            }
        }

        if (hintBonusText != null)
        {
            if (result.gotHintBonus)
            {
                hintBonusText.text = "Tanpa Hint: +" + result.hintBonusCoins + " Koin";
                hintBonusText.color = new Color(0.1f, 0.8f, 0.1f);
            }
            else
            {
                hintBonusText.text = "Pakai Hint: +0 Koin";
                hintBonusText.color = Color.gray;
            }
        }

        if (totalCoinsText != null) 
        {
            totalCoinsText.text = "TOTAL: " + result.totalCoins + " KOIN";
        }

        // Jalankan animasi bintang
        StartCoroutine(AnimateStars(result.totalStars));
    }

    private IEnumerator AnimateStars(int starCount)
    {
        // Animasi Bintang
        for (int i = 0; i < starCount; i++)
        {
            if (i < stars.Length && stars[i] != null)
            {
                yield return new WaitForSecondsRealtime(starDelay);
                
                stars[i].SetActive(true);
                stars[i].transform.localScale = Vector3.zero;
                
                StartCoroutine(PopAnimation(stars[i].transform, 0.3f, 1.3f));
            }
        }
    }

    private IEnumerator PopAnimation(Transform target, float duration = 0.3f, float peakMultiplier = 1.3f)
    {
        float elapsed = 0f;
        
        // Membuat efek membesar sedikit (bouncy) sebelum kembali normal
        Vector3 startScale = Vector3.zero;
        Vector3 peakScale = Vector3.one * peakMultiplier; 
        Vector3 endScale = Vector3.one;

        // Fase 1: Membesar dari 0 ke Peak
        while (elapsed < duration / 2f)
        {
            elapsed += Time.unscaledDeltaTime;
            target.localScale = Vector3.Lerp(startScale, peakScale, elapsed / (duration / 2f));
            yield return null;
        }

        elapsed = 0f;
        
        // Fase 2: Mengecil dari Peak ke 1.0
        while (elapsed < duration / 2f)
        {
            elapsed += Time.unscaledDeltaTime;
            target.localScale = Vector3.Lerp(peakScale, endScale, elapsed / (duration / 2f));
            yield return null;
        }

        target.localScale = endScale;
    }
}
