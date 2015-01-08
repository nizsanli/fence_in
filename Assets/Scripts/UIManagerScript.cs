using UnityEngine;
using System.Collections;

public class UIManagerScript : MonoBehaviour {

	// Launch the main game, fool (Called via UI)
	public void LaunchMainGame() {
		Application.LoadLevel("FenceIn");
	}
}
