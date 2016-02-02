using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;
using UnityEngine.UI;
using SimpleJSON;

public class SocketToDDListener : MonoBehaviour{
	public SocketIOComponent socket;
	public GameManager GameManagerReference;
	private socketDataDump gameData;
	public Text debugMessage;
	public GameObject loadingScreen;
	public TabletopInitialization meteorInitialize;
	private bool connecting = false;

	void Awake(){
		DontDestroyOnLoad (transform.gameObject);
	}

	public void Start(){
		meteorInitialize = GetComponent<TabletopInitialization> ();
		gameData = GetComponent<socketDataDump> ();
		socket = gameObject.GetComponent<SocketIOComponent>();
		socket.On("open", TestOpen);
		socket.On("test", Test);
		socket.On("error", TestError);
		socket.On("close", TestClose);
		socket.On ("toTabletop", toTableTopHandler);

		if (gameData != null) {			
			socket.On ("toTabletopInit", dataReceivedFromServer);
		}
	}

	public void toTableTopHandler(SocketIOEvent e){
		if (GameManagerReference != null) {
			GameManagerReference.messageReceivedFromServer (e);
		} else {
			GameManagerReference = GameObject.Find ("GameManager").GetComponent<GameManager> ();
			GameManagerReference.messageReceivedFromServer(e);
		}
	}

	//assume it works..
	public void connect(){
		if (!connecting) {
			connecting = true;
			if (debugMessage != null) {
				printMessage ("Opening connection...");
			}
			socket.Connect ();
			StartCoroutine (requestGameInfo ());
		}
	}

	private IEnumerator requestGameInfo(){
		yield return new WaitForSeconds (1.0f);
		printMessage ("Obtaining game data");
		JSONObject sendMsg = new JSONObject (JSONObject.Type.OBJECT);
		sendMsg.AddField ("appID", "DND");
		socket.Emit ("tableTopInit", sendMsg);
	}

	private void printMessage(string msg){
		debugMessage.text = msg;
	}

	private void dataReceivedFromServer(SocketIOEvent e){
		JSONObject incommingData = e.data;
		printMessage ("Game data received!");
		gameData.setMapData (incommingData);
		gameData.setCharacterData (incommingData);
		StartCoroutine (postDataReceived ());
	}
	private IEnumerator postDataReceived(){
		yield return new WaitForSeconds (1.0f);
		yield return StartCoroutine (connectToCollections ());
		yield return new WaitForSeconds (1.0f);
		yield return StartCoroutine (startingGame ());
	}
	private IEnumerator connectToCollections(){		
		printMessage ("Connecting to collections server");
		yield return new WaitForSeconds (1.0f);
		yield return StartCoroutine(meteorInitialize.MeteorInit());
		printMessage ("Connection to collections complete");
	}
	private IEnumerator startingGame(){
		printMessage ("Starting Game...");
		yield return new WaitForSeconds (1.0f);
		loadingScreen.SetActive (true);
		loadingScreen.GetComponent<SceneLoader> ().beginGame ();
	}

	public void TestOpen(SocketIOEvent e)
	{		
		Debug.Log("[SocketIO] Open received: " + e.name + " " + e.data);
	}	
	public void Test(SocketIOEvent e)
	{
		Debug.Log("[SocketIO] Test received: " + e.name + " " + e.data);
	}		
	public void TestError(SocketIOEvent e)
	{
		Debug.Log("[SocketIO] Error received: " + e.name + " " + e.data);

	}	
	public void TestClose(SocketIOEvent e)
	{	
		Debug.Log("[SocketIO] Close received: " + e.name + " " + e.data);
	}
}
