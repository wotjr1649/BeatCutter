using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : MonoBehaviour {

	public static int totalMissCount = 0;
	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag == "Node") {
			totalMissCount ++;
			UIRootController.instance.UpdateScoreText ();
		}
	}
}
