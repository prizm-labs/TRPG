using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//huhuhu
public class CreateWall : MonoBehaviour {
	public GameObject horizontalWall;
	public GameObject verticalWall;

	private Vector3 wallLocation0 = new Vector3(0.325f, 0f, 0.925f);
	private Vector3 wallLocation1 = new Vector3(0.665f, 0f, 0.45f);
	private Vector3 wallLocation2 = new Vector3(0.325f, 0f, 0f);
	private Vector3 wallLocation3 = new Vector3(0f, 0f, 0.45f);

	// Use this for initialization
	void Start () {
		CreateWallAt (0, 0, new List<int> {0, 2});
		CreateWallAt (1, 0, new List<int> {0, 2});
		CreateWallAt (2, 0, new List<int> {0, 2});
		CreateWallAt (3, 0, new List<int> {1, 2});
		CreateWallAt (4, 0, new List<int> {2,3});
		CreateWallAt (5, 0, new List<int> {2});
		CreateWallAt (6, 0, new List<int> {1, 2});
		CreateWallAt (7, 0, new List<int> {2,3});
		CreateWallAt (8, 0, new List<int> {2});
		CreateWallAt (9, 0, new List<int> {2});
		CreateWallAt (10, 0, new List<int> {2});
		CreateWallAt (11, 0, new List<int> {1, 2});
		CreateWallAt (0, 1, new List<int> {2,3});
		CreateWallAt (1, 1, new List<int> {2});
		CreateWallAt (2, 1, new List<int> {2});
		CreateWallAt (4, 1, new List<int> {0});
		CreateWallAt (5, 1, new List<int> {0});
		CreateWallAt (6, 1, new List<int> {0});
		CreateWallAt (8, 1, new List<int> {1,2});
		CreateWallAt (9, 1, new List<int> {0,3});
		CreateWallAt (10, 1, new List<int> {0});
		CreateWallAt (11, 1, new List<int> {2});
		CreateWallAt (0, 2, new List<int> {3});
		CreateWallAt (3, 2, new List<int> {1});
		CreateWallAt (4, 2, new List<int> {2,3});
		CreateWallAt (5, 2, new List<int> {2});
		CreateWallAt (6, 2, new List<int> {2});
		CreateWallAt (8, 2, new List<int> {1,2});
		CreateWallAt (9, 2, new List<int> {2,3});
		CreateWallAt (10, 2, new List<int> {2});
		CreateWallAt (11, 2, new List<int> {1});
		CreateWallAt (0, 3, new List<int> {3});
		CreateWallAt (1, 3, new List<int> {1});
		CreateWallAt (2, 3, new List<int> {3});
		CreateWallAt (3, 3, new List<int> {1});
		CreateWallAt (4, 3, new List<int> {3});
		CreateWallAt (8, 3, new List<int> {1});
		CreateWallAt (9, 3, new List<int> {3});
		CreateWallAt (11, 3, new List<int> {1});
		CreateWallAt (0, 4, new List<int> {0, 3});
		CreateWallAt (1, 4, new List<int> {0});
		CreateWallAt (2, 4, new List<int> {0});
		CreateWallAt (3, 4, new List<int> {0});
		CreateWallAt (4, 4, new List<int> {0});
		CreateWallAt (5, 4, new List<int> {0});
		CreateWallAt (6, 4, new List<int> {0});
		CreateWallAt (7, 4, new List<int> {0});
		CreateWallAt (8, 4, new List<int> {0,1});		
		CreateWallAt (9, 4, new List<int> {0,3});
		CreateWallAt (10, 4, new List<int> {0});
		CreateWallAt (11, 4, new List<int> {0});
	}

	public void CreateWallAt(int Xpos, int Ypos, List<int> loc){
		foreach (Transform tile in gameObject.transform) {
			DungeonTile objectProperties = tile.gameObject.GetComponent<DungeonTile>();
			if(objectProperties.tileLocationX == Xpos && objectProperties.tileLocationY == Ypos){
				for(int i =0; i<loc.Count; i++){
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
