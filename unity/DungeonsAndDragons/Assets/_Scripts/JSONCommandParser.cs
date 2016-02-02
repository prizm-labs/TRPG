using UnityEngine;
using System.Collections;
using System.IO;
using SimpleJSON;

public class JSONCommandParser : MonoBehaviour {
	private JSONClass j;

	public IEnumerator readJson(){
		j = null;
		string fileName = "trpg.json";
		string jsonPath = Application.streamingAssetsPath + "/" + fileName;
		StreamReader sr;

		//windows
		if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor) {
			Debug.LogError ("Found windows");
			sr = new StreamReader (jsonPath);
			yield return sr;
			j = JSON.Parse (sr.ReadToEnd ()) as JSONClass;
		}
		
		//android 
		else if (Application.platform == RuntimePlatform.Android) {
			WWW www = new WWW (jsonPath);
			yield return www;
			j = JSON.Parse (www.text.ToString ()) as JSONClass;
			Debug.LogError ("JSON found! Number of IDs located: " + j["rfBindings"].Count);
		} else 
			Debug.LogError ("Android and Windows only");
			yield return null;
	}

}
