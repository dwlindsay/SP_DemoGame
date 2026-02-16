using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScoresDisplayManager : MonoBehaviour
{
    void Start()
    {
        GameSettings gameSettings = GameObject.Find("GameSettings")?.GetComponent<GameSettings>();

        if (gameSettings == null)
            return;

        Text scoreDisplay = GetComponent<Text>();
        scoreDisplay.text = gameSettings.scoreText;
    }
}

