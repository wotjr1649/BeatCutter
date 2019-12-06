using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public enum BandType {
	Band1 = 1,
	Band2 = 2,
	Band3 = 3,
	Band4 = 4,
	Band5 = 5,
	Band6 = 6,
}

public class NoteGenerator : MonoBehaviour {

	[Header ("Note Prefabs")]
	public GameObject[] notePrefabs;
	public float speed = 1f;
	public bool useLerp = true;
	public float boostedTimeScale = 5f;
	public float boostedDuringTime = 0.5f;

	[Header ("Transform References")]
	public Transform notesParent;

	public Transform startPointsParent;
	[SerializeField] private Transform[] startPoints;
	public Transform endPointsParent;
	[SerializeField] private Transform[] endPoints;
	
	public GameObject NoteKochGenerator, Destroyer;

	private bool noteRegister = true;
	
	public static int totalNode = 0;
	
	private void Update()
	{
		if (noteRegister)
		{
			StartCoroutine(Register());
			noteRegister = false;
		}
	}


	IEnumerator Register()
	{
		yield return new WaitForSeconds(0.001f);
		startPoints = startPointsParent.GetComponentsInChildren<Transform>(true);
		endPoints = endPointsParent.GetComponentsInChildren<Transform>(true);

		AdvancedAudioAnalyzer.onBassTrigger += OnBassTrigger;
		AdvancedAudioAnalyzer.onBand2Trigger += OnBand2Trigger;
		AdvancedAudioAnalyzer.onBand3Trigger += OnBand3Trigger;
		AdvancedAudioAnalyzer.onBand4Trigger += OnBand4Trigger;
		AdvancedAudioAnalyzer.onBand5Trigger += OnBand5Trigger;
		AdvancedAudioAnalyzer.onBand6Trigger += OnBand6Trigger;
		NoteKochGenerator.SetActive(false);
		Destroyer.SetActive(false);
	}

	#region Audio Analyzer Callbacks
	public void OnBassTrigger () {
		GameObject go = Instantiate (notePrefabs[0]);
		go.name = "Band1Note";
		go.transform.SetParent (notesParent);
		go.GetComponent<FlowNote> ().InitNote (BandType.Band1, startPoints[(int) BandType.Band1], endPoints[(int) BandType.Band1], speed, useLerp);
		totalNode ++;
	}

	public void OnBand2Trigger () {
		// Debug.Log ("2");
		GameObject go = Instantiate (notePrefabs[1]);
		go.name = "Band2Note";
		go.transform.SetParent (notesParent);
		go.GetComponent<FlowNote> ().InitNote (BandType.Band2, startPoints[(int) BandType.Band2], endPoints[(int) BandType.Band2], speed, useLerp);
		if (!isBoostingUp) {
			StartCoroutine (NoteBoostUp ());
		}
		totalNode ++;
	}

	public void OnBand3Trigger () {
		// Debug.Log ("3");
		GameObject go = Instantiate (notePrefabs[2]);
		go.name = "Band2Note";
		go.transform.SetParent (notesParent);
		go.GetComponent<FlowNote> ().InitNote (BandType.Band3, startPoints[(int) BandType.Band3], endPoints[(int) BandType.Band3], speed, useLerp);
		totalNode ++;
	}

	public void OnBand4Trigger () {
		// Debug.Log ("4");
		GameObject go = Instantiate (notePrefabs[3]);
		go.name = "Band2Note";
		go.transform.SetParent (notesParent);
		go.GetComponent<FlowNote> ().InitNote (BandType.Band4, startPoints[(int) BandType.Band4], endPoints[(int) BandType.Band4], speed, useLerp);
		totalNode ++;
	}

	public void OnBand5Trigger () {
		// Debug.Log ("5");
		GameObject go = Instantiate (notePrefabs[4]);
		go.name = "Band2Note";
		go.transform.SetParent (notesParent);
		go.GetComponent<FlowNote> ().InitNote (BandType.Band5, startPoints[(int) BandType.Band5], endPoints[(int) BandType.Band5], speed, useLerp);
		totalNode ++;
	}

	public void OnBand6Trigger () {
		// Debug.Log ("6");
		GameObject go = Instantiate (notePrefabs[5]);
		go.name = "Band2Note";
		go.transform.SetParent (notesParent);
		go.GetComponent<FlowNote> ().InitNote (BandType.Band6, startPoints[(int) BandType.Band6], endPoints[(int) BandType.Band6], speed, useLerp);
		totalNode ++;
	}
	#endregion

	#region Notes Boost Up
	bool isBoostingUp = false;
	IEnumerator NoteBoostUp () {
		isBoostingUp = true;
		Time.timeScale = boostedTimeScale;
		yield return new WaitForSeconds (boostedDuringTime);
		Time.timeScale = 1f;
		isBoostingUp = false;
	}
	#endregion
}
