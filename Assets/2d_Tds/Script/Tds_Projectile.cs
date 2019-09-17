using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tds_Projectile : MonoBehaviour {

	public bool IsReady = false;
	public int vDmg = 1;
	public int vRebounce = 0; 				//how many time the bullet will rebounce? if 3, it will hit 3x wall and destroy on the 4th wall collision. 
	public float Speed = 1f;
	public GameObject vImpactFX = null;
	public Tds_Character.FactionType vProjFactionType = Tds_Character.FactionType.Hostile;
	public Tds_GameManager vGameManager;

	private Rigidbody2D vRB;
	private CircleCollider2D vCollider;

	// Use this for initialization
	void Start () {
		vRB = GetComponent<Rigidbody2D> ();
		vCollider = GetComponent<CircleCollider2D> ();
	}

	void FixedUpdate ()
	{
		//goes in the space when it's ready
		if (IsReady) {
			IsReady = false;

			//cannot collider with other projectile
			foreach (Tds_Projectile CurProj in vGameManager.vProjectileList)
				Physics2D.IgnoreCollision (CurProj.GetComponent<Collider2D> (), GetComponent<Collider2D> (), true);
				
			//calcualte how fast the bullet is in the air
			vRB.velocity = transform.up * Speed ;
		}
	}

	//when dying, create a impact where the bullet has been destroyed
	void ProjDie()
	{
		if (vImpactFX != null)
		{
			GameObject vNewImpact = Instantiate (vImpactFX);
			vNewImpact.transform.position = transform.position;
		}

		//remove itself from the main list
		if (vGameManager != null)
			vGameManager.vProjectileList.Remove (this);

		//destroy itself
		GameObject.Destroy (this.gameObject);
	}

	void CalculateRebounce(Collider2D other)
	{
		//disabling istrigger make the collider work and reboucne the bullet properly ONLY when there is a rebounce!
		vCollider.isTrigger = false;
		vRebounce--;

		//wait a little bit before re-enabling the collider
		StartCoroutine (WaitForTime (0.1f));
	}

	IEnumerator WaitForTime(float Sec)
	{
		yield return new WaitForSeconds(Sec);

		//reactivate the bullet
		vCollider.isTrigger = true;

		Vector2 v= vRB.velocity; 
		float angle = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg; 
		transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle-90f));
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		Tds_Tile vTile = other.GetComponent<Tds_Tile> ();
		if (vTile != null) {
			if (vTile.vTileType == Tds_Tile.cTileType.Destructible)
				vTile.TileDie ();

			if (vTile.vTileType == Tds_Tile.cTileType.Wall && vRebounce <= 0)
				ProjDie ();
			else if(vTile.vTileType == Tds_Tile.cTileType.Wall)
				CalculateRebounce(other);
		}
		else if (other.tag == "Character") {
			Tds_Character vChar = other.GetComponent<Tds_Character> ();

			//make sure the projectile is for the other faction and the target is alive.  
			if (vChar.vFactionType != vProjFactionType && vChar.IsAlive) {
				vChar.ApplyDamage (vDmg);
				ProjDie ();
			}
		}
	}
}
