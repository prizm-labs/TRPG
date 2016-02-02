using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Actor : MonoBehaviour {
	public GameObject highlightTargetPrefab;
	public GameObject deathEffectPrefab;
	public bool playerTurn = false;
	public string registeredPlayerID = "";
	public string registeredPlayerName = "";
	public string ActorID = "";
	public bool onSurface = false;

	public int HP;
	public int AC;
	public int maxMovementSpace;

	public int currentMovementSpace;

	public List<GameObject> actorTargets = new List<GameObject>();
	public List<Actor>selectedTargets = new List<Actor>();
	public List<GameObject> spellPrefabs = new List<GameObject>();
	public GameObject currentTile;
	public Transform EffectSpawnPoint;

	private LineRenderer lineTarget;
	public Material lineMaterial;


	public ActionProperties[] listOfActions;
	//this is what we'll return
	public List<string> abilityNameToIndex = new List<string>();

	void Start(){
		lineTarget = GetComponent<LineRenderer> ();
}

	public void updateMovementCount(int movedSpaces){
		currentMovementSpace = currentMovementSpace - (currentMovementSpace - (movedSpaces-1));
	}
	public void resetMovement(){
		currentMovementSpace = maxMovementSpace;
	}
	public void highlightAvailableSpaces(){
		StartCoroutine(currentTile.GetComponent<DungeonTile>().highlightMoveSpaces(ActorID, currentMovementSpace+1));
  	}

	public void performAction(string nameOfAbility, params GameObject[] target){
		int actionIndex = abilityNameToIndex.FindIndex (abilityString => abilityString == nameOfAbility);
		listOfActions [actionIndex].enabled = false;
		listOfActions [actionIndex].EffectSpawnPoint = EffectSpawnPoint;
		listOfActions [actionIndex].targets = target;
		listOfActions [actionIndex].enabled = true;
	}


	public void changeHP(int amount){
		HP += amount;
		if (HP <= 0) {
			StartCoroutine ("death");
			Debug.LogError ("Player: " + gameObject.name + " is dead");
		}
	}

	public IEnumerator death(){
		GetComponent<Collider> ().enabled = false;
		currentTile.GetComponent<DungeonTile> ().isTileOccupied = false;
		GameObject.Find ("ActorManager").GetComponent<ActorManager> ().someoneDied(ActorID);
		deathEffectPrefab.SetActive (true);
		yield return new WaitForSeconds (2.0f);
		Destroy (gameObject);
	}


	public List<string> scanActors(float radius, int type){
		actorTargets.Clear ();

		bool ignoreWall = false;
		int thisLayer = 0;
		if (type == 0) { //enemy, do not ignore wall
			ignoreWall = false;
			if (gameObject.layer == LayerMask.GetMask ("Hero")) {
				thisLayer = LayerMask.GetMask ("Monster");
			} else
				thisLayer = LayerMask.GetMask ("Hero");
		} else if (type == 1) { //enemy, ignore wall
			ignoreWall = true;
			if (gameObject.layer == LayerMask.GetMask ("Hero")) {
				thisLayer = LayerMask.GetMask ("Monster");
			} else
				thisLayer = LayerMask.GetMask ("Hero");
		} else if (type == 2) {//ally, do not ignore wall
			ignoreWall = false;
			if (gameObject.layer == LayerMask.GetMask ("Hero")) {
				thisLayer = LayerMask.GetMask ("Hero");
			} else
				thisLayer = LayerMask.GetMask ("Monster");
		} else if (type == 3) {//ally, ignore wall
			ignoreWall = true;
			if (gameObject.layer == LayerMask.GetMask ("Hero")) {
				thisLayer = LayerMask.GetMask ("Hero");
			}else
				thisLayer = LayerMask.GetMask("Monster");
		} else if (type == 4) {//hero and monster do not ignore wall;
			ignoreWall = false;
			thisLayer = LayerMask.GetMask ("Monster", "Hero");
		} else if (type == 5) {//hero and monster ignore wall
			ignoreWall = true;
			thisLayer = LayerMask.GetMask ("Monster", "Hero");
		} else
			Debug.LogError ("Invalid Type");
	
		List<string> scannedActors = new List<string>();
		RaycastHit[] hits;
		float angle =0.0f;
		do {
			Vector3 direction = transform.TransformDirection (Vector3.forward);
			float distanceToWall = radius;
			if(!ignoreWall){
				RaycastHit toWall;
				if(Physics.Raycast(transform.position,direction, out toWall, radius, LayerMask.GetMask("DungeonWall"))){
					distanceToWall = toWall.distance;
				}
			}
			hits = Physics.RaycastAll (transform.position, direction, distanceToWall, thisLayer);
			for(int i =0; i<hits.Length; i++){
				RaycastHit hit = hits[i];
				if (!actorTargets.Contains (hit.transform.gameObject) ){
					actorTargets.Add(hit.collider.gameObject);
					scannedActors.Add(hit.collider.gameObject.name);
				}
			}		
			transform.Rotate (0, angle, 0);
			angle +=0.05f;
		} while (angle < 360);
		Debug.LogError ("Done Scanning");
		return scannedActors;
	}


	public void highlightTargets(){
		drawLineToTarget ();
		highlightTarget ();
	}

	public void drawLineToTarget(){
		
		for (int i = 0; i < selectedTargets.Count; i++) {		
			selectedTargets [i].lineTarget.enabled = true;	
			selectedTargets[i].lineTarget.material = lineMaterial;
			selectedTargets[i].lineTarget.SetWidth (0.0f, 0.5f);
			selectedTargets[i].lineTarget.SetColors (new Color (255, 255, 255, 255), new Color (255, 255, 255, 0));
			selectedTargets[i].lineTarget.SetPosition (0, transform.position);
			selectedTargets[i].lineTarget.SetPosition (1, selectedTargets [i].transform.position);
		}

	}

	public void highlightTarget(){
		for (int i = 0; i < selectedTargets.Count; i++) {
			selectedTargets [i].highlighted (true);
		}
	}

	public void highlighted(bool toggle){
		highlightTargetPrefab.SetActive (toggle);
	}

	public void disableHighlightOfTargets(){
		for (int i = 0; i < selectedTargets.Count; i++) {
			if (selectedTargets [i] != null) {
				selectedTargets [i].highlighted (false);
			}
		}
		for (int i = 0; i < selectedTargets.Count; i++) {		
			if (selectedTargets [i] != null) {
				selectedTargets [i].lineTarget.enabled = false;	
			}
		}

		selectedTargets.Clear ();


		
	}
}

