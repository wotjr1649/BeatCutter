using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRootController : MonoBehaviour {
	
	[Header ("UI References")]
	public GameObject mainPanel;
	public GameObject ingamePanel;
	public UIAutoCounter autoCounter;
	public GameObject NoteKochGenerator, Destroyer;

	public static UIRootController instance;

	void Awake() {
		instance = this;
	}

	#region uGUI Callbacks
	public void OnStartClicked () {
		mainPanel.SetActive (false);
		ingamePanel.SetActive (true);
		if (AudioPeer._audioSource != null) {
			AudioPeer._audioSource.Play ();
			NoteKochGenerator.SetActive(true);
			Destroyer.SetActive(true);
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
	}
	#endregion
	
	#region Incode UI Update Methods
	public void UpdateScoreText () {
		autoCounter.UpdateText ();
	}
	#endregion
}
