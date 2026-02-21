using System.Net;
using System;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
using System.Collections.Generic;



[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PlayerDatagram
{
    public int PlayerId;

    public float Distance;
    public float Angle;

    // We store 4 booleans inside this single byte
    private byte _flags;

    public PlayerDatagram(int playerId, float dist, float angle, bool b1, bool b2, bool b3, bool b4)
    {
        PlayerId = playerId;
        Distance = dist;
        Angle = angle;
        _flags = 0;

        // Packing bits
        if (b1) _flags |= 1 << 0;
        if (b2) _flags |= 1 << 1;
        if (b3) _flags |= 1 << 2;
        if (b4) _flags |= 1 << 3;
    }

    // Helper methods to work with bools
    public void SetBool(int index, bool value)
    {
        if (value)
            _flags |= (byte)(1 << index); // Set bit
        else
            _flags &= (byte)~(1 << index); // Clear bit
    }
    public bool GetBool(int index) => (_flags & (1 << index)) != 0;
}


public class SP_Listener : MonoBehaviour
{
    public int listenPort = 27100;  // public? mutable?

    // TODO: custom inspector ui to show connected status or number of players connected?

    OrderedDictionary playerInfo = new();

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
                // convert to PlayerDatagram
                PlayerDatagram newPlayerMessage = ByteArrayToPlayerDatagram(data);
                
                playerInfo[newPlayerMessage.PlayerId.ToString()] = newPlayerMessage;
                Debug.Log("New count:" + playerInfo.Count);
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

    static PlayerDatagram ByteArrayToPlayerDatagram(byte[] data)
    {
        if (data.Length < Marshal.SizeOf<PlayerDatagram>())
            throw new ArgumentException("Data length is too short to convert to PlayerDatagram");

        GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);
        try
        {
            return Marshal.PtrToStructure<PlayerDatagram>(handle.AddrOfPinnedObject());
        }
        finally
        {
            handle.Free();
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

    public PlayerDatagram? GetPlayerInfo(int playerId)
    {
        int playerIndex = playerId - 1;
        if (playerInfo.Count > playerIndex)
            return (PlayerDatagram)playerInfo[playerIndex];
        //throw new KeyNotFoundException($"Player ID {playerId} not found");
        return null;
    }
}
