using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAutoCounter : MonoBehaviour {

	public static int hitCount = 0;
	const string scoreStrFormat = "{0}%";

	Text text;
	Animator animator;

	void Awake () {
		text = GetComponent<Text> ();
		animator = GetComponent<Animator> ();
	}
	public void UpdateText () {
		hitCount ++;
		float tmpResult = 100f - (float) (Destroyer.totalMissCount / 3.0f) / (float) NoteGenerator.totalNode * 100f;
		text.text = string.Format (scoreStrFormat, tmpResult.ToString ("F2"));
		animator.SetTrigger ("TriggerShake");
	}
}
