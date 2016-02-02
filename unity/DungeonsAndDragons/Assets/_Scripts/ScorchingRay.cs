using UnityEngine;
using System.Collections;

public class ScorchingRay : ActionProperties {
	void Start(){
		setSpellPrefab ();
		action ();
	}

	public void setSpellPrefab(){
		for(int i=0; i<GetComponent<Actor>().spellPrefabs.Count;i++){
			if (GetComponent<Actor> ().spellPrefabs [i].name == "ScorchingRay") {
				spellPrefab = GetComponent<Actor> ().spellPrefabs [i];
			}
		}
	}

	public void action(){
		for (int i = 0; i<targets.Length; i++) {
			GameObject scorchingRay = Instantiate (spellPrefab, EffectSpawnPoint.transform.position, Quaternion.identity)as GameObject;
			EffectSettings projectileEffect = scorchingRay.GetComponent<EffectSettings>();
			projectileEffect.LayerMask = LayerMask.GetMask (LayerMask.LayerToName(targets [i].gameObject.layer));
			projectileEffect.Target = targets[i].gameObject;
			projectileEffect.CollisionEnter += (n, e) => { Debug.Log(e.Hit.transform.name); };
		}
	}
}
