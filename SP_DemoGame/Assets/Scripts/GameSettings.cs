using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    public int numPlayers = 4;
    public string scoreText = "NO SCORES YET";
    public string homeSceneName = "MainMenu";

    private void Start()
    {
        DontDestroyOnLoad(this);
    }
}
