Roll = (dieCount,dieType)->
  sum = 0
  rolls = []
  for count in [0...dieCount]
    roll=Math.floor(Math.random()*dieType)+1
    rolls.push roll
    sum+=roll
  console.log(rolls)
  return sum

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
    @lastValue = 0

    _.each(@valueGenerators,(vg)=>
      console.log vg
      # @lastValue += Roll(vg.dieCount,vg.dieType) * multiplier ? 1 + vg.modifier
      @lastValue += Roll(vg.dieCount,vg.dieType) + vg.modifier
      console.log @lastValue
    )
    # return @lastValue + modifier ? 0
    return @lastValue

class Action
  effects: []
  range: 0,
  type: null
  description: ""
  target: null,
  targetType: null

  constructor: (effects,range,targeting,type,description)->
    @effects = effects
    @range = range
    @type = type
    @description = description


  areaOfEffect: ()-> 1

  setTarget: (target)->
    @target = target

  resolve: ()->

    resolveDamage = (effect)->
      console.log @target.hitPoints
      @target.hitPoints-=effect.resolve()
      console.log @target.hitPoints

    _.each(@effects,(effect)=>
      switch effect.type
        when "damage" then resolveDamage.call(@,effect)
        else null
      )




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
  currentAction: null

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

  selectAction: (key)->
    @currentAction = @actions[key] if @actions[key]

  unselectAction: ()->
    @currentAction = null

  showActions: ()->
    _.each(@actions,(action)->
      console.log(action.description)
      )

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


@Actor = Actor
@TurnTracker = TurnTracker
@Effect = Effect
@Action = Action
