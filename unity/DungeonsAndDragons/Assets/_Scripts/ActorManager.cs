using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using SimpleJSON;

public class ActorManager : MonoBehaviour {
	public GameObject DDServices;
	public TabletopInitialization CollectionReference;
	public socketDataDump dataDump;
	enum PrefabEnum{warrior, wizard, cleric, thief, ranger, goblin, kobold, zombie};
	public GameObject newActorPrefab;
	public List<GameObject> ActorPrefabs = new List<GameObject>();
	public Dictionary<string, Actor> Actors = new Dictionary<string, Actor>();

	void Awake(){
		if (DDServices ==null) {
			DDServices = GameObject.Find ("SocketIODDService");
			if (DDServices != null) {
				dataDump = DDServices.GetComponent<socketDataDump> ();
				CollectionReference = DDServices.GetComponent<TabletopInitialization> ();
			}
		}
	}



	/*
	public void initializeCharacterData(string actorName,  int initMove, int initHP, int initAC, string[] listOfAbilities, int[] abilitiesDamageAmount, float[] abilitiesRange){
		Debug.LogError ("Initializing characters with actor name :" + actorName);
		for (int i = 0; i < ActorPrefabs.Count; i++) {//search through all actor prefabs
			if (ActorPrefabs [i].name == actorName) {// if we found this actor by its name....
				Actor actorProperties = ActorPrefabs[i].GetComponent<Actor>();
				actorProperties.AC = initAC; //set prefab's initial AC
				actorProperties.HP = initHP;//set prefab's initial HP
				actorProperties.maxMovementSpace = initMove; //set prefab's initial movement

				for (int j = 0; j < listOfAbilities.Length; j++) {
					ActorPrefabs[i].AddComponent (System.Type.GetType (listOfAbilities [j]));
					actorProperties.abilityNameToIndex.Add(listOfAbilities[j]);
				}
				Debug.LogError (actorProperties.listOfActions.Length);
				actorProperties.listOfActions = ActorPrefabs [i].GetComponents<ActionProperties> ();
				Debug.LogError (actorProperties.listOfActions.Length + " number of properties");
				for (int j = 0; j < actorProperties.listOfActions.Length; j++) {
					Debug.LogError (actorProperties.listOfActions [j] + " component ");
				actorProperties.listOfActions [j].damageAmount = abilitiesDamageAmount [j];
				actorProperties.listOfActions [j].scanRange = abilitiesRange [j];
				actorProperties.listOfActions [j].enabled = false;
				}
			}
		}
	}
*/

	public bool actorExistInGame(string idOfActor){ 
		return Actors.ContainsKey (idOfActor);
  	}

	public void someoneDied(string idOfActor){
		endActorTurn (idOfActor);
		Actors.Remove (idOfActor);
	}

	public bool actorsCurrentlyTakingTurn(){
		bool anyActorTurn = false;

		foreach (KeyValuePair<string,Actor> actor in Actors) {
			if (actor.Value.playerTurn) {
				anyActorTurn = true;
				break;
			}
		}

		return anyActorTurn;
	}

	public void beginActorTurn(string idOfActor){
		Actors [idOfActor].resetMovement ();
		Actors [idOfActor].playerTurn = true;
	}

	public void endActorTurn(string idOfActor){
		Vector3 tilePosition = Actors[idOfActor].currentTile.transform.position;
		tilePosition.y += 0.5f;
		tilePosition.x += 0.75f;
		tilePosition.z += 1.0f;
		Actors [idOfActor].gameObject.transform.position = tilePosition;
		Actors [idOfActor].onSurface = true;
		Actors [idOfActor].disableHighlightOfTargets ();
		Actors [idOfActor].currentMovementSpace = 0;
		Actors [idOfActor].playerTurn = false;
	}

	public bool actorAllowedToMove(string idOfActor){
		if (Actors [idOfActor].currentMovementSpace > 0 && Actors[idOfActor].playerTurn) {
			return true;
		} else
			return false;
	}

	//invoke from STE
	public void moveActorToVoid(string idOfActor){
		Actors [idOfActor].gameObject.transform.position = new Vector3 (100.0f, 100.0f, 100.0f);
	}

