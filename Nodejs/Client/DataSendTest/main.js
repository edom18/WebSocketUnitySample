
class WebSocketController {
    static MessageType = {
        Color: 0,
        Image: 1,
    };
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

function changedColor(evt) {
    var colorData = getColorData(evt.target.value);
    var array = new Uint8Array(colorData);
    var buffer = new ArrayBuffer(array.byteLength + 1);
    var data = new Uint8Array(buffer);
    data.set([WebSocketController.MessageType.Color], 0);
    data.set(array, 1);
    ws.sendMessage(data.buffer);
}

function getColorData(colorStr) {
    return new TextEncoder().encode(colorStr).buffer;
}

function changedFile(evt) {
    var files = evt.target.files;

    if (files.length <= 0) {
        return;
    }

    var file = files.item(0);
    var reader = new FileReader();

    reader.onload = function () {
        var array = new Uint8Array(reader.result);

        var buffer = new ArrayBuffer(array.byteLength + 1);
        var data = new Uint8Array(buffer);
        data.set([WebSocketController.MessageType.Image], 0);
        data.set(array, 1);

        console.log(data.buffer.byteLength);

        ws.sendMessage(data.buffer);
    }

    reader.readAsArrayBuffer(file);
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

document.getElementById("Color").addEventListener('change', changedColor);
document.getElementById("File").addEventListener('change', changedFile);