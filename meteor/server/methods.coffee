Meteor.methods(
  invite: (data)->
    console.log "testMeteorCall:",data
    return data+" echo"

  selectAction: (actorId,actionId)->
    console.log "selectAction",actorId,actionId


  selectTargeting: (actionId, targeting)->
    console.log "selectTargeting", actionId,targeting

)
