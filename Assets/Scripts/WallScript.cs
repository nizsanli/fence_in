using UnityEngine;
using System.Collections;

public class WallScript : MonoBehaviour {

	public bool particlesEnabled;
	private bool electric;

	public void setElectric(bool status)
	{
		electric = status;
	}

	public bool isElectric()
	{
		return electric;
	}

	public void Start()
	{
		particlesEnabled = false;
		particleSystem.enableEmission = false;
	}

	public void Update()
	{

	}
}
