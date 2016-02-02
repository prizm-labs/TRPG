using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;
using UnityEngine.UI;
using SimpleJSON;

public class SocketMessenger : MonoBehaviour{
	public SocketIOComponent socket;

	public delegate void ReceivedData(JSONObject data);
	public ReceivedData receivedData;

	public delegate void ReceivedMessage(JSONObject data);
	public ReceivedMessage receivedMessage;

	public delegate void DidOpen(JSONObject data);
	public DidOpen didOpen;

	public delegate void DidError(JSONObject data);
	public DidError didError;

	public delegate void DidClose(JSONObject data);
	public DidClose didClose;

	public static SocketMessenger Instance;

	void Awake(){
		Instance = this;
		DontDestroyOnLoad (transform.gameObject);
	}

	public void Start(){
		socket = gameObject.GetComponent<SocketIOComponent> ();
		socket.On ("open", onOpen);
		socket.On ("error", onError);
		socket.On ("close", onClose);

		socket.On ("toTabletop", messageReceivedFromServer);
		socket.On ("tabletopData", dataReceivedFromServer);
	}

	public void messageReceivedFromServer(SocketIOEvent e)
	{
		JSONObject data = e.data;
		Debug.Log("data received!");
		receivedMessage (data);
	}

	public void dataReceivedFromServer(SocketIOEvent e)
	{
		JSONObject data = e.data;
		Debug.Log("Init data received!");
		receivedData (data);
	}

	public void requestData(string appId, string topic)
	{
		Debug.Log("Requesting initialization data"+appId);
		JSONObject msg = new JSONObject (JSONObject.Type.OBJECT);
		msg.AddField ("appID", appId);
		msg.AddField ("topic", topic);
		socket.Emit ("tableTopData", msg);
	}

	public void sendMessageToPlayer(JSONObject data, string playerId)
	{
		// JSONObject data = new JSONObject (JSONObject.Type.OBJECT);
		data.AddField ("to", playerId);
		socket.Emit ("fromTableTop", data);
	}

	public void sendMessageToAll(JSONObject data) 
	{
		// JSONObject data = new JSONObject (JSONObject.Type.OBJECT);
		data.AddField ("to", "all");
		socket.Emit ("fromTableTop", data);
	}

	public void onOpen(SocketIOEvent e)
	{		
		Debug.Log("[SocketIO] Open received: " + e.name + " " + e.data);
		JSONObject data = e.data;
		didOpen (data);
	}	

	public void onError(SocketIOEvent e)
	{
		Debug.Log("[SocketIO] Error received: " + e.name + " " + e.data);
		JSONObject data = e.data;
		didError (data);
	}	
	public void onClose(SocketIOEvent e)
	{	
		Debug.Log("[SocketIO] Close received: " + e.name + " " + e.data);
		JSONObject data = e.data;
		didClose (data);
	}
}
