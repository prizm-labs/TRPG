using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
//wowzers
public class TileManager : MonoBehaviour {
	public GameObject horizontalWall;
	public GameObject verticalWall;
	public DungeonTile[] allTiles;
	private Vector3 wallLocation0 = new Vector3(0.325f, 0f, 0.925f);
	private Vector3 wallLocation1 = new Vector3(0.665f, 0f, 0.45f);
	private Vector3 wallLocation2 = new Vector3(0.325f, 0f, 0f);
	private Vector3 wallLocation3 = new Vector3(0f, 0f, 0.45f);

	void Start(){
		allTiles = GetComponentsInChildren<DungeonTile> ();
	}

	public void clearHeroMovementHighlights(string idOfActor){
		for (int i = 0; i < allTiles.Length; i++) {
			if (allTiles [i].actorMoveCount.ContainsKey (idOfActor)) {
				allTiles [i].disableMoveSpaces (idOfActor);
			}
		}
	}
			
	public void enabledActorToStandAll(string idOfActor){
			for(int i =0; i<allTiles.Length;i++){
				if (!allTiles [i].actorMoveCount.ContainsKey (idOfActor)) {
					allTiles [i].actorMoveCount.Add (idOfActor, 1);
				}
			}
		}

	public void GenerateWalls(JSONObject wallData){
		Debug.LogError ("Generating walls");
		for (int i = 0; i < wallData.Count; i++) {
			int xPos;
			int yPos;
			xPos = System.Int32.Parse (wallData [i] ["_tileLocation"] [0].str);
			yPos = System.Int32.Parse (wallData [i] ["_tileLocation"] [1].str);
			int[] walls = new int[wallData [i] ["_tileWalls"].Count];
			for (int j = 0; j < walls.Length; j++) {
				walls [j] = System.Int32.Parse (wallData [i] ["_tileWalls"] [j].str);
			}

			CreateWallAt (xPos, yPos, walls);
		}
				/*
			xPos = (int)wallData ["_tileLocation"] [0].str;
			yPos = (int)wallData ["_tileLocation"] [1].str;

			CreateWallAt ();*/
		//	}
	}
	private void CreateWallAt(int Xpos, int Ypos, params int[] loc){
		foreach (Transform tile in gameObject.transform) {
			DungeonTile objectProperties = tile.gameObject.GetComponent<DungeonTile>();
			if(objectProperties.tileLocationX == Xpos && objectProperties.tileLocationY == Ypos){
				for(int i =0; i<loc.Length; i++){
					switch (loc[i]) {
					case 0:
						objectProperties.walls[0] = Instantiate(horizontalWall, tile.position, horizontalWall.transform.rotation) as GameObject;
						objectProperties.walls[0].transform.parent = tile;
						objectProperties.walls[0].transform.localPosition = wallLocation0;
						break;
					case 1:
						objectProperties.walls[1] = Instantiate(verticalWall, tile.position,verticalWall.transform.rotation) as GameObject;
						objectProperties.walls[1].transform.parent = tile;
						objectProperties.walls[1].transform.localPosition = wallLocation1;
						break;
					case 2:
						objectProperties.walls[2] = Instantiate(horizontalWall, tile.position, horizontalWall.transform.rotation) as GameObject;
						objectProperties.walls[2].transform.parent = tile;
						objectProperties.walls[2].transform.localPosition = wallLocation2;
						break;
					case 3:
						objectProperties.walls[3] = Instantiate(verticalWall, tile.position, verticalWall.transform.rotation) as GameObject;
						objectProperties.walls[3].transform.parent = tile;
						objectProperties.walls[3].transform.localPosition = wallLocation3;
						break;
					default:
						Debug.LogError(loc + " is not a valid location");
						break;
					}
					//found the tile to create wall in, exiting foreach loop
				}
			}
		}
	}

	public void RemoveWallAt(int Xpos, int Ypos, List<int> loc){
		foreach (Transform tile in gameObject.transform) {
			DungeonTile objectProperties = tile.gameObject.GetComponent<DungeonTile>();
			if(objectProperties.tileLocationX == Xpos && objectProperties.tileLocationY == Ypos){
				for(int i =0; i<loc.Count; i++){
					switch (loc[i]) {
					case 0:
						if(objectProperties.walls[0]){
							Destroy (objectProperties.walls[0].gameObject);
							objectProperties.walls[0]= null;
						}
						break;
					case 1:
						if(objectProperties.walls[1]){
							Destroy (objectProperties.walls[1].gameObject);
							objectProperties.walls[1]= null;
						}
						break;
					case 2:
						if(objectProperties.walls[2]){
							Destroy (objectProperties.walls[2].gameObject);
							objectProperties.walls[2]= null;
						}
						break;
					case 3:
						if(objectProperties.walls[3]){
							Destroy (objectProperties.walls[3].gameObject);
							objectProperties.walls[3]= null;
						}
						break;
					default:
						Debug.LogError(loc + " is not a valid location");
						break;
					}
					//found the tile to create wall in, exiting foreach loop
				}
				break;
			}
		}
	}



}
