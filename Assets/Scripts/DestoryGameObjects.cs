using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestoryGameObjects : MonoBehaviour {

	public DestroyThisGameObject[] gameObjectsToDestroy;

	// Use this for initialization
	void Start () {
		gameObjectsToDestroy = FindObjectsOfType <DestroyThisGameObject>();
	}
	
	// Update is called once per frame
	void OnDestroy () {
		ThanosSnap ();
	}

	void ThanosSnap() {
		for (int i = 0; i < gameObjectsToDestroy.Length; i++) {
			if (gameObjectsToDestroy [i].gameObject)
				Destroy (gameObjectsToDestroy [i].gameObject);
		}
	}
}
