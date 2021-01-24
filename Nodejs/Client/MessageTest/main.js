
class WebSocketController {
    connection = null;
    eventTypeField = null;
    messageDisplayField = null;

    constructor(eventTypeField, messageDisplayField) {
        this.eventTypeField = eventTypeField;
        this.messageDisplayField = messageDisplayField;
    }

    connect(url, port) {
        const URL = "ws://" + url + ":" + port + "/";
        console.log("Will connect to " + URL);

        // Connect to the server.
        this.connection = new WebSocket(URL);

        // 接続通知
        this.connection.onopen = (event) => {
            this.eventTypeField.innerHTML = "通信接続イベント受信";
            this.messageDisplayField.innerHTML = event.data === undefined ? "--" : event.data;
        };

        //エラー発生
        this.connection.onerror = (error) => {
            this.eventTypeField.innerHTML = "エラー発生イベント受信";
            this.messageDisplayField.innerHTML = error.data;
        };

        //メッセージ受信
        this.connection.onmessage = (event) => {
            console.log("Received a message [" + event.data + "]");

            this.eventTypeField.innerHTML = "メッセージ受信";
            this.messageDisplayField.innerHTML = event.data;
        };

        //切断
        this.connection.onclose = () => {
            this.eventTypeField.innerHTML = "通信切断イベント受信";
            this.messageDisplayField.innerHTML = "";
        };
    }

    disconnect() {
        console.log("Will disconnect from " + URL);
        this.connection.close();
    }

    sendMessage(message) {
        console.log("Will send message.");
        this.connection.send(message);
    }
}

let eventTypeField = document.getElementById("eventType");
let messageDisplayField = document.getElementById("dispMsg");

const ws = new WebSocketController(eventTypeField, messageDisplayField);

document.getElementById("Connect").addEventListener('click', () => {
    let url = document.getElementById("serverAddress").value;
    let port = document.getElementById("serverPort").value;
    ws.connect(url, port);
});

document.getElementById("Disconnect").addEventListener('click', ws.disconnect);

document.getElementById("Send").addEventListener('click', () => {
    var message = document.getElementById("inputField").value;
    ws.sendMessage(message);
});