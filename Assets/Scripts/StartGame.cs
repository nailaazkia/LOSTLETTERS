using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    public void StartButton()
    {
        string targetScene = GetLatestUnlockedLevelScene();
        Debug.Log("Melanjutkan permainan ke scene: " + targetScene);
        SceneManager.LoadScene(targetScene);
    }

    // Mendapatkan scene level tertinggi yang terbuka untuk melanjutkan permainan
    public static string GetLatestUnlockedLevelScene()
    {
        if (LostLettersJournalController.IsLevelCleared(4)) return "Level5";
        if (LostLettersJournalController.IsLevelCleared(3)) return "Level4";
        if (LostLettersJournalController.IsLevelCleared(2)) return "Level3";
        if (LostLettersJournalController.IsLevelCleared(1)) return "Level2";

        return "Level1"; // Default awal dari Level 1 jika belum ada progres/setelah reset
    }
}