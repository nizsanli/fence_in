﻿using UnityEngine;
using System.Collections;

public class WallScript : MonoBehaviour {

	public int numBurstParticles = 2;
	private bool electric;
	
	public void Start()
	{
		particleSystem.enableEmission = false;
		particleSystem.renderer.sortingLayerName = "Foreground";
	}

	public void emitParticles()
	{
		particleSystem.enableEmission = true;
		particleSystem.Emit(numBurstParticles);
		particleSystem.enableEmission = false;
	}

	public void setElectric(bool status)
	{
		electric = status;
	}

	public bool isElectric()
	{
		return electric;
	}
}
