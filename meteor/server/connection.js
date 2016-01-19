

  var express = Meteor.npmRequire('express');
  var http = Meteor.npmRequire('http');
  var io = Meteor.npmRequire('socket.io');
  var router = Meteor.npmRequire('socket.io-events')();

  WebSocketDelegate = function (port, eventEmitter) {

    //this.events = eventEmitter;
    this.connections = [];

    var app = express();
    var httpServer = http.createServer(app);
    this.server = io(httpServer);



    router.on('*', function (sock, args, next) {
      console.log('router:',args);
      var name = args.shift(), msg = args.shift();
      sock.emit('received event', name, msg);

      switch(name) {
        // HH->S->TT   action-selected
        case "action-selected":

        break;

        // TT->S->HH   action-received
        case "action-received":
          share.Combat.startTurn();
        break;

        // HH->S->TT   target-selected
        case "action-selected":

        break;

        // HH->S->TT   target-confirmed
        case "action-selected":

        break;

        // TT->S->HH   action-resolved
        case "action-selected":

        break;

        // TT->S       movement-initiated
        case "action-selected":

        break;

        // TT->S       movement-selected
        case "action-selected":

        break;

        // HH->S       movement-confirmed
        case "action-selected":

        break;

        // S->HH,TT    turn-start
        case "action-selected":

        break;

        // TT->S       turn-end
        case "action-selected":

        break;

        default:
        break;
      }
    });
    this.server.use(router);

    this.server.on('connection', onConnection.bind(this) );


    function onConnection (socket) {
      console.log("A NEW CONNECTION",socket.id);;
      //var payload = {"characterId":1,"actionId":"handaxe"};
      var payload = {"characterId":1,"actionId":"scorchingray"};
      socket.emit('action-selected',payload);
      //socket.on('action-received', onMessage.bind(this) );
      this.connections.push(socket);

      //connection.on('close', onClose.bind(this));
      //connection.on('message', onMessage.bind(this));
    }

    httpServer.listen(port);

    //this.events.on('broadcastToClient',broadcastToClient.bind(this));

    //this.server.on('request', onRequest.bind(this));

    function broadcastToClient(data) {

      // TO LIMIT MESSAGES TO CLIENT
      // SEND CLIENT ONLY "smarttouch" MESSAGES

      if (data.topic=="smarttouch-start" || data.topic=="smarttouch-end" || data.topic=="board-restart") {
        this.write(data.topic,data.body);
      }

    }

    function onRequest(request) {
      console.log('requestedProtocols:',request.requestedProtocols);

        // UniWeb plugin requests 'chat','superchat' protocols
        // req.headers.Add ("Sec-WebSocket-Protocol", "chat, superchat");
        // in ./Assets/UniWeb/Plugins/HTTP/WebSocket.cs

        // The "Sec-WebSocket-Protocol" header lists the subprotocols
        // https://github.com/theturtle32/WebSocket-Node/issues/32

        var connection = request.accept(null, request.origin);
        connection.on('close', onClose.bind(this));
        connection.on('message', onMessage.bind(this));

        this.connections.push(connection);

        console.log("New websocket connection:",connection.remoteAddress);

    }

    function onClose(connection) { // Handle closed connections
        console.log(connection + " disconnected");

        var index = this.connections.indexOf(connection);
        if (index !== -1) {
            // remove the connection from the pool
            connections.splice(index, 1);
        }
    }

    function onMessage(message) { // Handle incoming messages
      console.log("MESSAGE FROM CLIENT:",message);

        if (message.type === 'utf8') {
            try {
                var messageObject = JSON.parse(message.utf8Data);
                console.log(messageObject);

                this.parseClientMessage(messageObject);
            }
            catch(e) {
                // do nothing if there's an error.
                console.log("data not valid json.");
            }
        }
    }

    console.log("SmartTouch server ready");
  };

  WebSocketDelegate.prototype = {

      write: function (msg, data) {
        console.log("websocket write:",msg,data);
        if (this.connections.length==0) {
          console.log("no WebSocket  connections.");
          return false;
        } else {

          console.log("sending:",msg,data);

          _.each(this.connections,function(connection){
              connection.emit(msg,data);
          });

          return true;
        }

      },

      parseClientMessage: function (messageObject) {

        var topic = messageObject.topic, message = messageObject.message;

        switch (topic) {
          case "antennas":
            // client wants to know where to draw live antenna zones on screen
            this.events.emit("reportAntennaZones");
          break;

          case "smartpieces":
            // client wants to know association between RFTag IDs and GameObjects
            this.events.emit("reportSmartPieceManifest");
          break;

          case "lightup":
            // client wants to light up a smart piece
            console.log("parse: lightup");
            this.events.emit("lightingAction",message);
          break;

          default:
          break;
        }
      }
  }

  //var eventEmitter = new EventEmitter();
    var webSocketDelegate = new WebSocketDelegate(6666 );

this.ONCONNECTION = function (socket) {
  var addedUser = false;
  var server = this;

  console.log('NEW CONNECTION',this.numUsers);
  this.server.emit('test','{"hello":"world!"}');

  // // when the client emits 'new message', this listens and executes
  // socket.on('new message', function (data) {
  //   // we tell the client to execute 'new message'
  //   socket.broadcast.emit('new message', {
  //     username: socket.username,
  //     message: data
  //   });
  // });
  //
  // // when the client emits 'add user', this listens and executes
  // socket.on('add user', function (username) {
  //   if (addedUser) return;
  //
  //   // we store the username in the socket session for this client
  //   socket.username = username;
  //   ++server.numUsers;
  //   addedUser = true;
  //   socket.emit('login', {
  //     numUsers: server.numUsers
  //   });
  //   // echo globally (all clients) that a person has connected
  //   socket.broadcast.emit('user joined', {
  //     username: socket.username,
  //     numUsers: server.numUsers
  //   });
  // });
  //
  // // when the client emits 'typing', we broadcast it to others
  // socket.on('typing', function () {
  //   socket.broadcast.emit('typing', {
  //     username: socket.username
  //   });
  // });
  //
  // // when the client emits 'stop typing', we broadcast it to others
  // socket.on('stop typing', function () {
  //   socket.broadcast.emit('stop typing', {
  //     username: socket.username
  //   });
  // });
  //
  // // when the user disconnects.. perform this
  // socket.on('disconnect', function () {
  //   if (addedUser) {
  //     --server.numUsers;
  //
  //     // echo globally that this client has left
  //     socket.broadcast.emit('user left', {
  //       username: socket.username,
  //       numUsers: server.numUsers
  //     });
  //   }
  // });
}
