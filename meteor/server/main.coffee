
grid = new Grid("square",5,5,20,20)

grid.getRangeAroundCoordinate(0,0,2)

grid.getRangeAroundCoordinate(2,2,2)

tracker = new TurnTracker()



console.log tracker.participants


player = new Actor("fighter",40,12,ActorTypes.player,{
  STR: 2, DEX: 3, CON: 2, INT: 0, WIS: 1, CHA:-1
  },{})
player2 = new Actor("wizard",30,8,ActorTypes.player)
player3 = new Actor("cleric",30,11,ActorTypes.player)

enemy = new Actor("goblin",40,10,ActorTypes.enemy)
enemy2 = new Actor("goblin",40,10,ActorTypes.enemy)
enemy3 = new Actor("goblin",40,10,ActorTypes.enemy)

player.grid = grid
player.setMapCoordinate(0,0)

movementAction = new MovementAction(30)

# p257 PH
magicMissleEffect = new Effect([{dieCount:1,dieType:4,modifier:1}],"damage","magic")
magicMissileAction = new Action([magicMissleEffect,magicMissleEffect,magicMissleEffect],
120,[1,3],ActionTypes.normal,"Cast Magic Missile")

# p220 PH
burningHandsEffect = new Effect([{dieCount:3,dieType:6,modifier:0}],"damage","fire")
burningHandsAction = new Action([burningHandsEffect],0,["cone",15],ActionTypes.normal,"Cast Burning Hands")

greatswordDamageEffect = new Effect([{dieCount:2,dieType:6,modifier:4}],"damage","slashing")
greatswordAction = new Action([greatswordDamageEffect],5,1,ActionTypes.normal,"Attack with greatsword")

player.addAction("greatsword",greatswordAction)
player.addAction("movement",movementAction)
player.showActions()

player.selectAction("greatsword")
player.currentAction.setTarget(enemy)
player.currentAction.resolve()


player2.addAction("magicMissile",magicMissileAction)
player2.addAction("burningHands",burningHandsAction)
player2.showActions()

player2.selectAction("magicMissile")
player2.currentAction.setTarget(enemy2)
player2.currentAction.resolve()

# participants = [player,player2,player3,enemy,enemy2,enemy3]
#
# _.each(participants,(participant,index)->
#   participant.randomizeInitiative()
#   tracker.addParticipant(participant)
#   )
#
# tracker.begin()
# tracker.advanceTurn()
# tracker.advanceTurn()
# tracker.advanceTurn()
# tracker.advanceTurn()
# tracker.advanceTurn()
# tracker.advanceTurn()
