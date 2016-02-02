using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;
using SimpleJSON;
using Prizm;

[HideInInspector]
public class SocketToJSON : MonoBehaviour{
	private SocketIOComponent socket;
	public RFManager RFManagerReference;
	private List<string> rfidList = new List<string>();
	public float tagReadDelay = 0.1f;
	void Awake(){
		DontDestroyOnLoad (transform.gameObject);
	}

	void Start() {
		socket = gameObject.GetComponent<SocketIOComponent>();
		socket.On("smarttouch-start", SmartTouch);
		socket.On("smarttouch-end", SmartTouch);
	}

	//when receiving smart touch data, call this function:
	public void SmartTouch(SocketIOEvent e){
		string RFID = e.data.GetField("tagId").str;
		if (!rfidList.Contains (RFID)) {
			rfidList.Add (RFID);
			string typeOfTouch = e.name;
			Vector3 smartTouchPoint = new Vector3 ();
			touchType ST;
			ST = enumerateString (typeOfTouch);
			RFID = filterRFID (RFID);

			smartTouchPoint.x = e.data.GetField ("x").n;
			smartTouchPoint.y = Camera.main.pixelHeight - e.data.GetField ("y").n;
			Debug.LogError ("Smart touch detected!");
			Debug.LogError (RFID);
			Debug.LogError (smartTouchPoint);
			if (RFManagerReference != null) {
				RFManagerReference.prizmFactory.RFIDEventManager (RFID, ST, smartTouchPoint);
			} else {
				RFManagerReference = GameObject.Find ("GameManager").GetComponent<RFManager> ();
				RFManagerReference.prizmFactory.RFIDEventManager (RFID, ST, smartTouchPoint);
			}
			StartCoroutine(delayTagRead (RFID));
		}
	}
	IEnumerator delayTagRead(string rfid){
		yield return new WaitForSeconds (tagReadDelay);
		rfidList.Remove (rfid);
	}

	private touchType enumerateString(string str){
		if (str == "smarttouch-start") {
			return touchType.smartTouchStart;
		} else
			return touchType.smartTouchEnd;
	}

	private string filterRFID(string ID){
		if (ID.Length == 12) {
			return ID.Substring (0, ID.Length - 1);
		} else
			return ID;

	}
}