


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

greatswordDamageEffect = new Effect([{dieCount:2,dieValue:6,modifier:4}],"damage","slashing")
greatswordAction = new Action([greatswordDamageEffect],5,1,ActionTypes.normal)

player.addAction(greatswordAction)

participants = [player,player2,player3,enemy,enemy2,enemy3]

_.each(participants,(participant,index)->
  participant.randomizeInitiative()
  tracker.addParticipant(participant)
  )

tracker.begin()
tracker.advanceTurn()
tracker.advanceTurn()
tracker.advanceTurn()
tracker.advanceTurn()
tracker.advanceTurn()
tracker.advanceTurn()
