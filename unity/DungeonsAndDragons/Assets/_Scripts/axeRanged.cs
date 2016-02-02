using UnityEngine;
using System.Collections;

public class axeRanged : ActionProperties {
	void OnEnable(){
		Debug.LogError ("activateSpell");
		setSpellPrefab ();
		action ();
	}
	public void setSpellPrefab(){ //get spellprefab
		for(int i=0; i<GetComponent<Actor>().spellPrefabs.Count;i++){
			if (GetComponent<Actor> ().spellPrefabs [i].name == "axeRanged") {
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
				GameObject acidBreath = Instantiate (spellPrefab, EffectSpawnPoint.position, Quaternion.identity) as GameObject;
				EffectSettings projectileEffect = acidBreath.GetComponent<EffectSettings> ();
				projectileEffect.LayerMask = LayerMask.GetMask (LayerMask.LayerToName (targets [i].gameObject.layer));
				projectileEffect.Target = targets [i].gameObject;
				projectileEffect.CollisionEnter += (n, e) => {
					if (e.Hit.transform.GetComponent<Actor> ()) {
						Actor actorProperties = e.Hit.transform.GetComponent<Actor> ();
						Debug.LogError (e.Hit.transform.name);
						actorProperties.changeHP (-damageAmount);
					}
				};
			}
		}
	}
}
