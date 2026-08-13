using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class HintController : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI hintNumberText; // Angka di atas lampu
    
    [Header("Settings")]
    public int firstHintCost = 50;
    public int secondHintCost = 100;
    
    private int hintUsageCount = 0; // 0 = belum dipakai, 1 = dipakai 1x, 2 = dipakai 2x
    private Button hintButton;
    private TextMeshProUGUI errorText;

    void Awake()
    {
        hintButton = GetComponent<Button>();
        if (hintButton == null)
        {
            Debug.LogWarning("HintController harus dipasang di objek yang memiliki komponen Button!");
        }
        else
        {
            hintButton.onClick.AddListener(OnHintButtonClicked);
        }
        
        CreateErrorText();
    }

    void Start()
    {
        UpdateUI();
    }

    void CreateErrorText()
    {
        GameObject textObj = new GameObject("HintErrorText");
        textObj.transform.SetParent(transform, false);
        errorText = textObj.AddComponent<TextMeshProUGUI>();
        errorText.text = "Koin Tidak Cukup!";
        errorText.color = Color.red;
        errorText.fontSize = 20;
        errorText.fontStyle = FontStyles.Bold;
        errorText.alignment = TextAlignmentOptions.Center;
        
        RectTransform rt = errorText.rectTransform;
        rt.sizeDelta = new Vector2(250, 50);
        rt.anchoredPosition = new Vector2(-100, -60); // Geser ke kiri (-100) agar tidak terpotong tepi layar
        
        textObj.SetActive(false); // Sembunyikan di awal
    }

    public void OnHintButtonClicked()
    {
        if (hintUsageCount == 0)
        {
            // Beli Hint 1
            if (CoinManager.UseCoins(firstHintCost))
            {
                hintUsageCount = 1;
                UpdateUI();
                TriggerHintInLevel();
            }
            else
            {
                StartCoroutine(ShowErrorText());
            }
        }
        else if (hintUsageCount == 1)
        {
            // Beli Hint 2
            if (CoinManager.UseCoins(secondHintCost))
            {
                hintUsageCount = 2;
                UpdateUI();
                TriggerHintInLevel();
                
                // Matikan tombol setelah hint kedua
                if (hintButton != null) hintButton.interactable = false;
            }
            else
            {
                StartCoroutine(ShowErrorText());
            }
        }
    }

    IEnumerator ShowErrorText()
    {
        if (errorText != null)
        {
            errorText.gameObject.SetActive(true);
            yield return new WaitForSeconds(1.5f);
            errorText.gameObject.SetActive(false);
        }
    }

    private void UpdateUI()
    {
        if (hintNumberText != null)
        {
            if (hintUsageCount == 0) 
            {
                hintNumberText.text = ""; // Bisa dikosongkan atau diisi "1"
            }
            else if (hintUsageCount == 1) 
            {
                hintNumberText.text = "1";
            }
            else if (hintUsageCount == 2) 
            {
                hintNumberText.text = "MAX";
            }
        }
    }

    private void TriggerHintInLevel()
    {
        // Cari Level Manager yang sedang aktif dan panggil UseHint()
        if (FindObjectOfType<Level1Manager>() != null) FindObjectOfType<Level1Manager>().UseHint();
        else if (FindObjectOfType<Level2Manager>() != null) FindObjectOfType<Level2Manager>().UseHint();
        else if (FindObjectOfType<Level3Manager>() != null) FindObjectOfType<Level3Manager>().UseHint();
        else if (FindObjectOfType<Level4Manager>() != null) FindObjectOfType<Level4Manager>().UseHint();
        else if (FindObjectOfType<Level5Manager>() != null) FindObjectOfType<Level5Manager>().UseHint();
        else
        {
            Debug.LogWarning("Tidak ada Level Manager yang ditemukan untuk menggunakan hint!");
        }
    }
}
