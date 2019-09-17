using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;


#if UNITY_EDITOR
	using UnityEditor;
#endif

public class Tds_Tile : MonoBehaviour {
	
	public enum cTileType{Ground, Wall, Above, Teleport, Destructible, Trap}; 

	//Ground 	= Your walking on it
	//Wall   	= Can't walk on it. Stop movement
	//Above		= You will always walk below this just like a bird. 
	//Teleport  = will teleport the player on the other teleport field
	//Destructible = Will be destroyed when being shooted. May spawn Items on ground for the player
	//trap 		= When it's ON, when the player touch it, receive dmg one time.

	public int HP = 3; 										    //if destructible, can be destroyed on it
	public int Dmg = 1;											//dmg done by trap
	public float ElapseTrapTimer = 0f;							//in how much time the trap will begin
	public float ElapseTrapON = 0f;								//how much time the trap is UP (do some dmg)
	public float ElapseTrapOFF = 0f;							//how much time the traps is DOWN
	public List<ItemName> vItemList = new List<ItemName>();  	//if the list is empty, then it the destructible item doesn't give items
	public List<GameObject> vDebrisList = new List<GameObject>();  	//when destroying a destructible, it will spawn items on ground
	public cTileType vTileType = cTileType.Ground;
	public GameObject vLinkedObject;
	public bool DoorStatus = false;								//handle if the door is closed or not
	public bool ShowParticles = false;							//show the particle effect above the PixelTile
	public int LayerOrder = 0;										//keep a trace of the original layer order
	public List<Texture2D> vAnimationList = new List<Texture2D>();	//keep the texture2d for the animation there
	private List<Sprite> vAnimationSprite;
	public float AnimationSpeed; 									//if the animation list isn't empty, we animate it with this speed
	public Color color = Color.white;
	public GameObject objParticle = null;						//link here the instantied particle system here so we can show/hide it when switching levels.
	private Tds_Grid grid;
	private bool IsDragging = false;
	private bool IsAnimated = false;
	private int vCurIndexTile = 0;
	public bool DoorIsMoving = false; 							//prevent multiple click on the door. Open then we can close
	public SpriteRenderer vRenderer; 
	private Tds_GameManager vGameManager;
	public bool CanBeSeen = false;								//check if we can see it when the camera is close. Save some memory here
	public bool IsReady = false;

	public bool TrapIsActivated = false;
	private bool CanDoDmg = false;								//prevent trap to do multiple dmg each time
	private Tds_Character vMainPlayer;

	//get the right Sorting Order by level + pixeltile type
	public int GetSortingOrderByTile ()
	{
		//initialise variable
		int vNewOrder = 0;

		Tds_Tile vCurTile = this;

		//redraw the tiles correctly when we play
		if (vCurTile.vTileType == Tds_Tile.cTileType.Above) //above is always at top!
			vNewOrder = vCurTile.LayerOrder + 300;
		else
			vNewOrder = vCurTile.LayerOrder + 100;

		//return new order
		return vNewOrder;
	}

	void Start()
	{
		CanBeSeen = true;

		//check if it's animated or not
		IsAnimated = false;

		//get the renderer automatically
		vRenderer = GetComponent<SpriteRenderer> ();

		if (vAnimationList.Count > 0) {
			//initialise the sprite list to be used
			vAnimationSprite = new List<Sprite>();

			//create 1 time the sprite list to be used after
			foreach (Texture2D vCurText in vAnimationList)
				vAnimationSprite.Add(Sprite.Create (vCurText, vRenderer.sprite.rect, new Vector2 (0f, 0f), 128f));

			IsAnimated = true;
		}

		if (vTileType == cTileType.Destructible) {
			vGameManager = GameObject.Find ("GameManager").GetComponent<Tds_GameManager> ();

			//change it transform.Z to be near the camera so the Mouseover, mouseenter and mouseexit are trigged before everthing else
			Vector3 vPosition = transform.position;
			vPosition.z = -2f;
			gameObject.transform.position = vPosition;
		}
			
		IsReady = true;

		if (vTileType == cTileType.Trap) {
			//get the main player
			vMainPlayer = GameObject.Find("Player").GetComponent<Tds_Character>();

			InvokeRepeating ("ActivateTrap", 0f, AnimationSpeed);
		}
		else if (IsAnimated)
			InvokeRepeating ("AnimateTile", 0f, AnimationSpeed);
	}


