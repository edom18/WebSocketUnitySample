using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

public class DataSendTest : MonoBehaviour
{
    [SerializeField] private Text _text = null;
    [SerializeField] private string _serverAddress = "localhost";
    [SerializeField] private int _serverPort = 3000;
    [SerializeField] private GameObject _targetObj = null;

    private Material _material = null;
    private WebSocket _webSocket = null;
    private bool _isConnected = false;

    private enum DataType
    {
        None = -1,
        Color = 0,
        Image = 1,
    }

    private void Connect()
    {
        _material = _targetObj.GetComponent<Renderer>().material;

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

            context.Post(_ => { HandleMessage(args.RawData); }, null);
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

    private void HandleMessage(byte[] data)
    {
        DataType type = GetDataType(data);

        switch (type)
        {
            case DataType.Color:
                HandleAsColor(data);
                break;

            case DataType.Image:
                HandleAsImage(data);
                break;
        }
    }

    private void HandleAsColor(byte[] data)
    {
        string colorStr = Encoding.UTF8.GetString(data, 1, 7);

        if (ColorUtility.TryParseHtmlString(colorStr, out Color color))
        {
            Debug.Log(color);
            _material.color = color;
        }
    }

    private void HandleAsImage(byte[] data)
    {
        byte[] texData = new byte[data.Length - 1];
        Array.Copy(data, 1, texData, 0, texData.Length);

        Texture2D tex = new Texture2D(1, 1);
        tex.LoadImage(texData);
        tex.Apply();
        
        _material.mainTexture = tex;

        Debug.Log(tex.width);
    }

    private DataType GetDataType(byte[] data)
    {
        if (data == null || data.Length == 0)
        {
            return DataType.None;
        }

        return (DataType)data[0];
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
        if (GUI.Button(new Rect(10, 10, 130, 30), "Disconnect"))
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