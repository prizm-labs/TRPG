using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Prizm;
using SimpleJSON;
using SocketIO;
using System.IO;

public class GameManager : MonoBehaviour {
	public GameObject DDServices;
	public TabletopInitialization CollectionReference;
	public socketDataDump dataDump;
	public SocketToDDListener DDSocketReference;
	enum msgRecieved{characterData, characterSelect, startTurn, abilitySelect, targetSelect, abilityCancel, targetConfirm, newPlayer, endTurn};
	public ActorManager ActorManagerReference;
	public TileManager TileManagerReference;
	public Dictionary<string, string> playerIDandName = new Dictionary<string, string> (); 
	public JSONNode abilityTargets;

	void Awake(){
		if (DDServices == null) {
			DDServices = GameObject.Find ("SocketIODDService");
			dataDump = DDServices.GetComponent<socketDataDump> ();
			CollectionReference = DDServices.GetComponent<TabletopInitialization> ();
			DDSocketReference = DDServices.GetComponent<SocketToDDListener> ();
		}

		//StartCoroutine (defineAbilityTargets ());
	}

	void Start(){		
		Debug.LogError (dataDump.mapData);
		initializeMap (dataDump.mapData);

	}

	void Update(){
		if (Input.GetKeyDown ("q")) {
		}
	}

