using UnityEngine;
using System.Collections;

public class WallScript : MonoBehaviour {

	private bool electric;


	public void setElectric(bool status)
	{
		electric = status;
	}

	public bool isElectric()
	{
		return electric;
	}



}
