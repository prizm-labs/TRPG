using UnityEngine;
using System.Collections;

public class AuraBehavior : MonoBehaviour {
	public GameObject activate;
	public GameObject deactivate;
	public GameObject specialEffect;
	public GameObject specialBuff;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	void OnEnable(){
		//GameObject activateEffect = Instantiate (activate, transform.position, Quaternion.Identity);
	}

	void OnDisable(){

		//deactivate.SetActive (true);
	}
}
