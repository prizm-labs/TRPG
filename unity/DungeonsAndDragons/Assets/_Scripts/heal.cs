using UnityEngine;
using System.Collections;

public class heal : ActionProperties {

	void OnEnable(){
		Debug.LogError ("activateSpell");
		setSpellPrefab ();
		action ();
	}
	public void setSpellPrefab(){ //get spellprefab
		for(int i=0; i<GetComponent<Actor>().spellPrefabs.Count;i++){
			if (GetComponent<Actor> ().spellPrefabs [i].name == "heal") {
				spellPrefab = GetComponent<Actor> ().spellPrefabs [i];
			}
		}
	}

	public void action(){ //use spell
		if (targets != null) {
			if (EffectSpawnPoint == null) {
				EffectSpawnPoint = gameObject.transform;
			}
			for (int i = 0; i < targets.Length; i++) {
				GameObject acidBreath = Instantiate (spellPrefab, targets [i].transform.position, Quaternion.identity) as GameObject;
				EffectSettings projectileEffect = acidBreath.GetComponent<EffectSettings> ();
				projectileEffect.LayerMask = LayerMask.GetMask (LayerMask.LayerToName (targets [i].gameObject.layer));
				projectileEffect.Target = targets [i].gameObject;
				if (targets [i].GetComponent<Actor> ()) {
					Actor actorProperties = targets [i].GetComponent<Actor> ();
					actorProperties.changeHP (damageAmount);
				}
			}
		}
	}
}
