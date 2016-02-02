using UnityEngine;
using System.Collections;

public class MassCureWound : ActionProperties {
	void Start(){
		setSpellPrefab ();
		action ();
	}

	public void setSpellPrefab(){
		for(int i=0; i<GetComponent<Actor>().spellPrefabs.Count;i++){
			if (GetComponent<Actor> ().spellPrefabs [i].name == "MassCureWound") {
				spellPrefab = GetComponent<Actor> ().spellPrefabs [i];
			}
		}
	}

	public void action(){
		for (int i = 0; i<targets.Length; i++) {
			Instantiate (spellPrefab, targets [i].transform.position, Quaternion.identity);
		}
	}
}
