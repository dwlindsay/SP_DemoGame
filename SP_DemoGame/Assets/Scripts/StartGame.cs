using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    public void LoadMainGameplayScene(int numPlayers)
    {
        GameObject gameSettingsObj = GameObject.Find("GameSettings");
        if (gameSettingsObj == null)
        {
            Debug.LogError("GameSettings not found. Did you start from the Startup scene?");
            return;
        }
        GameSettings g = gameSettingsObj.GetComponent<GameSettings>();
        g.numPlayers = numPlayers;
        g.scoreText = "NO SCORES YET";
        g.homeSceneName = "234Menu";
        SceneManager.LoadScene("CompleteMainScene");
    }

    public void LoadMainGameplaySceneSP()
    {
        GameObject gameSettingsObj = GameObject.Find("GameSettings");
        if (gameSettingsObj == null)
        {
            Debug.LogError("GameSettings not found. Did you start from the Startup scene?");
            return;
        }
        GameSettings g = gameSettingsObj.GetComponent<GameSettings>();
        //g.numPlayers = numPlayers;  // handled by HostMenuManager
        g.scoreText = "NO SCORES YET";
        g.homeSceneName = "SPHostMenu";
        SceneManager.LoadScene("CompleteMainSceneSP");
    }

    public void LoadScene(string sceneName)
    {
        GameObject gameSettingsObj = GameObject.Find("GameSettings");
        if (gameSettingsObj != null)
        {
            GameSettings gameSettings = gameSettingsObj.GetComponent<GameSettings>();
            gameSettings.scoreText = "NO SCORES YET";
        }
        SceneManager.LoadScene(sceneName);
    }
}
