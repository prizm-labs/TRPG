# express = Meteor.npmRequire('express')
# http = Meteor.npmRequire('http')
# io = Meteor.npmRequire('socket.io')
#
# port = 6666
#
# app = express()
# # app.use((req, res, next)->
# #   res.header("Access-Control-Allow-Origin", "*:*")
# #   res.header("Access-Control-Allow-Headers", "X-Requested-With")
# #   res.header("Access-Control-Allow-Headers", "Content-Type")
# #   res.header("Access-Control-Allow-Methods", "PUT, GET, POST, DELETE, OPTIONS")
# #   next()
# # )
# httpServer = http.createServer(app)
# @server = io(httpServer)
#
#
# @numUsers = 99
#
# @server.on('connection', (socket)=>
#   console.log('NEW CONNECTION')
#   socket.emit('test','{"hello":"world!"}')
# )
#
# httpServer.listen(port)
