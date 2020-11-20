using UnityEngine;
using System.Collections;

public class WallScript : MonoBehaviour {

	public int numBurstParticles = 2;
	private bool electric;
	
	public void Start()
	{
		GetComponent<ParticleSystem>().enableEmission = false;
		GetComponent<ParticleSystem>().GetComponent<Renderer>().sortingLayerName = "Foreground";
	}

	public void emitParticles()
	{
		GetComponent<ParticleSystem>().enableEmission = true;
		GetComponent<ParticleSystem>().Emit(numBurstParticles);
		GetComponent<ParticleSystem>().enableEmission = false;
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
