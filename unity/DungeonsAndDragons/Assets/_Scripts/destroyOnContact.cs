using UnityEngine;
using System.Collections;

public class destroyOnContact : MonoBehaviour {

	void OnCollisionEnter(Collision other){
		foreach (ContactPoint contact in other.contacts){
			//if(other.gameObject.tag != "earth"){
				Destroy (other.gameObject);
			//}
		}
	}
	void OnTriggerEnter(Collider other){
		if(other.transform.tag == "sunEnergy" || other.transform.tag == "pea"){
			Destroy(other.gameObject);
		}
	}
}