using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tds_HandAnimation : MonoBehaviour {

	private Tds_Character vCurCharacter;
	private Tds_GameManager vGameManager;
	private AudioSource vAudioSource;

	// Use this for initialization
	void Start () {
		vCurCharacter = transform.parent.GetComponent<Tds_Character> ();
		vGameManager = GameObject.Find ("GameManager").GetComponent<Tds_GameManager> ();
		vAudioSource = GetComponent<AudioSource> ();
	}

	public void FinishRecharging()
	{
		vCurCharacter.RechargeWeapon ();
	}

	public void FinishShooting()
	{
		vCurCharacter.FinishShooting ();
	}

	public void PlaySound(string vSound)
	{
		//get the audio clip from the game manager
		AudioClip vClip = vGameManager.GetAudioClip (vSound);

		if (vAudioSource != null && vClip != null) {
			vAudioSource.clip = vClip;
			vAudioSource.Play();
		}
	}

	public void MeleeAttack()
	{
		//do the attack right there.
		vCurCharacter.MeleeAttack ();
	}
}
