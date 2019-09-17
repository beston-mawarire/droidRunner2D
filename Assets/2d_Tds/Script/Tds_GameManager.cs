using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AssemblyCSharp;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Tds_GameManager : MonoBehaviour {

	public List<Tds_Weapons> vWeaponList;
	public List<Tds_Items> vItemsList;
	public List<Tds_SoundList> vSoundList;
	public List<Tds_WeaponMenu> vWeaponMenuList;
	public List<Tds_Projectile> vProjectileList;

	//define here the color for every kind of items
	public Color vFriendlyColor = Color.green;
	public Color vHostileColor = Color.red;
	public Color vNeutralColor = Color.white;
	public Color vItemColor = Color.yellow;

	public bool ShowDamageNumber = true;				//if True, we show the damage number when doing dmg above enemy or your own player
	public float vMeleeRange = 2f;						//how close the NPC will come and do a melee attack? (knife, hand..)
	public float vRangedRange = 40f;					//how close the NPC will come dan do a ranged attack? (rifle, bazooka...)
	public float vCursorRange = 12f; 					//how far until the rotation on the player is disable. It prevent from rotating infinitly

	public Color vBlinkDamageColor = Color.red;			//blink the enemy with this color
	public Color vDamageTextColor = Color.white;		//when showing the damage number, we use this color	

	//icons for the loot panel
	public Sprite vLowIcon = null;
	public Sprite vMediumIcon = null;
	public Sprite vHighIcon = null;
	public Sprite vGodlyIcon = null;

	public GameObject AimObj = null;					//you can put your aim obj here. it will replace the mouse cursor
	public GameObject ReloadObj = null;					//when reloading, we see the reloading animation as the mouse icon
	public GameObject vDmgText = null;					//we use this fabrik to show dmg text above characters who are damaged
	public GameObject vDyingAnim = null;				//if exist, instantiate this new object will make the character dying

	public Tds_Character vMainPlayer = null;
	public GameObject vWorldObj = null;					//put everything in this gameobject 
	public GameObject vCanvasUI = null;
	public bool IsReady = false;						//start every player/enemy at the same time
	public GameObject vGameOverObj = null;				//when main player die, just stop the game and show the UI
	public GameObject vPressSpaceObj = null;
	public GameObject vItemObj = null;					//has the item obj fabrik
	public GameObject vItemLootedAnim = null;			//when looting, if exist will show a little text line at the bottom showing the item name and it's color

	private bool PlayerIsDead = false;
	private AudioSource vAudioSource = null;

	//blink effect mats
	public Material vSpriteMat;
	public Material vBlinkMat;

	public Image vHpImage;
	public Text vPlayerText;

	// Use this for initialization
	void Start () {
		InitialiseGame ();
	}
	
	// Update is called once per frame
	void Update () {
		//reload this scene when the player is dead
		if (PlayerIsDead && Input.GetMouseButton (0))
			SceneManager.LoadScene (0);
	}

	void InitialiseGame()
	{
		//every character need a spriterenderer by default
		vSpriteMat = GetComponent<SpriteRenderer>().material;	
		//get back the default material
		vBlinkMat = (Material)Resources.Load("Components/DroidSansMono", typeof(Material));		//get this material which make the sprite white

		//disable the gameover obj when the game start.
		if (vGameOverObj != null)
			vGameOverObj.SetActive (false);

		//disable the vPressSpaceObj obj when the game start.
		if (vPressSpaceObj != null)
			vPressSpaceObj.SetActive (false);

		vAudioSource = GetComponent<AudioSource> ();

		//we hide the Mouse Icon completely
		if (AimObj != null)
			Cursor.visible = false;

		//initialise every character
		foreach (Tds_Character vChar in Resources.FindObjectsOfTypeAll<Tds_Character>())
			if (vChar.IsCharacter) {

				//get the main player
				if (vChar.IsPlayer)
					vMainPlayer = vChar;

				//initialise the character
				vChar.InitialiseChar (this);
			}
		
		//game is ready
		IsReady = true;
	}

	public void SwitchScene(string vNewScene)
	{
		if (vNewScene != null)
			SceneManager.LoadScene (vNewScene);
	}

	public void RefreshPlayerHP(float vPerc)
	{
		//show different color for HP value for player
		if (vPerc >= 0.7f)
			vHpImage.color = Color.green;
		else if (vPerc >= 0.3f)
			vHpImage.color = Color.yellow;
		else
			vHpImage.color = Color.red;

		//update HP
		vHpImage.fillAmount = vPerc;
	}

	public void SetPlayerDead()
	{
		IsReady = false;
		//remove main camera from player to prevent it from being destroyed when dying.
		Camera.main.transform.parent = null;
		vGameOverObj.SetActive (true);

		//player losing music
		PlaySound("Losing");
		PlayerIsDead = true;
	}

	//search for the selected clipname in the list and play it
	public AudioClip GetAudioClip(string vClipName)
	{
		AudioClip vclip = null;

		foreach (Tds_SoundList vSound in vSoundList)
			if (vSound.vName == vClipName)
				vclip = vSound.vAudioClip;

		return vclip;
	}
		
	public void PlaySound(string vSound)
	{
		//get the audio clip from the game manager
		AudioClip vClip = GetAudioClip (vSound);

		if (vAudioSource != null && vClip != null) {
			vAudioSource.clip = vClip;
			vAudioSource.Play();
		}
	}

	//blink effect when receiving dmg!
	public IEnumerator BlinkEffect(Transform vTrans)
	{
		//get all the sprite renderer child for this character and make them blink!
		SpriteRenderer[] vRendererList = vTrans.GetComponentsInChildren<SpriteRenderer> ();

		//white color
		foreach (SpriteRenderer vRenderer in vRendererList) {
			vRenderer.material = vBlinkMat;
			vRenderer.color = vBlinkDamageColor;
		}

		//wait
		yield return new WaitForSeconds (0.02f);

		//normal color
		foreach (SpriteRenderer vRenderer in vRendererList) {
			vRenderer.material = vSpriteMat;
			vRenderer.color = Color.white;
		}

		//get back to original color
		yield return new WaitForSeconds (0.2f);
	}


	//blink effect when receiving dmg!
	public IEnumerator AlphaEffect(Transform vTrans)
	{
		//get all the sprite renderer child for this character and make them blink!
		SpriteRenderer[] vRendererList = vTrans.GetComponentsInChildren<SpriteRenderer> ();

		//go back to original mats to make it dissapear correctly
		foreach (SpriteRenderer vRenderer in vRendererList) {
			vRenderer.material = vSpriteMat;
			vRenderer.color = Color.white;
		}

		//go from 1f to 0f
		float vcpt = 1f;

		while (vcpt > 0f) {
			Color vNewColor = new Color (1f, 1f, 1f, (float)vcpt);
			//white color
			foreach (SpriteRenderer vRenderer in vRendererList)
				vRenderer.color = vNewColor;

			//wait
			yield return new WaitForSeconds (0.005f);

			//decrease counter
			vcpt -= 0.01f;
		}


		//destroy itself
		GameObject.Destroy (vTrans.gameObject);
	}
}
