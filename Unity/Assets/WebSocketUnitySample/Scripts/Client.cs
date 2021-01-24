using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

public class Client : MonoBehaviour
{
    [SerializeField] private Text _text = null;
    [SerializeField] private string _serverAddress = "localhost";
    [SerializeField] private int _serverPort = 3000;
    [SerializeField] private GameObject _targetObj = null;

    private Material _material = null;
    private WebSocket _webSocket = null;

    private void Start()
    {
        _material = _targetObj.GetComponent<Renderer>().material;

        SynchronizationContext context = SynchronizationContext.Current;

        _webSocket = new WebSocket($"ws://{_serverAddress}:{_serverPort}/");

        _webSocket.OnOpen += (sender, args) => { Log("WebSocket opened."); };

        _webSocket.OnMessage += (sender, args) =>
        {
            Debug.Log("OnMessage");
            
            context.Post(_ =>
            {
                string colorStr = Encoding.UTF8.GetString(args.RawData, 0, 7);

                byte[] texData = new byte[args.RawData.Length - 7];
                Array.Copy(args.RawData, 7, texData, 0, texData.Length);
                
                Texture2D tex = new Texture2D(1, 1);
                tex.LoadImage(texData);
                tex.Apply();
                
                Debug.Log(tex.width);
                
                _material.mainTexture = tex;

                if (ColorUtility.TryParseHtmlString(colorStr, out Color color))
                {
                    Debug.Log(color);
                    _material.color = color;
                }
            }, null);
        };

        _webSocket.OnError += (sender, args) => { Log($"WebScoket Error Message: {args.Message}"); };

        _webSocket.OnClose += (sender, args) => { Log("WebScoket close"); };

        _webSocket.Connect();

        Log("WebSocket Client is started.");
    }

    private void Log(string message)
    {
        _text.text = message + "\n" + _text.text;
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 130, 30), "Send"))
        {
            _webSocket.Send("Test message.");
        }
    }

    private void OnDestroy()
    {
        _webSocket?.Close();
        _webSocket = null;
    }
}