using UnityEngine;
using System.Collections;

public class FrostRayScript : MonoBehaviour {
	public int openingDownAngle = 56;
	public int startingAngle = 28;
	public int rotateSpeed = 15;
	public int reverse = -1;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.localRotation = Quaternion.Euler(0, reverse * Mathf.PingPong(Time.time*rotateSpeed, openingDownAngle)-startingAngle, 0);
	}
}