	//smartTouchAction
	public IEnumerator pieceLanded(bindedObject rfAttributes){
		string nameOfActor = rfAttributes.Properties ["name"];
		string idOfActor = rfAttributes.ID;
		string sideOfActor = rfAttributes.Components [0];
		Vector3 landLocation = rfAttributes.Location;
		Ray ray = Camera.main.ScreenPointToRay (landLocation);
		RaycastHit hit;
		if (Physics.Raycast (ray, out hit, 100)) {//check if we hit a tile
			if (hit.transform.tag == "DungeonTile") {//if we found a tile, check if this object is in play and the tile allows us to put that object ontop of it
				GameObject tile = hit.transform.gameObject;
				DungeonTile tileProperties = tile.GetComponent<DungeonTile> ();
			 //does the tile allow the actor to stand?
				if (!ActorManagerReference.Actors.ContainsKey (idOfActor)) {   // if the piece and tile are okay with it, check if the actor exists in the game and move it there,  if not, create the actor
					ActorManagerReference.Actors.Add(idOfActor, null);
					Debug.LogError("Creating Actor");
					yield return StartCoroutine(ActorManagerReference.createActor (idOfActor, nameOfActor, tile));  //create new actor with rfid as ID, name of actor,  and put it on the  tile it stands on
					//TileManagerReference.enabledActorToStandAll (idOfActor);
				} else {
					if (tile.GetComponent<DungeonTile> ().ActorAllowedToStand (idOfActor)) {
						ActorManagerReference.moveActorToTile (idOfActor, tile);  //physically move actor to new tile, 
						ActorManagerReference.Actors [idOfActor].disableHighlightOfTargets ();  //disable the target's highlighting of another actor
						TileManagerReference.clearHeroMovementHighlights (idOfActor);  //if hero moves to new tile, clear all highlighted tiles
						ActorManagerReference.Actors [idOfActor].onSurface = true; // set actor on surface flag on
						yield return null;
					}
				}
					//actor is already in play and tile is highlighted for that actor
				} else
					Debug.LogError ("Object is not a dungeonTile");
			} else
				Debug.LogError ("Raycast Missed!");
		yield return null;
  	}
	public IEnumerator pieceLifted(bindedObject rfAttributes){
		string nameOfActor = rfAttributes.Properties ["name"];
		string idOfActor = rfAttributes.ID;
		if (ActorManagerReference.actorExistInGame(idOfActor)) {//if actor is currently in game:
			if (string.IsNullOrEmpty(ActorManagerReference.Actors [idOfActor].registeredPlayerName)) {//if actor is not registered
				yield return StartCoroutine(ActorManagerReference.RemoveFromGame(idOfActor));
				Debug.LogError ("Actor Destroyed!");
				//remove, and destroy piece data
			} else{//if actor is registered
				if (ActorManagerReference.actorAllowedToMove (idOfActor)) { //if actor's movement is more than 0
					ActorManagerReference.Actors [idOfActor].onSurface = false;
					ActorManagerReference.moveActorToVoid (idOfActor);  //hide the actor from view
					ActorManagerReference.enableActorMovement (idOfActor); //highlight available spaces to move
					Debug.LogError ("Actor has " + ActorManagerReference.Actors [idOfActor].currentMovementSpace + " move spaces available");
					yield return null;
				} else {
					Debug.LogError ("Actor is not allowed to move!");
				}
			//else do nothing
			}
		} else
			Debug.LogError ("Actor does not exist in game!");
		//if name of player is registered and created 
		yield return null;
	}
	public void messageReceivedFromServer(SocketIOEvent e ){
		JSONObject incommingData = e.data;
		string msgString = incommingData ["event"].str;
		int msgIndex = 	(int)System.Enum.Parse (typeof(msgRecieved), msgString);

		switch (msgIndex) {
			case 0: //create character 
				{
				}
				break;
			case 1: //characterSelect
				{
				/*
					Actor registeringActor;
					ActorManagerReference.Actors.TryGetValue (incommingData ["data"] ["character_id"].str, out registeringActor);
					if (registeringActor != null) {
						if (registeringActor.PlayerID == string.Empty) {
							Debug.LogError ("Actorid: " + incommingData ["data"] ["character_id"].str + " is registered to playerid: " + incommingData ["data"] ["player_id"].str);
							registeringActor.PlayerID = incommingData ["data"] ["player_id"].str;
							registeringActor.PlayerName = playerIDandName [incommingData ["data"] ["player_id"].str];
						}
					} else {
						Debug.LogError ("The actor you wish to register does not exist");
					}
					*/
				}
				break;
			case 2: //startTurndd
				{
					string idOfActor = incommingData ["data"].str;
					Debug.LogError ("Starting x player turn");
					if (ActorManagerReference.Actors [idOfActor].onSurface && !ActorManagerReference.actorsCurrentlyTakingTurn()) { //actor must be on surface
						if (!ActorManagerReference.actorAllowedToMove (idOfActor)) { //if actor is allowed to move
							ActorManagerReference.beginActorTurn (idOfActor); //being the actor's turn
						} else
							Debug.LogError ("Actor is already taking their turn");
					} else
						Debug.LogError ("Actor is not on surface! or another actor is taking their turn");

				}
				break;
			case 3: //abilitySelect
				{
					string idOfActor = incommingData ["data"] ["character_id"].str;
					string nameOfAbility = incommingData ["data"] ["ability"].str;
					float radius = System.Int32.Parse (abilityTargets [nameOfAbility] ["radius"].Value.ToString ());
					int scanType = System.Int32.Parse(abilityTargets[nameOfAbility]["targetType"].Value.ToString());
					ActorManagerReference.Actors [idOfActor].scanActors (radius, scanType);
					Debug.LogError ("Selected: " + nameOfAbility);

					JSONObject sendMsg = new JSONObject (JSONObject.Type.OBJECT);
					JSONObject arrData = new JSONObject (JSONObject.Type.ARRAY);
					sendMsg.AddField ("character_id", idOfActor);

				for (int i = 0; i < ActorManagerReference.Actors[idOfActor].actorTargets.Count; i++) {
					string actorName = ActorManagerReference.Actors[idOfActor].actorTargets [i].name;
					string actorID = ActorManagerReference.Actors[idOfActor].actorTargets [i].GetComponent<Actor> ().ActorID;
					JSONObject targetObjects = new JSONObject (JSONObject.Type.OBJECT);
					targetObjects.AddField ("character_id", actorID);
					targetObjects.AddField ("name", actorName);
					arrData.Add (targetObjects);
				}
				sendMsg.AddField ("data", arrData);
				messageFromTableTop (sendMsg);
				}
				break;

			case 4: //target select
				{
				Debug.LogError ("Selecting targets");
				string idOfActor = incommingData ["data"] ["character_id"].str;
				string[] nameOfTargets = new string[incommingData ["data"] ["targets"].Count];
				Debug.LogError (nameOfTargets.Length);
				for (int i = 0; i < nameOfTargets.Length; i++) {
					nameOfTargets [i] = incommingData ["data"] ["targets"] [i].str;
					Debug.LogError (nameOfTargets [i]);
				}
				//public void selectTargets(string idOfActor, params string[] targetID){
				ActorManagerReference.selectTargets(idOfActor, nameOfTargets);
				Debug.LogError ("target selected");
				}
				break;
			case 5: //abilityCancel
				{
				string idOfActor = incommingData ["data"] ["character_id"].str;
				ActorManagerReference.deselectTargets (idOfActor);
					Debug.LogError ("canceling ability");
				}
				break;
			case 6://targetConfirm
				{
					Debug.LogError ("target confirmed!");
					string idOfActor = incommingData ["data"] ["character_id"].str;
					string nameOfAbility = incommingData ["data"] ["ability"].str;
					string[] idOfTargets = new string[incommingData ["data"] ["targets"].Count];
				Debug.LogError ("id: " + idOfActor);
				Debug.LogError ("name of abilities" + nameOfAbility);
			
					for (int i = 0; i < idOfTargets.Length; i++) {
						idOfTargets [i] = incommingData ["data"] ["targets"] [i].str;
					Debug.LogError(idOfTargets[i]);
					}
				ActorManagerReference.performActorAction (idOfActor, nameOfAbility, idOfTargets);

					Debug.LogError ("Target confirmed!");
				}
				break;
			case 7://newPlayer
				{
					Debug.LogError ("Player " + incommingData ["data"] ["name"] + " has joined the session!");
					if (!playerIDandName.ContainsKey (incommingData ["data"] ["_id"].str)) {
						playerIDandName.Add (incommingData ["data"] ["_id"].str, incommingData ["data"] ["name"].str);
					} else
						Debug.LogError ("PlayerAlready Registered!");
				}
				break;
		case 8:
			{
				Debug.LogError ("End Turn");
				string idOfActor = incommingData ["data"].str;
				Debug.LogError ("of actor " + idOfActor);
				TileManagerReference.clearHeroMovementHighlights (idOfActor);
				ActorManagerReference.endActorTurn (idOfActor);

			}
			break;
			default:
				{
					Debug.LogError ("Unkown event message");
				}
				break;
			}
	}

