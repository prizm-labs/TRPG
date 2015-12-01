Roll =
  d20: (dice)-> Math.floor(Math.random()*20)

ActorTypes =
  player: 0
  playerAlly: 1
  enemy: 2
  ally: 3,
  neutral: 4

ActionTypes =
  normal: 0
  bonus: 1
  reaction: 2

@ActionTypes = ActionTypes
@ActorTypes = ActorTypes

class Effect
  valueGenerators: []
  lastValue: null
  type: null
  subtype: null
  description: ""

  constructor: (valueGenerators,type,subtype,description)->
    @valueGenerators = valueGenerators
    @type = type
    @subtype = subtype
    @description = description

  presentResolution: (value)->
    # manual resolution
    # valueGenerators to string
    @lastValue = value

  resolve: (modifier,multiplier)->
    # automatic resolution
    _.each(@valueGenerators,(vg)->
      lastValue+=vg.dieType*vg.dieCount
      )
    return @lastValue * multiplier?multiplier:1 + modifier

class Action
  effects: []
  range: 0,
  type: null
  areaOfEffect: ()-> 1
  constructor: (effects,range,targeting,type)->
    @effects = effects
    @range = range
    @type = type


class Actor

  type: null
  initiative: 0
  movement: 0
  hitPoints: 0
  name: null
  currentMap: null
  currentXCoordinate: null
  currentYCoordinate: null
  sizeX: 0
  sizeY: 0
  actions: {}
  stats: {}

  constructor: (name,movementSpeed,hitPoints,type,stats)->
    @type = type
    @name = name
    @movementSpeed = movementSpeed
    @hitPoints = hitPoints
    @stats = stats

  info: ()->
    console.log(@name,@movementSpeed)

  setMapCoordinate: (x,y)->
    @currentXCoordinate = x
    @currentYCoordinate = y

  setIntiative: (value)->
    @initiative = value

  addAction: (key, action)->
    @actions[key] = action

  randomizeInitiative: ()->
    @setIntiative Roll.d20 1

class TurnTracker
  constructor:(@type)->

  currentTurn: null
  currentParticipantIndex: null
  currentParticipant: null
  totalTurns:0

  turnOrder: []
  participants:[]

  addParticipant: (participant)->
    @participants.push(participant)

  advanceTurn: ()->
    if @totalTurns == 0
      @currentParticipantIndex = 0
      @currentTurn = 1
    else
      @currentParticipantIndex+=1
      @currentTurn+=1

      if @currentParticipantIndex>= @participants.length
        @currentParticipantIndex = 0

    @totalTurns+=1

    @currentParticipant = @turnOrder[@currentParticipantIndex]
    console.log @currentParticipant

  begin: ()->
    @sortByInitiative()
    @advanceTurn()

  sortByInitiative: ()->
    @turnOrder = _.sortBy(@participants,(p)->
      return -p.initiative
      )
    console.log @turnOrder

class Session
  constructor: ()->


class Action
  constructor: ()->

@Actor = Actor
@TurnTracker = TurnTracker
@Effect = Effect
@Action = Action