	void ShowDamage(int vDmg)
	{
		//create dmg text on target
		GameObject vDmgText = Instantiate (vGameManager.vDmgText);
		vDmgText.transform.position = transform.position;
		vDmgText.transform.parent = vGameManager.vWorldObj.transform;
		vDmgText.GetComponent<Tds_DmgText> ().StartMoving (vDmg.ToString (), vGameManager.vDamageTextColor);	//make the dmg text starting moving
	}

	public Tds_Items GetItems(ItemName vCurName)
	{
		Tds_Items vItemFound = null;

		foreach (Tds_Items vCurItem in vGameManager.vItemsList)
			if (vCurItem.vItemName == vCurName)
				vItemFound = CopyItem(vCurItem);
		
		return vItemFound;
	}

	//make a TRUE copy of this weapon to be used
	public Tds_Items CopyItem(Tds_Items vOld)
	{
		//copy everything for the weapon
		Tds_Items vNew = new Tds_Items ();
		vNew.vItemName = vOld.vItemName;
		vNew.vName = vOld.vName;
		vNew.GiveWeapon = vOld.GiveWeapon;
		vNew.ItemIcon = vOld.ItemIcon;
		vNew.vWeaponName = vOld.vWeaponName;
		vNew.Usable = vOld.Usable;
		vNew.vAmmoType = vOld.vAmmoType;
		vNew.vDmgType = vOld.vDmgType;

		//return new tds_weapons
		return vNew;
	}

	public void ApplyDamage (int vDmg)
	{
		//show current Dmg or not
		if (vGameManager.ShowDamageNumber)
			ShowDamage (vDmg);

		//reduce dmg
		HP -= vDmg;

		//check if the character died
		if (HP <= 0) {
			TileDie ();
		} else
			StartCoroutine (vGameManager.BlinkEffect (transform));
	}

	public void TileDie()
	{
		foreach (ItemName vCurItem in vItemList) {
			Tds_Items vNewItem = GetItems (vCurItem);

			//make sure the fabrik item obj exist
			if (vGameManager.vItemObj != null) {

				//create the empty item obj
				GameObject vNewItemObj = Instantiate (vGameManager.vItemObj);

				vNewItemObj.transform.position = vRenderer.bounds.center;
				vNewItemObj.transform.parent = vGameManager.vWorldObj.transform;

				//then modify every component to match the new items
				vNewItemObj.GetComponent<SpriteRenderer>().sprite = vNewItem.ItemIcon;

				if (vNewItem.Usable)
					//send the items for the instantied obj
					vNewItemObj.GetComponent<Tds_Loot>().InitialiseLoot(vNewItem, vGameManager);
				else
					vNewItemObj.GetComponent<Tds_Loot>().enabled = false; //trash item on ground doesn't need tds_loot
			}
		}

		//spawn the debris on the target location
		foreach (GameObject vDebris in vDebrisList) {
			//create the empty item obj
			GameObject vNewDebrisobj = Instantiate (vDebris);
			vNewDebrisobj.transform.position = transform.position;
		}

		GameObject.Destroy (this.gameObject);
	}
		
	public void AnimateTile()
	{
		//increase counter
		vCurIndexTile++;

		//check if we go back to the first one
		if (vCurIndexTile >= vAnimationList.Count)
			vCurIndexTile = 0;

		//get the right sprite to show
		vRenderer.sprite = vAnimationSprite [vCurIndexTile];
	}

	public void ActivateTrap()
	{
		if (ElapseTrapTimer >= 0f) {

			if (TrapIsActivated && CanDoDmg) {

				//calculate the dist between
				float vDist = Vector3.Distance (vMainPlayer.transform.position, transform.position);

				//look around if the player is nearby. Do DMG!
				if (vMainPlayer != null)
				if (vDist <= 1.5f) {
					vMainPlayer.ApplyDamage (Dmg);
					CanDoDmg = false;
				}
			}

			ElapseTrapTimer -= Time.deltaTime;
		} else {
			if (!TrapIsActivated) {
				vCurIndexTile++; //increase animaiton

				//check if we are at the last one
				if (vCurIndexTile >= vAnimationList.Count-1) {
					TrapIsActivated = true;

					//get the timer for how log the trap must be kept ON
					ElapseTrapTimer = ElapseTrapON;
				}
			} else {
				vCurIndexTile--; //decrease animation

				if (vCurIndexTile <= 0) {
					TrapIsActivated = false;

					//get the timer for how log the trap must be kept ON
					ElapseTrapTimer = ElapseTrapOFF;

					//re-enable the trap itself to do dmg next time
					CanDoDmg = true;
				}
			}

			//get the right sprite to show
			vRenderer.sprite = vAnimationSprite [vCurIndexTile];
		}
	}
		

