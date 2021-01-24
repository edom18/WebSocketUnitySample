var WebSocket = require('ws');
var ws = WebSocket.Server;
var wss = new ws({port: 3000});

console.log(wss.clients);

wss.brodcast = function (data) {
    this.clients.forEach(function each(client) {
        if (client.readyState === WebSocket.OPEN) {
            client.send(data);
        }
    });
};

wss.on('connection', function (ws) {
    ws.on('message', function (message) {
        var now = new Date();
        console.log(now.toLocaleString() + ' Received.');
        wss.brodcast(message);
    });
});