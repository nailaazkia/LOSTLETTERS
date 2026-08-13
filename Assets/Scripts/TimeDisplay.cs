using UnityEngine;
using TMPro;

public class TimeDisplay : MonoBehaviour
{
    private TextMeshProUGUI timeText;

    void Awake()
    {
        timeText = GetComponent<TextMeshProUGUI>();
        if (timeText == null)
        {
            Debug.LogWarning("TimeDisplay membutuhkan komponen TextMeshProUGUI!");
        }
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
        LevelRewardController.OnTimeUpdated += UpdateTimeDisplay;
    }

    void OnDisable()
    {
        LevelRewardController.OnTimeUpdated -= UpdateTimeDisplay;
    }

    private void UpdateTimeDisplay(float timeLeft)
    {
        if (timeText != null)
        {
            int minutes = Mathf.FloorToInt(timeLeft / 60F);
            int seconds = Mathf.FloorToInt(timeLeft - minutes * 60);
            timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }
}
