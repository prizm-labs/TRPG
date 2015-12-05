Meteor.methods(
  invite: (data)->
    console.log "testMeteorCall:",data
    return data+" echo"
)
