# WebSocket with Unity

このプロジェクトはWebブラウザとUnity間でWebSocket通信を行う簡単なサンプルプロジェクトです。
C#側は[こちらのプロジェクトのライブラリ](https://github.com/sta/websocket-sharp)を使用しています。

WebSocketサーバにはNode.jsを使用しているので、動作確認する場合はNode.jsのセットアップとサーバの起動をしてください。



## Node.jsサーバの起動

WebSocketサーバには`ws`モジュールを利用しています。以下の手順でセットアップを行ってください。

```shell
$ cd Nodejs/Server
```

```shell
$ npm install
```

```shell
$ node server.js
```



## Webブラウザからデータ送信

サーバの起動ができたらUnityのサンプルシーンと対象のHTMLを開いてください。
それぞれ同名のフォルダ名・シーン名になっています。（例： `MessageTest`フォルダと`MessageTest`シーン）



-----------------------------------------------



# WebSocket with Unity

This project is a demo that shows how to use a WebSocket between Web browser and Unity.
It uses a WebSocket library of [this project](https://github.com/sta/websocket-sharp).



## Launch a Node.js server

`ws` module is used for the server. Please set up this project following below.


```shell
$ cd Nodejs/Server
```

```shell
$ npm install
```

```shell
$ node server.js
```



## Send data from Web browser

Launching the server, you have to play Unity and open a HTML file into the Web browser.
There are samples as same name, such as `MessageTest`