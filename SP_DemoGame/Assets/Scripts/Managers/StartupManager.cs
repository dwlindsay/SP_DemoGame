using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartupManager : MonoBehaviour
{
    void Awake()
    {
        // load any bundles, etc. here
    }

    void Start()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
