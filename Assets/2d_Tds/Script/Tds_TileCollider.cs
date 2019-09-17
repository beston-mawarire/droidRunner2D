using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;
using UnityEngine.UI;
using System.Linq;

public class Tds_TileCollider : MonoBehaviour {

	public List<Tds_Tile> vListCollider; 		//hold only the current level
	public Tds_Character vCharacter; 

	// Use this for initialization
	void Start () {
		//get the player once to be able to update just when we have NEW collider
		//we will refresh the ground type, and other conditions on the player ONLY when we make changement
		vCharacter = transform.parent.parent.GetComponent<Tds_Character> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		Tds_Tile vTile = col.GetComponent<Tds_Tile> ();
		Tds_Loot vLoot = col.GetComponent<Tds_Loot> ();

		if (vTile != null) {

			//ONLY refresh variable current level
			vListCollider.Add (vTile);

			//make the player refresh it's pixel tiles variables
			if (vCharacter != null)
				vCharacter.RefreshVariables (vListCollider);
		}

		if (vLoot != null) {

			//make the player refresh it's pixel tiles variables
			if (vCharacter != null)
				vCharacter.RefreshLoot (vLoot);
		}
	}

	void OnTriggerExit2D(Collider2D col)
	{
		Tds_Tile vTile = col.GetComponent<Tds_Tile> ();
		Tds_Loot vLoot = col.GetComponent<Tds_Loot> ();

		//check if we have it on the list so we can remove it
		if (vTile != null) {
			if (vListCollider.Contains (vTile)) {
				vListCollider.Remove (vTile);

				//make the player refresh it's pixel tiles variables
				vCharacter.RefreshVariables (vListCollider);
			}
		} else if (vLoot != null) {
			vLoot = null;			
			vCharacter.RefreshLoot (vLoot);
		}
	}
}
