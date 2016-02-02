using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Prizm;
using TouchScript;
using UnityEngine.UI;

//would-be-developer's script
public class RFManager: MonoBehaviour {
	public GameManager GameManagerReference;
	public Dictionary<string, GameObject> instantiatedObjects = new Dictionary<string, GameObject>();//keeps track of objects instantiated in game
	public PrizmObject prizmFactory = new PrizmObject();

	void Start(){
		StartCoroutine(prizmFactory.readJson());
	}

	void OnEnable(){
		prizmFactory.smartTouchStart += smartTouchStartHandler;
		prizmFactory.smartTouchEnd += smartTouchEndHandler;
	}

	void OnDisable(){
		prizmFactory.smartTouchStart -= smartTouchEndHandler;
		prizmFactory.smartTouchEnd -= smartTouchEndHandler;
	}

	//location is in pixel coordinate
	private void smartTouchStartHandler(bindedObject rfAttributes){
		StartCoroutine(GameManagerReference.pieceLanded (rfAttributes));
	}

	private void smartTouchEndHandler(bindedObject rfAttributes){
		StartCoroutine(GameManagerReference.pieceLifted (rfAttributes));
	}

}