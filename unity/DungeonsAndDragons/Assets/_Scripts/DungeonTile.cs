using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DungeonTile : MonoBehaviour {
	/// adjacent tiles
	/// list index 0 - top tile
	/// list index 1 - right tile
	/// list index 2 - bottom tile
	/// list index 3 - left tile
	public List<DungeonTile> adjacentTiles = new List<DungeonTile>();

	/// blocked tiles
	/// array index 0 - top tile
	/// array index 1 - right tile
	/// array index 2 - bottom tile
	/// array index 3 - left tile
	public List<GameObject> walls = new List<GameObject>();

	public int tileLocationX;
	public int tileLocationY;
	public bool isTileOccupied = false;
	public bool isHighlighted = false;
	public GameObject highlight;

	public string RoomName;

	private Vector3 highlightSpawnLocation = new Vector3 (0.325f, 0f, 0.45f);
	public Dictionary<string, int> actorMoveCount = new Dictionary<string,int> ();
	public int currentSpace = 0;

	void Start () {
		setAdjacentTiles (); 
	}

	public bool ActorAllowedToStand(string idOfActor){
		return actorMoveCount.ContainsKey (idOfActor);
	}


	public IEnumerator highlightMoveSpaces(string idOfActor, int numSpaces){
		yield return new WaitForSeconds (0.01f);
		int greaterNumber = numSpaces;
		if (actorMoveCount.ContainsKey (idOfActor)) {
			if (numSpaces > actorMoveCount [idOfActor]) {
				greaterNumber = numSpaces;
			} else
				greaterNumber = actorMoveCount [idOfActor];
		}

		if (greaterNumber > 0 && !isTileOccupied) {
			highlight.SetActive (true);
			if(actorMoveCount.ContainsKey(idOfActor)){
				actorMoveCount[idOfActor] = greaterNumber;
			}
			else
				actorMoveCount.Add (idOfActor, greaterNumber);
			currentSpace = greaterNumber;
			isHighlighted = true;

			for (int i = 0; i < 4; i++) {
				if (walls [i] == null && adjacentTiles [i] != null) {
					if (!adjacentTiles[i].isHighlighted &&  !adjacentTiles [i].isTileOccupied) {
						StartCoroutine(adjacentTiles [i].highlightMoveSpaces (idOfActor, greaterNumber - 1));	
					}
				}
			}

		}
		yield return null;

	}


	public void disableMoveSpaces(string idOfActor){
			actorMoveCount.Remove (idOfActor);
			highlight.SetActive (false);
			isHighlighted = false;
	}

	public void setAdjacentTiles(){
		foreach (Transform tiles in transform.parent.transform) {
			if((tiles.gameObject.GetComponent<DungeonTile>().tileLocationX == tileLocationX) && (tiles.gameObject.GetComponent<DungeonTile>().tileLocationY == tileLocationY + 1)){
				adjacentTiles[0] = tiles.gameObject.GetComponent<DungeonTile>();
			}
			if((tiles.gameObject.GetComponent<DungeonTile>().tileLocationX == tileLocationX + 1) && (tiles.gameObject.GetComponent<DungeonTile>().tileLocationY == tileLocationY)){
				adjacentTiles[1] = tiles.gameObject.GetComponent<DungeonTile>();
			}
			if((tiles.gameObject.GetComponent<DungeonTile>().tileLocationX == tileLocationX) && (tiles.gameObject.GetComponent<DungeonTile>().tileLocationY == tileLocationY - 1)){
				adjacentTiles[2] = tiles.gameObject.GetComponent<DungeonTile>();
			}
			if((tiles.gameObject.GetComponent<DungeonTile>().tileLocationX == tileLocationX - 1) && (tiles.gameObject.GetComponent<DungeonTile>().tileLocationY == tileLocationY)){
				adjacentTiles[3] = tiles.gameObject.GetComponent<DungeonTile>();
			}
		}
	}



}
