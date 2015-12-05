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
  movement: 3

@ActionTypes = ActionTypes
@ActorTypes = ActorTypes

class Effect
  valueGenerators: []
  defenses: []
  lastValue: null
  type: null
  subtype: null
  description: ""

  constructor: (valueGenerators,type,subtype,description,defenses)->
    @valueGenerators = valueGenerators
    @type = type
    @subtype = subtype
    @description = description
    @defenses = defenses

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
  duration: 6,
  actor: null

  constructor: (effects,range,targeting,type,description)->
    @effects = effects
    @range = range
    @type = type
    @description = description


  showTargetRange: ()->

  areaOfEffect: ()-> 1

  setTarget: (target)->
    @target = target

    #TODO distinguish between single, multiple, AoE

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

class MovementAction extends Action

  #gridType: square or hex

  constructor: (range)->
    super([],range,1,ActionTypes.movement,"Move character at regular speed")


  setRange: (range)->
    @range = range

  showTargetRange: ()->
    origin = [@actor.currentTile.x,@actor.currentTile.y]
    range = @actor.grid.
    return @actor.grid.getRangeAroundCoordinate(origin[0],origin[1])

  setTarget: ()-> #currentXCoordinate

  resolve: ()->



class Grid

  tileWidth: 0
  tileHeight: 0
  tileUnit: 0
  tiles: []

  constructor: (tileType,tileUnit,heightByTile,widthByTile)->

    @tileUnit = tileUnit

    generateSquareGrid = (tiles)->
      @tileWidth = tileUnit
      @tileHeight = tileUnit

      for x in [0...widthByTile]
        for y in [0...heightByTile]
          tiles.push(new Tile(x,y))

    generateHexGrid = (tiles)->

    switch tileType
      when "square" then generateSquareGrid(@tiles)
      when "hex" then generateHexGrid(@tiles)



  getTile: (x,y)-> _.findWhere(@tiles,{x:x,y:y})

  getRangeByTile: (speed)-> speed/@tileUnit

  getRangeAroundCoordinate: (x,y,range)->
    # restrict to orthagonal movement
    coordinates = []
    conjugates = []
    # [0,0]

    origin = [x,y]

    getAllConjugatesAtRange = (range)->
      difference = 0
      conjugates = []
      while difference<range
        conjugates.push [range-difference,difference]
        conjugates.push [difference,range-difference] if difference!=range-difference
        difference++

      return conjugates

    for r in [0...range]
      conjugates = conjugates.concat getAllConjugatesAtRange(r+1)

    translations = []

    for pair in conjugates
      translations.push pair
      translations.push [-pair[0],pair[1]] if pair[0]!=0
      translations.push [-pair[0],-pair[1]] if pair[1]!=0 && pair[0]!=0
      translations.push [pair[0],-pair[1]] if pair[1]!=0

    console.log(translations)

    for move in translations
      target = [origin[0]+move[0],origin[1]+move[1]]
      coordinates.push target if target[0]>=0 && target[1]>=0
      #TODO other conditions to invalidate coordinate
      #coordinate is occupied by an actor
      #coordinate is impassable terrain/object

    console.log(coordinates)

    return coordinates


class Tile
  x: null
  y: null
  constructor: (x,y)->
    @x = x
    @y = y

class Actor

  type: null
  initiative: 0
  movementSpeed: 0
  availableMovement: 0
  hitPoints: 0
  name: null
  currentMap: null
  currentTile: null
  grid: null
  sizeX: 0
  sizeY: 0
  actions: {}
  stats: {}
  currentAction: null


  constructor: (name,movementSpeed,hitPoints,type,stats)->
    @type = type
    @name = name
    @movementSpeed = movementSpeed
    @availableMovement = movementSpeed
    @hitPoints = hitPoints
    @stats = stats

  info: ()->
    console.log(@name,@movementSpeed)

  setMapCoordinate: (x,y)->
    @currentTile = @grid.getTile(x,y)

  setIntiative: (value)->
    @initiative = value

  addAction: (key, action)->
    _action = _.clone(action,true)
    @actions[key] = _action
    _action.actor = @

  selectAction: (key)->
    @currentAction = @actions[key] if @actions[key]

  unselectAction: ()->
    @currentAction = null

  showActions: ()->
    _.each(@actions,(action)->
      console.log(action.description)
      )

  randomizeInitiative: ()->
    @setIntiative Roll(1,20)

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




@Grid = Grid
@Actor = Actor
@TurnTracker = TurnTracker
@Effect = Effect
@Action = Action
@MovementAction = MovementAction
