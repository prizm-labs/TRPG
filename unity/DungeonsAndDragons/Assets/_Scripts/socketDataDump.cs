using UnityEngine;
using System.Collections;
using SimpleJSON;
using SocketIO;

public class socketDataDump : MonoBehaviour {	
	public JSONObject mapData;
	public JSONObject characterData;

	public void setMapData(JSONObject data){
		if (mapData == null) {
			mapData = data ["data"] ["mapData"];
		}
	}

	public void setCharacterData(JSONObject data){
		if (characterData == null) {
			characterData = data ["data"] ["characterData"];
		}
	}
}
