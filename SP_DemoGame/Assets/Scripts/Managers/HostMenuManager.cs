using System.Net;
using System;
using System.Net.Sockets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HostMenuManager : MonoBehaviour
{
    public TMP_Text hostCodeDisplay;
    public TMP_Text playersConnectingDisplay;
    public Text playerScoresDisplay;

    public StartGame startGame;

    public int listenPort = 27100;

    private UdpClient udpClient;
    private bool running;

    // Start is called before the first frame update
    void Start()
    {
        GameObject gameSettingsObj = GameObject.Find("GameSettings");
        if (gameSettingsObj == null)
        {
            Debug.LogError("GameSettings not found. Did you start from the Startup scene?");
            return;
        }
        GameSettings g = gameSettingsObj.GetComponent<GameSettings>();
        if (g.scoreText == "NO SCORES YET")
        {
            g.numPlayers = 1;  // start with host playing
            playerScoresDisplay.text = "PLAYER 1 Ready!";
        }


        udpClient = new UdpClient(listenPort);
        udpClient.EnableBroadcast = true;

        running = true;
        ReceiveLoop();

        Debug.Log($"Listening for UDP broadcasts on port {listenPort}");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    async void ReceiveLoop()
    {
        while (running)
        {
            try
            {
                UdpReceiveResult result = await udpClient.ReceiveAsync();
                if (result == null)
                    continue;
                byte[] data = result.Buffer;
                IPEndPoint sender = result.RemoteEndPoint;
                if (data == null)
                    continue;
                // Print bytes
                Debug.Log($"Received {data.Length} bytes from {sender}");
                Debug.Log(BitConverter.ToString(data));
            }
            catch (ObjectDisposedException)
            {
                // Socket closed, safe to exit
                break;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
        }
    }

    void OnDestroy()
    {
        StopListening();
    }

    void StopListening()
    {
        running = false;
        udpClient?.Close();
    }

    public void SetHostCodeDisplay(int hostCode)
    {
        hostCodeDisplay.text = hostCode.ToString();
    }

    public void SetPlayersConnectingDisplay(string playersConnecting)
    {
        playersConnectingDisplay.text = playersConnecting;
    }

    // called from Start button
    public void StartGameAsHost()
    {
        StopListening();
        startGame.LoadMainGameplaySceneSP();
    }

    // called by Start button for now to simulate players joining - refactor this into AddPlayer and redirect the Start button back to LoadMainGameplaySceneSP after network API is ready
    public void AddPlayerOrStartGameIfFull(string playerName = null)
    {
        GameObject gameSettingsObj = GameObject.Find("GameSettings");
        if (gameSettingsObj == null)
        {
            Debug.LogError("GameSettings not found. Did you start from the Startup scene?");
            return;
        }

        GameSettings g = gameSettingsObj.GetComponent<GameSettings>();

        if (g.numPlayers == 4 )
        {
            // all full, start game now!
            startGame.LoadMainGameplaySceneSP();
            return;
        }

        ++g.numPlayers;

        string playerDisplayName = string.IsNullOrEmpty(playerName)? "PLAYER "+g.numPlayers.ToString() : playerName;

        playerScoresDisplay.text += "\n" + playerDisplayName + " Ready!";
    }

    public void UpdateNumberOfPlayers(int numberOfPlayers)
    {
        GameObject gameSettingsObj = GameObject.Find("GameSettings");
        if (gameSettingsObj == null)
        {
            Debug.LogError("GameSettings not found. Did you start from the Startup scene?");
            return;
        }

        GameSettings g = gameSettingsObj.GetComponent<GameSettings>();
        g.numPlayers = numberOfPlayers;
    }
}
