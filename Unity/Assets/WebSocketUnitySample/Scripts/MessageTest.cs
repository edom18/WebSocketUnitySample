using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

public class MessageTest : MonoBehaviour
{
    [SerializeField] private Text _text = null;
    [SerializeField] private string _serverAddress = "localhost";
    [SerializeField] private int _serverPort = 3000;

    private WebSocket _webSocket = null;
    private bool _isConnected = false;

    private void Connect()
    {
        SynchronizationContext context = SynchronizationContext.Current;

        _webSocket = new WebSocket($"ws://{_serverAddress}:{_serverPort}/");

        _webSocket.OnOpen += (sender, args) =>
        {
            _isConnected = true;
            Log("WebSocket opened.");
        };

        _webSocket.OnMessage += (sender, args) =>
        {
            Debug.Log("OnMessage");

            context.Post(_ => { Log(args.Data); }, null);
        };

        _webSocket.OnError += (sender, args) => { Log($"WebScoket Error Message: {args.Message}"); };

        _webSocket.OnClose += (sender, args) =>
        {
            _isConnected = false;
            Log("WebScoket close");
        };

        _webSocket.Connect();

        Log("WebSocket Client is started.");
    }

    private void Log(string message)
    {
        Debug.Log(message);
        _text.text = message;
    }

    private void OnGUI()
    {
        if (_isConnected)
        {
            DrawDisconnectUI();
        }
        else
        {
            DrawConnectUI();
        }
    }

    private void DrawConnectUI()
    {
        GUI.Label(new Rect(10, 10, 100, 20), "Server Address:");
        _serverAddress = GUI.TextField(new Rect(110, 10, 130, 20), _serverAddress);

        GUI.Label(new Rect(10, 40, 100, 20), "Server Address:");
        string portStr = GUI.TextField(new Rect(110, 40, 130, 20), _serverPort.ToString());
        int.TryParse(portStr, out _serverPort);

        if (GUI.Button(new Rect(250, 10, 130, 50), "Connect"))
        {
            Connect();
        }
    }

    private void DrawDisconnectUI()
    {
        if (GUI.Button(new Rect(10, 10, 130, 50), "Disconnect"))
        {
            _webSocket.Close();
        }
    }

    private void OnDestroy()
    {
        _webSocket?.Close();
        _webSocket = null;
    }
}