using UnityEngine;
using System.Collections;

public class UIManagerScript : MonoBehaviour {

	public FenceListener main;

	// Launch the main game, fool (Called via UI)
	public void LaunchMainGame() {
		Application.LoadLevel("FenceIn");
	}

	public void reset()
	{
		main.reset();
	}
}
