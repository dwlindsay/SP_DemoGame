using System.Net;
using System;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;

public class SP_Listener : MonoBehaviour
{
    public int listenPort = 27100;  // public? mutable?

    // TODO: custom inspector ui to show connected status or number of players connected?

    private UdpClient udpClient;
    private bool running;  // could add a custom inspector ui to show listening status

    // Start is called before the first frame update
    void Start()
    {
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

    // TODO: GetNumPlayersConnected()? callback or broadcast event for devs that want
    // to know how many players there are and/or when players connect or disconnect?

    // TODO: StartListening()? for devs that want to manually start network activity?

    // call this if you need to manually stop network activity
    public void StopListening()
    {
        running = false;
        udpClient?.Close();
    }
}
