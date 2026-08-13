using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        // Proteksi kunci level
        if (sceneName == "Level2" && !LostLettersJournalController.IsLevelCleared(1)) return;
        if (sceneName == "Level3" && !LostLettersJournalController.IsLevelCleared(2)) return;
        if (sceneName == "Level4" && !LostLettersJournalController.IsLevelCleared(3)) return;
        if (sceneName == "Level5" && !LostLettersJournalController.IsLevelCleared(4)) return;

        SceneManager.LoadScene(sceneName);
    }
}