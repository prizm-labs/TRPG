using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Runtime.InteropServices;
using Prizm;
using System.Collections.Generic;

public class AndroidReceiver : MonoBehaviour {
	private RFManager RFManagerReference;
	string javaMessage = "uninitialized from Unity";
	int xPos;
	int yPos;
	string ID = "";
	public int num = 0;

	void Start(){
		RFManagerReference = GameObject.Find ("GameManager").GetComponent<RFManager> ();
		//StartCoroutine(checkJSON.readJson());
		// Acces the android java receiver we made

	}

	void Update()
	{    
#if UNITY_ANDROID
		num++;
		using (AndroidJavaClass ajc = new AndroidJavaClass("com.prizm.unityreceiver.MyReceiver")) {
			if (ajc.GetStatic<bool> ("hasMessage")) {
				//OtherSetText();
				javaMessage = ajc.GetStatic<string>("recv");
				Debug.Log ("Java Plugin has a message for us:" + javaMessage + " " + ID.ToString() + "THE LENGTH OF ID IS: "+ ID.Length);
				xPos = ajc.GetStatic<int>("xCoord");
				yPos = ajc.GetStatic<int>("yCoord");
				xPos = convertTouchCoordinateX(xPos);
				yPos = convertTouchCoorindateY(yPos);
				yPos = Camera.main.pixelHeight - yPos;
				ID = javaMessage;
				ID = filterRFID (ID);
				RFManagerReference.prizmFactory.RFIDEventManager (ID, touchType.smartTouchStart, new Vector3 (xPos, yPos, 0));

				ajc.SetStatic<bool>("hasMessage", false);
			}
		}
#endif
	}
	private string filterRFID(string ID){
		if (ID.Length == 12) {
			return ID.Substring (0, ID.Length - 1);
		} else if (ID.Length == 14) {
			return ID.Substring (1, ID.Length - 3);
		} else if (ID.Length == 13) {
			return ID.Substring (1, ID.Length - 2);
		}else
			return ID;
		
	}

	private int convertTouchCoordinateX(int pos){
		float newPos = 0f;
		newPos = pos / 32768.0f;
		newPos = newPos * 1920.0f;
		return (int)newPos;
	}
	private int convertTouchCoorindateY(int pos){
		float newPos = 0f;
		newPos = pos / 32768.0f;
		newPos = newPos * 1080.0f;
		return (int)newPos;
	}
}
