express = Meteor.npmRequire('express')
http = Meteor.npmRequire('http')
io = Meteor.npmRequire('socket.io')

port = 6666

app = express()
app.use((req, res, next)->
  res.header("Access-Control-Allow-Origin", "*:5000")
  res.header("Access-Control-Allow-Headers", "X-Requested-With")
  res.header("Access-Control-Allow-Headers", "Content-Type")
  res.header("Access-Control-Allow-Methods", "PUT, GET, POST, DELETE, OPTIONS")
  next()
)
httpServer = http.createServer(app)
server = io(httpServer, {origins:'*:5000'})


@numUsers = 0

server.on('connection', @ONCONNECTION.bind(@) )

httpServer.listen(port)
