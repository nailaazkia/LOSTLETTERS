using UnityEngine;
using TMPro;

public class LevelRewardResult
{
    public int totalStars;
    public int baseCoins;
    public int timeBonusCoins;
    public int hintBonusCoins;
    public int totalCoins => baseCoins + timeBonusCoins + hintBonusCoins;
    public bool gotTimeBonus;
    public bool gotHintBonus;
}

public class LevelRewardController : MonoBehaviour
{
    [Header("Level Settings")]
    public float targetTimeSeconds = 300f; // 5 menit
    public float maxTimeSeconds = 480f;    // 8 menit
    
    private float timeLeft;
    private bool usedHint = false;
    private bool levelFinished = false;

    public static event System.Action<float> OnTimeUpdated;

    void Start()
    {
        timeLeft = maxTimeSeconds;
    }

    void Update()
    {
        if (!levelFinished && timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
            
            if (timeLeft <= 0)
            {
                timeLeft = 0;
                levelFinished = true;
                TriggerGameOver();
            }

            OnTimeUpdated?.Invoke(timeLeft);
        }
    }

    void TriggerGameOver()
    {
        Debug.Log("Waktu Habis! Game Over.");
        gameObject.SendMessage("ShowLose", SendMessageOptions.DontRequireReceiver);
    }

    public void MarkHintUsed()
    {
        usedHint = true;
    }

    public LevelRewardResult CalculateReward()
    {
        levelFinished = true;

        LevelRewardResult result = new LevelRewardResult();
        result.totalStars = 1;
        result.baseCoins = 80;
        
        float timeElapsed = maxTimeSeconds - timeLeft;
        result.gotTimeBonus = (timeElapsed <= targetTimeSeconds);
        if (result.gotTimeBonus)
        {
            result.totalStars++;
            result.timeBonusCoins = 15;
        }
        else
        {
            result.timeBonusCoins = 0;
        }

        result.gotHintBonus = !usedHint;
        if (result.gotHintBonus)
        {
            result.totalStars++;
            result.hintBonusCoins = 25;
        }
        else
        {
            result.hintBonusCoins = 0;
        }

        CoinManager.AddCoins(result.totalCoins);

        return result;
    }

    // Fungsi lama dibiarkan sementara agar tidak error
    public static void DisplayRewardOnPanel(GameObject winPanel, LevelRewardResult reward)
    {
        // Akan digantikan oleh WinPopupController
    }
}