	public void initializeMap(JSONObject mapData){
		Debug.LogError ("map data acquired");
		JSONObject wallData = mapData ["_walls"];
		JSONObject mapSize = mapData ["_size"];
		JSONObject startHeroLoc = mapData ["_startHeroTile"];
		JSONObject startVillLoc = mapData["_startVillanTile"];
		JSONObject mapObjects = mapData ["_mapObject"];
		TileManagerReference.GenerateWalls (wallData);

	}


	public void messageFromTableTop(JSONObject data){
		DDSocketReference.socket.Emit ("fromTableTop", data);
	}
	 //server actions 



	/*	private IEnumerator defineAbilityTargets(){
		string fileName = "abilityTargetType.json";
		string jsonPath =  Application.streamingAssetsPath + "/" + fileName;
		StreamReader sr;

		if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor) {
			sr = new StreamReader (jsonPath);
			yield return sr;
			abilityTargets = JSON.Parse (sr.ReadToEnd ()) as JSONNode;
		} else if (Application.platform == RuntimePlatform.Android) {
			WWW www = new WWW (jsonPath);
			yield return www;
			abilityTargets = JSON.Parse (www.text.ToString ()) as JSONNode;
		} else
			Debug.LogError ("Android and Windows only");
		yield return null;

	}*/


	/*

	//create
	PrizmRecord<ActorTemplate> actorRecord = new PrizmRecord<ActorTemplate>();
	//PrizmRecord<RoadSchema> roadRecord = gameObject.AddComponent<PrizmRecord<RoadSchema>>();

	actorRecord.mongoDocument.playerID = "1234";
	actorRecord.mongoDocument.onSurface = true;

	yield return StartCoroutine(actorRecordGroup.AddRecord(actorRecord));
	//end create

	//sync
	//actorRecordGroup.associates.Find(r => r.mongoDocument.ActorName == "omse");
	//(actorRecordGroup.SyncAll())
	//needsUpdate
	//actorRecordGroup.Sync( Prizm record here)
	//end sync
	*/



}