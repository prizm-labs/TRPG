using UnityEngine;
using System.Collections;

public class dualSwords : ActionProperties {
	void OnEnable(){
		Debug.LogError ("activateSpell");
		setSpellPrefab ();
		action ();
	}

	public void setSpellPrefab(){ //get spellprefab
		for(int i=0; i<GetComponent<Actor>().spellPrefabs.Count;i++){
			if (GetComponent<Actor> ().spellPrefabs [i].name == "dualSwords") {
				spellPrefab = GetComponent<Actor> ().spellPrefabs [i];
			}
			if (GetComponent<Actor> ().spellPrefabs [i].name == "bloodEffect2") {
				spellPrefab2 = GetComponent<Actor> ().spellPrefabs [i];
			}
		}

	}

	public void action(){ //use spell

		if (targets != null) {
			if (EffectSpawnPoint == null) {
				EffectSpawnPoint = gameObject.transform;
			}

			for (int i = 0; i < targets.Length; i++) {
				GameObject slashEffect = Instantiate (spellPrefab, transform.position, Quaternion.identity) as GameObject;
				slashEffect.transform.LookAt (targets [i].transform);
				GameObject impactEffect = Instantiate (spellPrefab2, targets [i].transform.position, Quaternion.identity) as GameObject;
				impactEffect.transform.LookAt (transform);
				targets [i].GetComponent<Actor> ().changeHP (-damageAmount);
			}
		}
	}
}