	//invoke from STE
	public void enableActorMovement(string idOfActor){
		Actors [idOfActor].currentTile.GetComponent<DungeonTile> ().isTileOccupied = false;
		Actors [idOfActor].highlightAvailableSpaces ();
	}

	//invoke from STS
	public void moveActorToTile(string idOfActor, GameObject tile){
		Vector3 tilePosition = tileSpawnPosition (tile);
		Actors [idOfActor].gameObject.transform.position = tilePosition;
		if (Actors [idOfActor].currentTile != null) {
			Actors [idOfActor].updateMovementCount (tile.GetComponent<DungeonTile>().actorMoveCount[idOfActor]);
			Actors [idOfActor].currentTile.GetComponent<DungeonTile> ().isTileOccupied = false;
		}
		Actors [idOfActor].currentTile = tile;
		Actors [idOfActor].currentTile.GetComponent<DungeonTile> ().isTileOccupied = true;

	}
		

	public Vector3 tileSpawnPosition(GameObject tile){
		Vector3 tilePosition = tile.transform.position;
		tilePosition.y += 0.5f;
		tilePosition.x += 0.75f;
		tilePosition.z += 1.0f;
		return tilePosition;
	}

	//invoke from HH
	public IEnumerator createActor(string ActorID, string nameOfActor, GameObject tile){
		Vector3 tileSpawn = tileSpawnPosition (tile);  //tile spawn point;
		int prefabIndex = (int)System.Enum.Parse (typeof(PrefabEnum), nameOfActor);
		GameObject newActor = Instantiate (ActorPrefabs [prefabIndex], tileSpawn, Quaternion.identity) as GameObject;
		newActor.SetActive (false);
		Actors[ActorID] = newActor.GetComponent<Actor> ();
		Actor actorProperties = newActor.GetComponent<Actor> ();
		newActor.name = nameOfActor;
		actorProperties.ActorID = ActorID;
		actorProperties.onSurface = true;
		tile.GetComponent<DungeonTile> ().isTileOccupied = true;

		initializeActor (newActor);
		addActorToCollections (newActor);
		newActor.SetActive (true);
		yield return null;
	}

	public void initializeActor(GameObject actor){
		Actor actorProperties = actor.GetComponent<Actor> ();
		string actorName = actor.name;
		JSONObject ActorData = new JSONObject();
		//search datadump for actor properties by its name
		for (int i = 0; i < dataDump.characterData.Count; i++) {
			if(actorName == dataDump.characterData[i]["class"].str){
				ActorData = dataDump.characterData [i];
			}
		}
		int initMove =(int)ActorData ["movement"].n;  //found its movement
		int initHP = (int)ActorData["hp"].n;  //found its hp
		int initAC = (int)ActorData["ac"].n;  //found its AC
		string [] nameOfabilities = new string [ActorData["abilities"].Count]; 
		int[] abilitiesDamageAmount = new int[ActorData ["abilities"].Count];
		float[] abilitiesRange = new float[ActorData ["abilities"].Count];

		for(int i =0; i<nameOfabilities.Length;i++){
			nameOfabilities[i] = ActorData["abilities"][i]["id"].str;  //found all names of abilities
			abilitiesDamageAmount [i] = System.Int32.Parse (ActorData["abilities"] [i] ["amount"].str); //found all ability damage 
			abilitiesRange [i] = float.Parse(ActorData ["abilities"] [i] ["radius"].str);  //found all ability ranges

		}
	
		actorProperties.AC = initAC; //set prefab's initial AC
		actorProperties.HP = initHP;//set prefab's initial HP
		actorProperties.maxMovementSpace = initMove; //set prefab's initial movement

		for (int i = 0; i < nameOfabilities.Length; i++) {
			actor.AddComponent (System.Type.GetType (nameOfabilities [i]));
			actorProperties.abilityNameToIndex.Add(nameOfabilities[i]);
		}
		actorProperties.listOfActions = actor.GetComponents<ActionProperties> ();
		for (int i = 0; i < actorProperties.listOfActions.Length; i++) {
			actorProperties.listOfActions [i].damageAmount = abilitiesDamageAmount [i];
			actorProperties.listOfActions [i].scanRange = abilitiesRange [i];
			actorProperties.listOfActions [i].enabled = false;
		}
		Debug.LogError ("parameters set");
	}

