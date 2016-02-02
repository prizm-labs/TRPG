using UnityEngine;
using System.Collections;

public class coneOfCold : ActionProperties {
	void OnEnable(){
		Debug.LogError ("activateSpell");
		setSpellPrefab ();
		action ();
	}

	public void setSpellPrefab(){
		for(int i=0; i<GetComponent<Actor>().spellPrefabs.Count;i++){
			if (GetComponent<Actor> ().spellPrefabs [i].name == "coneOfCold") {
				spellPrefab = GetComponent<Actor> ().spellPrefabs [i];
			}
		}
	}

	public void action(){
		if (targets != null) {
			if (EffectSpawnPoint == null) {
				EffectSpawnPoint = gameObject.transform;
			}
			Instantiate (spellPrefab, transform.position, Quaternion.identity);
		}
	}
}
