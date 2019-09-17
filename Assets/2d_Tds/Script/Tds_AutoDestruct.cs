using UnityEngine;
using System.Collections;

public class Tds_AutoDestruct : MonoBehaviour {

	ParticleSystem vPS;

	// Use this for initialization
	void Start () {
		vPS = GetComponent<ParticleSystem> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (vPS)
		if (!vPS.IsAlive())
			GameObject.Destroy (this.gameObject);
	}
}