	public IEnumerator OpenCloseDoor()
	{
		//toggle the door status
		DoorStatus = !DoorStatus;

		if (IsAnimated)
		{
			//choose if we open or close the door
			if (DoorStatus == true) {
				int vcpt = 0; 
				while (vcpt < vAnimationList.Count) {
					yield return new WaitForSeconds (AnimationSpeed);
					//vRenderer.sprite = Sprite.Create (vAnimationList [vcpt], vRenderer.sprite.rect, new Vector2 (0f, 0f), 128f);
					vRenderer.sprite = vAnimationSprite[vcpt];
					vcpt++;
				}
			} 
			else {
				int vcpt = vAnimationList.Count-1; 
				while (vcpt >= 0) {
					yield return new WaitForSeconds (AnimationSpeed);
					//vRenderer.sprite = Sprite.Create (vAnimationList [vcpt], vRenderer.sprite.rect, new Vector2 (0f, 0f), 128f);
					vRenderer.sprite = vAnimationSprite[vcpt];
					vcpt--;
				}
			} 
		}
		else {
			//choose if we open or close the door
			if (DoorStatus == true) {
				float vAlpha = 1;
				while (vAlpha > 0f) {
					vAlpha -= 0.05f; 
					yield return new WaitForSeconds (0.01f);
					ChangeAlpha (vAlpha);
				}
			} else {
				float vAlpha = 0;
				while (vAlpha < 1f) {
					vAlpha += 0.05f; 
					yield return new WaitForSeconds (0.01f);
					ChangeAlpha (vAlpha);
				}
			}
		}

		//finished
		DoorIsMoving = false;
	}

	//change the alpha of this object
	void ChangeAlpha (float vNewAlpha)
	{
		//get all the spriterenderer
		SpriteRenderer[] vRenderer = GetComponentsInChildren<SpriteRenderer> ();

		//change the color for the whole character alpha
		foreach (SpriteRenderer vCurRenderer in vRenderer)
			vCurRenderer.color = new Color(1f,1f,1f,vNewAlpha);
	}


	#if UNITY_EDITOR
	void OnDrawGizmos()
	{
		//teleport line are always red
		Gizmos.color = Color.red;

		if (grid == null)
			grid = GameObject.Find ("AutoTile").GetComponent<Tds_Grid>();

		if (Selection.activeGameObject == this.gameObject) 
		{
			Event e = Event.current;
			Ray ray = Camera.current.ScreenPointToRay(new Vector3(e.mousePosition.x, -e.mousePosition.y + Camera.current.pixelHeight));
			Vector3 mousePos = ray.origin;

			//remove active selection
			if (e.type == EventType.mouseUp && !IsDragging) //enable drag
			{
				IsDragging = true;
			}
			else if (e.type == EventType.mouseUp && IsDragging)
			{
				IsDragging = false;
			}
			else if (IsDragging)
			{				
				//get the new position
				Vector3 aligned = new Vector3(Mathf.Floor(mousePos.x/grid.width)*grid.width, Mathf.Floor(mousePos.y/grid.height)*grid.height, 0.0f);
				
				//modify current position of this square on the mouse
				if (this.transform.position != aligned)
					this.transform.position = aligned;
			}

			//if teleport, we draw the line bewteen the 2 objects
			if (vTileType == cTileType.Teleport)
			{
				if (vLinkedObject == null)
					Debug.Log("Find another linked object because it's missing!");
				else
				{
					//draw a line between both
					Gizmos.DrawLine(this.transform.position, vLinkedObject.transform.position);
				}
			}
		}
	}

	#endif
}