	public void addActorToCollections(GameObject actor){
		PrizmRecord<ActorTemplate> actorRecord = new PrizmRecord<ActorTemplate>();
		if (!CollectionReference.actorRecordGroup.associates.Contains (actorRecord)) {
			Actor actorProperties = actor.GetComponent<Actor> ();
			//PrizmRecord<RoadSchema> roadRecord = gameObject.AddComponent<PrizmRecord<RoadSchema>>();
			actorRecord.mongoDocument.ActorID = actorProperties.ActorID;
			actorRecord.mongoDocument.ActorName = actor.name;
			actorRecord.mongoDocument.playerID = actorProperties.registeredPlayerID;
			actorRecord.mongoDocument.playerName = actorProperties.registeredPlayerName;
			actorRecord.mongoDocument.HP = actorProperties.HP;
			actorRecord.mongoDocument.AC = actorProperties.AC;
			actorRecord.mongoDocument.MaxMove = actorProperties.maxMovementSpace;
			actorRecord.mongoDocument.currentMove = actorProperties.currentMovementSpace;
			actorRecord.mongoDocument.playerTurn = actorProperties.playerTurn;
			actorRecord.mongoDocument.onSurface = actorProperties.onSurface;
			StartCoroutine (CollectionReference.actorRecordGroup.AddRecord (actorRecord));
		}
	}

	public IEnumerator RemoveFromGame(string idOfActor){
		Destroy (Actors [idOfActor].gameObject);
		Actors.Remove (idOfActor);
		yield return removeActorFromCollections(idOfActor);
	}

	public IEnumerator removeActorFromCollections(string idOfActor){
		if (CollectionReference.actorRecordGroup.associates.Find (p => p.mongoDocument.ActorID == idOfActor) != null) {
			yield return StartCoroutine (CollectionReference.actorRecordGroup.RemoveRecord (CollectionReference.actorRecordGroup.associates.Find (p => p.mongoDocument.ActorID == idOfActor), CollectionReference.actorRecordGroup.collectionKey));
		}
		else 
			yield return null;
	}
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
	
	//invoke from HH
	public void performActorAction(string idOfActor, string nameOfAction, params string[] target){
		Debug.LogError ("Performing Actions");
		//Actors
		GameObject[] newTarget = new GameObject[target.Length];
		for (int i = 0; i < target.Length; i++) {
			newTarget [i] = Actors[target [i]].gameObject;
		}
		Actors [idOfActor].performAction (nameOfAction, newTarget);
	}

	public void selectTargets(string idOfActor, params string[] targetID){
		Debug.LogError ("Searching for targets!");
		deselectTargets (idOfActor);
		for(int i =0; i<targetID.Length;i++){
			Actors [idOfActor].selectedTargets.Add (Actors [targetID [i]]);
			//Actors[idOfActor].selectedTargets.Add(GameObject.Find ("actors/Enemy/" + targetID[i]).GetComponent<Actor> ());
			Debug.LogError (Actors [idOfActor].selectedTargets [i].ActorID);
		}
		Actors [idOfActor].highlightTargets ();

	}

	public void deselectTargets(string idOfActor){
		Actors[idOfActor].	disableHighlightOfTargets ();
	}

	void OnApplicationQuit(){
		for (int i = 0; i < ActorPrefabs.Count; i++) {
			Actor actorProperties = ActorPrefabs [i].GetComponent<Actor> ();
			actorProperties.HP = 0;
			actorProperties.AC = 0;
			actorProperties.listOfActions = null;
			actorProperties.abilityNameToIndex.Clear ();
			ActionProperties[] gameObjectComponents = ActorPrefabs[i].GetComponents<ActionProperties>();
			for (int j = 0; j < gameObjectComponents.Length; j++) {
				Destroy (gameObjectComponents[j]);
			}
		}
	}
}