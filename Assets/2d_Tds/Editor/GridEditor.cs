using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

[CustomEditor(typeof(Tds_Grid))]
public class Tds_GridEditor : Editor {

	Tds_Grid grid;

	public GUISkin vCustSkin;
	public GUISkin vCustSkinSelected;
	public GUISkin vNONESkin;
	public GUISkin vMasterSkin;
	public GUISkin vParentSkin;

	//modes GUI Icons
	public Texture2D vCreateTileText;
	public Texture2D vCreateTileTextSel;
	public Texture2D vSelectTileText;
	public Texture2D vSelectTileTextSel;
	public Texture2D vDeleteTileText;
	public Texture2D vDeleteTileTextSel;
	public Texture2D vColorPickerText;
	public Texture2D vColorPickerTextSel;
	public Texture2D v1x1;
	public Texture2D v2x2;
	public Texture2D v3x3;
	public Texture2D vCanSeeWall;
	public Texture2D vCannotSeeWall;

	public GameObject vTileObject;
	public GameObject vWallObject;
	public GameObject vTilesObject;

	//by default it goes to the CreateTile
	private string vAction = "CreateTile";
	private int levelfactor = 300;					//configure every steps needed
	private float vFactorScale = 16f;
	public bool vShowFonctionUsed = false;
	private int vNbrButton;
	private int vBoxSize; 
	private int oldIndex;
	private int TabIndex = 0;
	private int TileIndex = 0;
	private int MasterFolderIndex = 0;
	private int ParentFolderIndex = 0;
	private int ColorPickerIndex = 0;
	private int TopMenuPickerIndex = 0;
	private int TileSize = 1; 
	private bool IsRefreshing = false;
	private bool IsSizingTile = false;
	private bool IsSearching = false;
	private bool IsLookingWall = false;
	private bool IsLookingLight = false;
	private bool IsLookingTeleportTo = false;
	//private bool IsDoor = false;
	private bool ShowParticle = false;
	private string IsRedoUndo = "";
	private List<GameObject> TilePreview = null;
	private List<Transform> vTeleportTile;
	private List<Texture2D> vTilesFound; 
	private List<GameObject> vGameobjectsFound; 
	private Tds_Tile vSelectedTds_Tile = null;
	private EventType vLastEventType = EventType.Ignore;
	private Vector3 lastaligned = Vector3.zero;
	private Tds_GameManager vGameManager;
	private GameObject vAutoTilesObjects; 
	public GameObject vTilesObjects; 

	void OnEnable(){

		//associate all the initial variable
		InitialiseVariables ();

		//try to get the vtilesObjects
		vTilesObjects = GameObject.Find("TilesGroup");
		if (vTilesObjects == null) {
			vTilesObjects = (GameObject)PrefabUtility.InstantiatePrefab (vTilesObject);
		}

		//clear old tiles
		ClearTilesPreview();

		oldIndex = 0;
		grid = (Tds_Grid)target;

		//increase localscale with a formula 
		if (grid != null) {
			vFactorScale = (1024 / grid.dimension);	
			grid.width = 4f; 
			grid.height = 4f; 
		}

		//try to get the last Action or last levels
		GameObject vGameManagerObject = GameObject.Find("GameManager");
		if (vGameManagerObject != null) {
			//make sure we go the gamemanager component
			if (vGameManagerObject.GetComponent<Tds_GameManager> () != null) {
				vGameManager = vGameManagerObject.GetComponent<Tds_GameManager> ();
			}
		}

		//get the teleport tile by default (hardcoded!)
		Texture2D vTexture2D = Resources.Load<Texture2D> ("Customs/TeleportIcon");
		if (vTexture2D != null) 
		{
			Tds_Texture vTds_Texture = new Tds_Texture();
			vTds_Texture.vFilename = "Customs/TeleportIcon";
			vTds_Texture.vTexture = vTexture2D;

			Texture2D vTextSelection = Resources.Load<Texture2D> ("Customs/teleportTile");
			Tds_Texture vSelectionTexture = new Tds_Texture();
			vSelectionTexture.vFilename = "Customs/teleportTile";
			vSelectionTexture.vTexture = vTextSelection;

			//attach the pixeltexture to the grid
			if (grid != null) {
				grid.teleportPrefab = vTds_Texture;
				grid.selectionPrefab = vSelectionTexture;
			}
		}
		else
			Debug.Log ("default teleportTile is missing! Default Path = Resources/Custom/TeleportIcon.png");

		//showse
		vAction = "";
		TopMenuPickerIndex = -1;
	}

	void OnDisable()
	{
		//clear old tiles
		ClearTilesPreview();
	}

	void InitialiseVariables()
	{
		//get default skin
		vCustSkin = (GUISkin)(AssetDatabase.LoadAssetAtPath("Assets/2d_Tds/2d_Tds_GUISKIN.guiskin", typeof(GUISkin)));
		vCustSkinSelected = (GUISkin)(AssetDatabase.LoadAssetAtPath("Assets/2d_Tds/2d_Tds_GUISKINSEL.guiskin", typeof(GUISkin)));
		vNONESkin = (GUISkin)(AssetDatabase.LoadAssetAtPath("Assets/2d_Tds/2d_Tds_NONE.guiskin", typeof(GUISkin)));
		vMasterSkin = (GUISkin)(AssetDatabase.LoadAssetAtPath("Assets/2d_Tds/2d_Tds_MASTER.guiskin", typeof(GUISkin)));
		vParentSkin = (GUISkin)(AssetDatabase.LoadAssetAtPath("Assets/2d_Tds/2d_Tds_PARENT.guiskin", typeof(GUISkin)));

		//modes GUI Icons
		vCreateTileText = Resources.Load<Texture2D> ("Customs/CreateTile");
		vCreateTileTextSel = Resources.Load<Texture2D> ("Customs/CreateTileSel");
		vSelectTileText = Resources.Load<Texture2D> ("Customs/SelectTile");
		vSelectTileTextSel = Resources.Load<Texture2D> ("Customs/SelectTileSel");
		vDeleteTileText = Resources.Load<Texture2D> ("Customs/DeleteTile");
		vDeleteTileTextSel = Resources.Load<Texture2D> ("Customs/DeleteTileSel");
		vColorPickerText = Resources.Load<Texture2D> ("Customs/ColorPicker");
		vColorPickerTextSel = Resources.Load<Texture2D> ("Customs/ColorPickerSel");

		//modes GUI Icons
		vCreateTileText = Resources.Load<Texture2D> ("Customs/CreateTile");
		vCreateTileTextSel = Resources.Load<Texture2D> ("Customs/CreateTileSel");
		vSelectTileText = Resources.Load<Texture2D> ("Customs/SelectTile");
		vSelectTileTextSel = Resources.Load<Texture2D> ("Customs/SelectTileSel");
		vDeleteTileText = Resources.Load<Texture2D> ("Customs/DeleteTile");
		vDeleteTileTextSel = Resources.Load<Texture2D> ("Customs/DeleteTileSel");
		vColorPickerText = Resources.Load<Texture2D> ("Customs/ColorPicker");
		vColorPickerTextSel = Resources.Load<Texture2D> ("Customs/ColorPickerSel");
		v1x1 = Resources.Load<Texture2D> ("Customs/1x1");
		v2x2 = Resources.Load<Texture2D> ("Customs/2x2");
		v3x3 = Resources.Load<Texture2D> ("Customs/3x3");
		vCanSeeWall = Resources.Load<Texture2D> ("Customs/CanSeeWall");
		vCannotSeeWall = Resources.Load<Texture2D> ("Customs/CannotSeeWall");

		vTileObject = Resources.Load<GameObject> ("Fabric/DontDeleteObject");
		vWallObject = Resources.Load<GameObject> ("Fabric/WallObj");
		vTilesObject = Resources.Load<GameObject> ("Fabric/TilesGroup");
	}

	[MenuItem("Assets/Create/TileSet")]
	static void CreateTileSet(){
		var asset = ScriptableObject.CreateInstance<Tds_TileSet> ();
		var path = AssetDatabase.GetAssetPath (Selection.activeObject);

		if (string.IsNullOrEmpty (path)) {
			path = "Assets";
		} else if (Path.GetExtension (path) != "") {
			path = path.Replace (Path.GetFileName (path), "");
		} else {
			path += "/";
		}

		var assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/TileSet.asset");
		AssetDatabase.CreateAsset(asset, assetPathAndName);
		AssetDatabase.SaveAssets();
		EditorUtility.FocusProjectWindow();
		Selection.activeObject = asset;
		asset.hideFlags = HideFlags.DontSave;
	}

	void ShowFunctionUsed (string vFunctionName)
	{
		if (vShowFonctionUsed)
			Debug.Log (vFunctionName);
	}

	void ClearTilesPreview()
	{
		//get autotiles objects
		vAutoTilesObjects = GameObject.Find ("AutoTile");

		//get all the tiles childs of the autotiles so we can destroy them. They are tilepreview left behind!
		Tds_Tile[] vTilesList = vAutoTilesObjects.GetComponentsInChildren<Tds_Tile> (); 
		foreach (Tds_Tile vCurTds_Tile in vTilesList)
			GameObject.DestroyImmediate (vCurTds_Tile.gameObject);
	}

	public void ChangeWallColor()
	{
		ShowFunctionUsed ("ChangeWallColor");
		int i = 0;
		while (i < vTilesObjects.transform.childCount) 
		{
			Transform vTile = vTilesObjects.transform.GetChild(i);
			Tds_Tile vTds_Tile = vTile.GetComponent<Tds_Tile> ();
			SpriteRenderer vRenderer = vTile.GetComponent<SpriteRenderer> ();

			//check if the Tds_Tile is a wall
			if (vTds_Tile.vTileType == Tds_Tile.cTileType.Wall && IsLookingWall)
				vRenderer.color = grid.WallColor;	//if wall, put it in green
			else {
				Color vNewColor = vRenderer.color;
				//change the color to white but do not touch the alpha
				vNewColor.b = 255;
				vNewColor.g = 255; 
				vNewColor.r = 255;
				vRenderer.color = vNewColor; //show them in white!
			}
			i++;
		}
	}

	public void ShowTileSet()
	{			
		//Tile Prefab
		EditorGUI.BeginChangeCheck ();
		
		Texture2D vTexturePixel = null;
		if (grid.tilePrefab != null)
			vTexturePixel = grid.tilePrefab.vTexture;

		//Tile Map
		EditorGUI.BeginChangeCheck();
		var newTileSet = (Tds_TileSet)EditorGUILayout.ObjectField ("Tileset", grid.tileSet, typeof(Tds_TileSet), false);
		if (EditorGUI.EndChangeCheck ()) {
			grid.tileSet = newTileSet;
			Undo.RecordObject(target, "Grid Changed");
		}
			
		if (grid.tileSet != null) {
			EditorGUI.BeginChangeCheck();
			var names = new string[grid.tileSet.prefabs.Count];
			var values = new int[names.Length];
		}

		//animation speed
		if (grid.tilePrefab != null)
		if (grid.tilePrefab.vAnimationList.Count > 0)
			grid.AnimationSpeed = EditorGUILayout.FloatField("Animation Speed", grid.AnimationSpeed); 

		//cannot be less than 0.2f for animation
		if (grid.AnimationSpeed == 0)
			grid.AnimationSpeed = 0.05f; 

		//layer order
		var newLayerOrder = EditorGUILayout.IntField("Layer Order", grid.OrderLayer); 
		if (newLayerOrder < 0)
			newLayerOrder = 0;
		grid.OrderLayer =  newLayerOrder;
		
		//Tds_Tiles
		var newTds_Tile = EditorGUILayout.EnumPopup("Tile Spec", grid.vTileType); 

		//show particle
		grid.ShowParticle = EditorGUILayout.Toggle("Show Particle (Teleport)", grid.ShowParticle); 

		//if Teleport change the default Prefab to apply. Can only apply a teleport field with the teleport prefab
		if ((Tds_Tile.cTileType)newTds_Tile == Tds_Tile.cTileType.Teleport) {
			grid.tilePrefab = grid.teleportPrefab;
		}
		grid.vTileType =  (Tds_Tile.cTileType)newTds_Tile;
		
		//LINE
		GUILayout.BeginHorizontal();
		if (GUILayout.Button ("Get TileSet folder")) {
			var vpath = EditorUtility.OpenFolderPanel ("Get your TileSet folder", "", "");
			if (vpath.Length != 0) {
				RefreshTileSet(vpath);
				
				//save object
				EditorUtility.SetDirty(grid.tileSet);
			}
		}            
		
		//End LINE
		GUILayout.EndHorizontal();

		//make sure we have a tileset
		if (grid.tileSet != null) 
		{
			//show the 3x options
			//TileIndex = GUILayout.Toolbar (TileIndex, new string[] {"All"});
			/*switch (TileIndex) {
			case 1: */
					///////////////////////////SHOW ALL///////////////////////////////////
				if (grid.tileSet.prefabs.Count <= 0 && grid.tileSet.TileSetPath != "")
					RefreshTileSet (grid.tileSet.TileSetPath);

				if (!IsRefreshing) {
					//change the button
					GUI.skin = vCustSkin;
					int vCpt = 0;

					//Try to get all the folder wich has texture on it
					List<Tds_Texture> vTexture2D = new List<Tds_Texture> ();

					foreach (Tds_Folder vPFolder in grid.tileSet.prefabs)
						foreach (Tds_Texture vTexture in vPFolder.vPixelTextureList)
							vTexture2D.Add (vTexture);	

					foreach (Tds_Texture vTexture in vTexture2D) {
						//make sure each texture has it's own button and same dimension
						if (vCpt <= 0)
							GUILayout.BeginHorizontal ();

						//show the texture
						HandleTexture (vTexture);

						vCpt++;
						if (vCpt >= 8) {
							vCpt = 0;
							GUILayout.EndHorizontal ();
						}
					}
					GUI.skin = null;
				}
				/*break;
				case 0 : 
					///////////////////////////SHOW BY FOLDERS NAME///////////////////////////////////
					if (grid.tileSet.prefabs.Count <= 0 && grid.tileSet.TileSetPath != "")
					RefreshTileSet (grid.tileSet.TileSetPath);

					if (!IsRefreshing) {
						
						//create the foldername list to be choosen
						List<string> vMasterFolderName = new List<string>();
						foreach (Tds_Folder vPFolder in grid.tileSet.prefabs)
						if (!vMasterFolderName.Contains(vPFolder.vMasterFolderName))
							vMasterFolderName.Add (GetNiceName(vPFolder.vMasterFolderName));

						//change the skin for the master one
						GUI.skin = vMasterSkin;
						//add little line
						EditorGUILayout.Space ();
						GUILayout.Label("Main Category");

						//show the master category
						MasterFolderIndex = GUILayout.SelectionGrid (MasterFolderIndex, vMasterFolderName.ToArray(),4);

						//check out which masterfolder is selected
						int cpt = 0; 

						//create the foldername list to be choosen from the mater folder!
						List<Tds_Folder> vFolderName = new List<Tds_Folder>();
						foreach (Tds_Folder vPFolder in grid.tileSet.prefabs) {
							//reset it value
							cpt = 0;

							foreach (string vMasterFolderN in vMasterFolderName) {
								if (GetNiceName(vPFolder.vMasterFolderName) == GetNiceName(vMasterFolderN) && cpt == MasterFolderIndex)
									vFolderName.Add (vPFolder);	
								
								//increase counter
								cpt++;
							}
						}

						//change the skin for the parent one
						GUI.skin = vParentSkin;

						//add little line
						EditorGUILayout.Space ();
						GUILayout.Label("Sub Category");

						//validate if we have a parent folder selected. if not, we get the first one to prevent a crash
						if (vFolderName.Count <= ParentFolderIndex)
							ParentFolderIndex = 0;
					
						List<string> vFolderN = new List<string> ();
							foreach (Tds_Folder vFolderr in vFolderName)
							vFolderN.Add (GetNiceName(vFolderr.vFolderName));

						//show the category
						ParentFolderIndex = GUILayout.SelectionGrid (ParentFolderIndex, vFolderN.ToArray(),4);

						int vCpt = 0;

						//Try to get all the folder wich has texture on it
						List<Tds_Texture> vTexture2D = new List<Tds_Texture> ();

						//get all the texture needed ONLY for the one selected!
						foreach (Tds_Folder vPFolder in grid.tileSet.prefabs)
						if (vPFolder == vFolderName[ParentFolderIndex])						//ONLY show the currently selected
							foreach (Tds_Texture vTexture in vPFolder.vPixelTextureList)
								vTexture2D.Add (vTexture);

						//show them all
						foreach (Tds_Texture vTexture in vTexture2D) {
							//make sure each texture has it's own button and same dimension
							if (vCpt <= 0)
								GUILayout.BeginHorizontal ();

							//show the texture
							HandleTexture (vTexture);

							vCpt++;
							if (vCpt >= 8) {
								vCpt = 0;
								GUILayout.EndHorizontal ();
							}
						}

						GUI.skin = null;
					}
				break;*/
			//}
		}
	}

	//make a string like this Hello, Folder...
	string GetNiceName(string vFolderName)
	{
		string vNewFolderName = "";
		if (vFolderName != "")
			vNewFolderName = vFolderName.Substring (0, 1).ToUpper () + vFolderName.Substring (1, vFolderName.Length - 1).ToLower ();
		return vNewFolderName;
	}

	void HandleTexture (Tds_Texture vTexture)
	{
		//switch between skin
		if (grid.tilePrefab == vTexture)
			GUI.skin = vCustSkinSelected;
		else
			GUI.skin = vCustSkin; 

		if (GUILayout.Button (new GUIContent ("", vTexture.vTexture), GUILayout.Width (50), GUILayout.Height (50))) {
			grid.tilePrefab = vTexture; 
		}
	}

	public void RefreshPreview()
	{
		ShowFunctionUsed ("RefreshPreview");
		//destroy the gameobject if not on scene
		if (TilePreview != null) {
			foreach (GameObject vObject in TilePreview)
				GameObject.DestroyImmediate(vObject);
			TilePreview = null;
		}
	}

	public override void OnInspectorGUI()
	{
		RefreshPreview ();

		string[] vArrayString = new string[] { "Tiles", "Settings" };

		//check if we have a pixel tile selected
		if (vSelectedTds_Tile  != null)
			vArrayString = new string[] {"Tiles", "Settings", "SelectedTile" };

		TabIndex = GUILayout.Toolbar (TabIndex, vArrayString);
		switch (TabIndex) {
			case 0 : 
				ShowTileSet();
			break;
			case 1 : 
				ShowSettings();	
			break;
		}

	}

	private float createSlider(string labelName, float sliderPosition){
		GUILayout.BeginHorizontal ();
		GUILayout.Label ("Grid " + labelName);
		sliderPosition = EditorGUILayout.Slider (sliderPosition, 1f, 100f, null);
		GUILayout.EndHorizontal ();

		return sliderPosition;
	}

	//Setting//
	private void ShowSettings()
	{
		//grid.width = createSlider ("Width", grid.width);
		//grid.height = createSlider ("Height", grid.height);
		grid.dimension = EditorGUILayout.IntField("Pixel Dimension", grid.dimension); ;
		grid.width = 4f;//Mathf.Sqrt (grid.dimension);
		grid.height = 4f;//Mathf.Sqrt (grid.dimension);

		if (GUILayout.Button ("Open Grid Windows")) {
			Tds_GridWindow window = (Tds_GridWindow)EditorWindow.GetWindow(typeof(Tds_GridWindow));
			window.init();
		}

		Texture2D vTexturePixel = null;
		if (grid.teleportPrefab != null)
			vTexturePixel = grid.selectionPrefab.vTexture;
	}

	//refresh the TileSet Path
	public void RefreshTileSet(string vpath)
	{
		IsRefreshing = true;

		//if we have found a real folder, we create a new fresh array for the new PNG files
		grid.tileSet.prefabs = new List<Tds_Folder>();

		//check if the folder exist on the local computer
		if (Directory.Exists(vpath))
		{
			grid.tileSet.TileSetPath = vpath;
			int cpt = 0;
			string[] allfiles = System.IO.Directory.GetFiles(vpath, "*.png", System.IO.SearchOption.AllDirectories);
			foreach (string vFilePath in allfiles)
			{
				//get the filepath without extension
				string vpath2 = vFilePath.Substring(vFilePath.IndexOf("Resources")+10);
				vpath2 = vpath2.Substring(0, vpath2.Length - 4);
				vpath2 = vpath2.Replace("\\","/");

				//get to the right folder
				string[] vFolder = vpath2.Split('/');

				//get the last one 
				string vLastF = vFolder [vFolder.Length - 2];
				string vFileName = vFolder [vFolder.Length - 1];

				//try to get the master folder name 
				string vMasterF = ""; 
				if (vFolder.Length > 2)
					vMasterF = vFolder [vFolder.Length - 3];

				//create the new Tds_Texture
				Tds_Texture vTexture = new Tds_Texture();

				//load the texture!
				Texture2D vText = Resources.Load(vpath2,typeof(Texture2D)) as Texture2D;

				//store the filename
				vTexture.vFilename = Path.GetFileName(vFilePath);
				vTexture.vTexture = vText;

				//check if it exist!
				if (grid.tileSet.prefabs.Find (x => GetNiceName(x.vFolderName) == GetNiceName(vLastF) && GetNiceName(x.vMasterFolderName) == GetNiceName(vMasterF)) == null) {
					//create a new pixel folder
					Tds_Folder vNewFolder = new Tds_Folder ();
					vNewFolder.vFolderName = GetNiceName(vLastF);				//keep the parent folder name
					vNewFolder.vMasterFolderName = GetNiceName(vMasterF); 		//keep the master folder name

					//check if there is a animated tile to add 
					if (vFileName.Contains ("[")) {
						vTexture.vAnimationList .Add (vTexture); 				//add itself to the list for the first animated tile
					}

					//then add the pixeltexture to the list
					vNewFolder.vPixelTextureList.Add(vTexture);

					//add it 
					grid.tileSet.prefabs.Add (vNewFolder);
				}
				else foreach (Tds_Folder vTds_Folder in grid.tileSet.prefabs)
				{
					if (GetNiceName(vTds_Folder.vFolderName) == GetNiceName(vLastF))
					{
						//check if it's a animated tile or normal tile
						if (vFileName.Contains ("[")) {

							bool vFound = false;
							string vShortFileName = vFileName.Substring (0, vFileName.LastIndexOf ("[")); //remove the [#]

							//check if it's the first animated Texture
							foreach (Tds_Texture vAPixelT in vTds_Folder.vPixelTextureList)
							{
								if (!vFound) {
									//get the filename
									string[] vF = vAPixelT.vFilename.Split ('/');
									string vFN = vF [vF.Length - 1];

									//check if it's animated!
									if (vFN.Contains ("[")) {
										//remove the [#]
										vFN = vFN.Substring (0, vFN.LastIndexOf ("["));

										//check if match!
										if (vShortFileName == vFN) {
											vFound = true;

											//add it for this one
											vAPixelT.vAnimationList.Add(vTexture);
										}
									}
								}
							}

							//if we still haven't found it, we just add the texture like a normal one, but add itself to the animation list so the next one will go in his list
							if (!vFound) {
								//add himself to the animation list before adding to the main list
								vTexture.vAnimationList.Add (vTexture);
								vTds_Folder.vPixelTextureList.Add (vTexture);
							}
						}
						else
							vTds_Folder.vPixelTextureList.Add (vTexture); //found the folder, then add the texture to it
					}
				}
			}
		}
		else
			grid.tileSet.TileSetPath = "";

		IsRefreshing = false;
	}

	//get the First Transform 
	Transform GetTransformFromPosition (Vector3 aligned){
		int i = 0;
		while (i < vTilesObjects.transform.childCount) {
			Transform transform = vTilesObjects.transform.GetChild(i);
			if (transform.position == aligned){
				return transform;
			}
			i++;
		}

		return null;
	}

	//check if the same Texture2D has been found
	List<GameObject> GridHaveThisTile (Texture2D vTexture, Vector3 aligned){

		List<GameObject> vTileObject = new List<GameObject>();
		for (int vNbrSizeX = 0; vNbrSizeX < TileSize; vNbrSizeX++)
		{
			for (int vNbrSizeY = 0; vNbrSizeY < TileSize; vNbrSizeY++) {
				int i = 0;
				while (i < vTilesObjects.transform.childCount) {
					Transform transform = vTilesObjects.transform.GetChild (i);

					//if have the same position, check if we have the same Texture2D
					if (transform.position == aligned + new Vector3(grid.width * vNbrSizeX, grid.height * vNbrSizeY, 0f)) {

						Sprite vsprite = transform.GetComponent<SpriteRenderer> ().sprite; 
						Tds_Tile vTds_Tile = transform.GetComponent<Tds_Tile> ();
						vTileObject.Add (transform.gameObject);
					}
					i++;
				}
			}
		}

		//return a result
		return vTileObject;
	}

	//check if the same Texture2D has been found
	public void GridHaveThisTile2 (Texture2D vTexture, Vector3 aligned, bool IsPreview)
	{
		ShowFunctionUsed("GridHaveThisTile2");
		GameObject vObjectToDelete = null;

		List<Tds_Tile> vTileAligned = GetTds_TilesInGrid (aligned);
		foreach (Tds_Tile vTds_Tile in vTileAligned)
			if (vTds_Tile.GetComponent<SpriteRenderer>().sprite.texture == vTexture)
				vObjectToDelete = vTds_Tile.gameObject;

		if (vObjectToDelete != null && !IsPreview)
			GameObject.DestroyImmediate (vObjectToDelete);
	}


	//create a list of object to be created
	List<GameObject> CreateTile(int vOrder, Tds_Tile.cTileType vcTile, Vector3 valigned, bool IsPreview = false)
	{
		ShowFunctionUsed ("CreateTile");
		//intialize the new list
		List<GameObject> ListObject = new List<GameObject> ();

		//every sprite will be for example 8x8 = 64pixels by 64pixels
		for (int vNbrSizeX = 0; vNbrSizeX < TileSize; vNbrSizeX++)
		{
			for (int vNbrSizeY = 0; vNbrSizeY < TileSize; vNbrSizeY++)
			{
				//by default, get the dimension needed
				int vDimX = grid.dimension;
				int vDimY = grid.dimension;

				if (grid.tilePrefab == grid.selectionPrefab)
				{
					vDimX = 64; //use default dimension for the teleport prefab
					vDimY = 64;
				}
				
				int vXcpt = 0;
				int vXLeft = grid.tilePrefab.vTexture.width;

				while (vXLeft > 0) {
					//reinitalize the Y variable
					int vYLeft = grid.tilePrefab.vTexture.height;
					int vYcpt = 0;

					while (vYLeft > 0) {
						//get the new position of this tile
						Vector3 vNewPosition = valigned + new Vector3 (grid.width * vXcpt, grid.height * vYcpt, 0f) + new Vector3 (grid.width * vNbrSizeX, grid.height * vNbrSizeY, 0f);

						//Check if the grid already has that tile on the same very position. if yes, we delete it to have the new info! Just like a replace
						if (!IsPreview)
							GridHaveThisTile2 (grid.tilePrefab.vTexture, vNewPosition, IsPreview);

						//create a different tile for 2D Walls
						GameObject vNewObj = vTileObject;
						if (vcTile == Tds_Tile.cTileType.Wall)
							vNewObj = vWallObject;

						//before going further, we check if it already exist
						GameObject gameObject = (GameObject)PrefabUtility.InstantiatePrefab (vNewObj);

						//change its tag for Tile
						gameObject.tag = "Tile";
						gameObject.name = grid.tilePrefab.vFilename.Substring (0, grid.tilePrefab.vFilename.Length - 4); //rename the gameobject + remove file extension
						gameObject.transform.localScale = new Vector3 (vFactorScale,vFactorScale, 1f);

						//handle the different size
						if (grid.tilePrefab == grid.selectionPrefab)
							gameObject.transform.localScale = new Vector3 (16f,16f, 1f);

						SpriteRenderer vRenderer = gameObject.GetComponent<SpriteRenderer> ();

						//make sure we get the last pixel even if it's not standard
						int vDimTakenX = vDimX;
						if (vDimTakenX > vXLeft)
							vDimTakenX = vXLeft;

						//make sure we get the last pixel even if it's not standard
						int vDimTakenY = vDimX;
						if (vDimTakenY > vYLeft)
							vDimTakenY = vYLeft;
					
						Sprite vsprite = Sprite.Create (grid.tilePrefab.vTexture, new Rect (vXcpt * vDimX, vYcpt * vDimY, vDimTakenX, vDimTakenY), new Vector2 (0f, 0f), 128f);
						vRenderer.sprite = vsprite;

						//show the tile preview in RED when deleting
						if (vAction == "DeleteTile")
							vRenderer.color = Color.red;
						
						//if teleport, get the teleport color by deault
						//put the teleport tile above all of the others
						if (grid.vTileType == Tds_Tile.cTileType.Teleport) {
							vRenderer.color = grid.TeleportColor;
							gameObject.GetComponent<SpriteRenderer> ().sortingOrder = 1500;
						}

						//change the tile wall
						Tds_Tile vTiles = gameObject.GetComponent<Tds_Tile> ();
						vTiles.vTileType = vcTile;
						vTiles.AnimationSpeed = grid.AnimationSpeed;

						//send the animated tiles
						if (grid.tilePrefab.vAnimationList.Count > 0) {

							foreach (Tds_Texture vAPixelT in grid.tilePrefab.vAnimationList)
								vAPixelT.vOrder = int.Parse(Regex.Replace (vAPixelT.vTexture.name, "[^0-9]", ""));

							//sort
							grid.tilePrefab.vAnimationList = grid.tilePrefab.vAnimationList.OrderBy(x=>x.vOrder).ToList();

							foreach (Tds_Texture vAPixelT in grid.tilePrefab.vAnimationList)
								if (!vTiles.vAnimationList.Contains (vAPixelT.vTexture))				//don't add texture already in it
									vTiles.vAnimationList.Add (vAPixelT.vTexture);
						}

						//keep a trace of the layerorder for this very tile so we can draw them correctly between levels
						vTiles.LayerOrder = vOrder;

						//make sure there is a spriterenderer on the prefab to apply the orderlayer
						if (gameObject.GetComponent<SpriteRenderer> () != null)
							gameObject.GetComponent<SpriteRenderer> ().sortingOrder = vTiles.GetSortingOrderByTile(); //return the right drawing order for this new tile

						//change position
						gameObject.transform.position = vNewPosition;

						if (vcTile == Tds_Tile.cTileType.Wall || vcTile == Tds_Tile.cTileType.Destructible)
							gameObject.AddComponent<PolygonCollider2D> ();

						//check if teleport, create 2 gameobject wich will be linked together
						if (grid.vTileType == Tds_Tile.cTileType.Teleport && vOrder != 1500) {
							//create a copy of it at the same location
							GameObject gameObject2 = Instantiate (gameObject);

							//link them together
							gameObject.GetComponent<Tds_Tile> ().vLinkedObject = gameObject2;
							gameObject2.GetComponent<Tds_Tile> ().vLinkedObject = gameObject;

							//add them
							ListObject.Add (gameObject2);
						}
										
						//add it
						ListObject.Add (gameObject);

						//remove pixels Y
						vYLeft -= vDimY;
						
						vYcpt++;
					}

					//remove pixels X
					vXLeft -= vDimX;

					vXcpt++;
				}
			}
		}

		return ListObject;
	}

	//save locally the variable and on gamemanager which will hold them all for next time we will use the GridEditor
	void SaveVariables(string vNewAction)
	{
		//local variables
		vAction = vNewAction;
	}

	public List<Tds_Tile> GetTds_TilesInGrid(Vector3 aligned)
	{
		ShowFunctionUsed ("GetTds_TilesInGrid");

		//initialise the variables
		List<Tds_Tile> vTilesFound = new List<Tds_Tile>();

		//get all object child of grid which has the same position as the grid we are on it!
		List<GameObject> vTilesO= vTilesObjects.transform.Cast<Transform>().Select(t=>t.gameObject).Where(t=>t.transform.position == aligned).ToList();

		//get all the texture in this
		foreach (GameObject vObject in vTilesO)
				vTilesFound.Add (vObject.GetComponent<Tds_Tile> ());

		//return all the objects in a list
		return vTilesFound;
	}

	void RefreshEditor()
	{
		//reset the top menu 
		TopMenuPickerIndex = -1;
		vSelectedTds_Tile = null;

		//switch to the Selected Tile manually
		TabIndex = 0;
		Repaint();
	}
		
	
	void OnSceneGUI()	
	{
		/////////////TOP MENU/////////////
		//go back to normal skin
		GUI.skin = vNONESkin;
	

		////////////////tile sie/////////////
		if (IsSizingTile) {
			GUILayout.BeginHorizontal ();

			//go back to normal skin
			GUI.skin = vNONESkin;

			//get the number of item to be shown
			vNbrButton = 3;
			vBoxSize = 65; 

			//create a windows
			GUILayout.Window (3, new Rect (Screen.width - 50 - (vBoxSize * vNbrButton), 0, vNbrButton * 85, 100), (id) => {

				//by default it's -1
				TileSize = -1;

				List<Texture2D> vSizeList = new List<Texture2D> ();
				vSizeList.Add (v1x1);
				vSizeList.Add (v2x2);
				vSizeList.Add (v3x3);

				//get the right size on the list
				TileSize = GUILayout.SelectionGrid (TileSize, vSizeList.ToArray (), 20);GUILayout.Height (vBoxSize);

				//check if we selected something
				if (TileSize > -1) {
					//increase that number by 1
					TileSize++;

					//switch back to CreateTile
					SaveVariables ("CreateTile");

					IsSizingTile = false;
				}

			}, "");

			//go back to normal skin
			GUI.skin = null;
			GUILayout.EndHorizontal ();
		}
		//////////////////////////////////

			/////////////COLOR PICKER///////////////
			//check if we have something so we show them, the user will select which of them we want! 
			//may have many many tile on the same grid!
			if (vTilesFound != null)
			if (vTilesFound.Count > 0) {

				//remove teleport tile before clicking on it
				grid.tilePrefab = null;

				GUILayout.BeginHorizontal ();

				//go back to normal skin
				GUI.skin = vCustSkin;
				
				//get the number of item to be shown
				vNbrButton = vTilesFound.Count;
				int vBoxSizeY = 96; 
				int vBoxSizeX = 40;
				int vLargestX = 96;
				
				//make sure we got enought room to show the current texture
				foreach (Texture2D vCurText in vTilesFound) {
				
					//incease width for every tiles
					vBoxSizeX += vCurText.width;

					//the main bos is equal to the biggest tiles in the list
					if (vBoxSizeY < vCurText.height)
						vBoxSizeY = vCurText.height;

					//keep the longest so we can draw the button correctly
					if (vLargestX < vCurText.width)
						vLargestX = vCurText.width;
				}

					//create a windows
					GUILayout.Window (3, new Rect (Screen.width - (vLargestX*vNbrButton)-10, 20, (vLargestX*vNbrButton), vBoxSizeY), (id) => {

					ColorPickerIndex = -1;

					//get the right color picker on the list + ALSO get the same tiles specs  EX : Ground, Order layer 40, levels, isdoor...)
					ColorPickerIndex = GUILayout.SelectionGrid (ColorPickerIndex, vTilesFound.ToArray (), vTilesFound.Count, GUILayout.Height(vBoxSizeY-20));

					//check if we selected something
					if (ColorPickerIndex > -1) {
						Tds_Texture vTextureFound = new Tds_Texture ();

						//get the right texture to use!
						foreach (Tds_Folder vFolder in grid.tileSet.prefabs)
							foreach (Tds_Texture vTexture in vFolder.vPixelTextureList)
								if (vTexture.vTexture == vTilesFound [ColorPickerIndex])
									vTextureFound = vTexture;
								
						//if we found it, we make our default pixeltexture this one
						if (vTextureFound != null) {
							//check if we SELECT the tiles or we ONLY GET THE TILES TO REDRAW
							if (vAction == "SelectTile")
							{
								GameObject vGameObjectSel = null;
								//get the right texture to use!
								foreach (GameObject vCurObject in vGameobjectsFound)
								{
									if (vCurObject.GetComponent<SpriteRenderer>().sprite.texture == vTextureFound.vTexture)
										vGameObjectSel = vCurObject;
								}

								//try to see if it's the teleport gameobject we want to get
								if (vGameObjectSel == null)
									foreach (GameObject vCurObject in vGameobjectsFound)
									{	
										//get the first teleport in this
										if (vCurObject.GetComponent<Tds_Tile>().vTileType == Tds_Tile.cTileType.Teleport)
											vGameObjectSel = vCurObject;
									}

								//check if we found it, so we select it to see the inspector!
								if (vGameObjectSel != null)
								{
									//get the pixel tile
									vSelectedTds_Tile = vGameObjectSel.GetComponent<Tds_Tile>();

									//switch to the Selected Tile manually
									TabIndex = 2;

									//reset both list
									vTilesFound = new List<Texture2D> ();
									vGameobjectsFound = new List<GameObject>();

									//refresh editor
									Repaint();
								}
							}
							else
							{
								//switch to Create Tile
								SaveVariables ("CreateTile");
						
								//get the new texture!
								grid.tilePrefab = vTextureFound;

								//try to get the 
								foreach (GameObject vCurObject in vGameobjectsFound)
								{
									if (vCurObject.GetComponent<SpriteRenderer>().sprite.texture == vTextureFound.vTexture)
									{
										//get the pixel tile!
									 	Tds_Tile vCurTds_Tile = vCurObject.GetComponent<Tds_Tile>();

										//now get the right info about this tile on the grid
										//grid.IsDoor = 	vCurTds_Tile.IsDoor;
										grid.OrderLayer = 	vCurTds_Tile.LayerOrder;
										grid.vTileType = 	vCurTds_Tile.vTileType;
										grid.ShowParticle = 	vCurTds_Tile.ShowParticles;
									}
								}

								//remove color picker
								vTilesFound = new List<Texture2D> ();
							}
						}
					}

				}, "");

				//go back to normal skin
				GUI.skin = null;
				GUILayout.EndHorizontal ();
			}

			////////////////Level Selector///////////////
			Handles.BeginGUI ();

			//stretch with the number of buttons
			vNbrButton = 5; 
			vBoxSize = 50;

			//go back to normal skin
			GUI.skin = null;

			//GUILayout.EndHorizontal ();
			Handles.EndGUI ();
			//////////////END of the Level Selector///////

	
			////////////////TILE EDITOR///////////////
			Handles.BeginGUI ();
			//GUILayout.BeginHorizontal ();

			//stretch with the number of buttons
			vNbrButton = 7; 
			vBoxSize = 50;

			//go back to normal skin
			GUI.skin = vNONESkin;

			//create a windows
			GUILayout.Window (2, new Rect (Screen.width - 65, Screen.height - (57 * vNbrButton), 70, vNbrButton * 85), (id) => {
				//check if we selected create tile on the GUI
				Texture2D vCreateTileToggle = vCreateTileText;
				if (vAction == "CreateTile")
					vCreateTileToggle = vCreateTileTextSel;

				//check if we have select tile selected
				Texture2D vSelectTileToggle = vSelectTileText;
				if (vAction == "SelectTile")
					vSelectTileToggle = vSelectTileTextSel;

				//check if we selected create tile on the GUI
				Texture2D vDeleteTileTextToggle = vDeleteTileText;
				if (vAction == "DeleteTile")
					vDeleteTileTextToggle = vDeleteTileTextSel;

				//check if we selected create tile on the GUI
				Texture2D vColorPickerToggle = vColorPickerText;
				if (vAction == "ColorPicker") {
					vColorPickerToggle = vColorPickerTextSel;
				}
				
				//get the right WallIcon
				Texture2D vWallIcon = vCanSeeWall;
				if (!IsLookingWall)
					vWallIcon = vCannotSeeWall;

				//by default it's 1x1
				Texture2D vSelSizeText = v1x1;
				if (TileSize == 2)
					vSelSizeText = v2x2;
				else if (TileSize == 3)
					vSelSizeText = v3x3;

				//Tile Size
				if (GUILayout.Button (vSelSizeText, GUILayout.Width (vBoxSize), GUILayout.Height (vBoxSize))) {
					SaveVariables ("SetSize");

					//remove color picker
					vTilesFound = new List<Texture2D> ();

					//show the size tile
					IsSizingTile = true;

					//refresh editor
					RefreshEditor();
				}

				//Selection Tile
				if (GUILayout.Button (vSelectTileToggle, GUILayout.Width (vBoxSize), GUILayout.Height (vBoxSize))) {
					//remove old preview
					RefreshPreview();

					//remove the current prefab to select a new one
					grid.tilePrefab = null;

					//switch action
					SaveVariables ("SelectTile");

					//when selecting, we have a tile size of 1
					TileSize = 1;

					//remove color picker
					vTilesFound = new List<Texture2D> ();

					//refresh editor
					RefreshEditor();
				}

				//Create Tile
				if (GUILayout.Button (vCreateTileToggle, GUILayout.Width (vBoxSize), GUILayout.Height (vBoxSize))) {
					SaveVariables ("CreateTile");

					//remove color picker
					vTilesFound = new List<Texture2D> ();

					//make sure a user doesn't waste his time being on a teleport
					if (grid.vTileType == Tds_Tile.cTileType.Teleport)
						grid.vTileType = Tds_Tile.cTileType.Ground;

					//refresh editor
					RefreshEditor();
				}

				//Delete Tile
				if (GUILayout.Button (vDeleteTileTextToggle, GUILayout.Width (vBoxSize), GUILayout.Height (vBoxSize))) {
					SaveVariables ("DeleteTile");


					//remove color picker
					vTilesFound = new List<Texture2D> ();

					//refresh editor
					RefreshEditor();
				}

				//Color Picker
				if (GUILayout.Button (vColorPickerToggle, GUILayout.Width (vBoxSize), GUILayout.Height (vBoxSize)))
				{
					SaveVariables ("ColorPicker");
					RefreshEditor();
				}
			}, "");
			
			//go back to normal skin
			GUI.skin = null;

			//GUILayout.EndHorizontal ();
			Handles.EndGUI ();
			//////////////END of the TILE EDITOR///////

			int controlId = GUIUtility.GetControlID (FocusType.Passive);
			Event e = Event.current;
			Ray ray = Camera.current.ScreenPointToRay (new Vector3 (e.mousePosition.x, -e.mousePosition.y + Camera.current.pixelHeight));
			Vector3 mousePos = ray.origin;

			Texture2D prefab = null;
			if (grid.tilePrefab != null)
				prefab = grid.tilePrefab.vTexture;

			//calculate where is the mouse cursor
			Vector3 aligned = new Vector3 (Mathf.Floor (mousePos.x / grid.width) * grid.width, Mathf.Floor (mousePos.y / grid.height) * grid.height, 0.0f);

			//check if the texture has been found before going further
			if (prefab) {
				//check if we have a prefab already bruilt
				if (TilePreview == null) {
					//initialize it before
					TilePreview = new List<GameObject> ();

					//create a preview tile by default
					TilePreview = CreateTile (1500, Tds_Tile.cTileType.Ground, aligned, true);

					//positionnate the tilepreview to be shown at the mouse location
					foreach (GameObject vObject in TilePreview) {
						Undo.IncrementCurrentGroup ();

						//vObject.transform.position = aligned;
						vObject.transform.parent = vAutoTilesObjects.transform;

						Undo.RegisterCreatedObjectUndo (vObject, "Create" + vObject.name);
					}

				} else if (aligned != lastaligned){ //only refresh preview if we change square
					lastaligned = aligned; 
					RefreshPreview ();
				}
			}

			if (e.isMouse && e.button == 0 && (e.type == EventType.mouseDown || e.type == EventType.MouseDrag)) {
				if (vLastEventType == EventType.Ignore) {
					GUIUtility.hotControl = controlId;
					e.Use ();
					vLastEventType = e.type;
				}

				List<GameObject> gameObject = new List<GameObject> ();

				//check if the texture has been found before going further
			if (prefab && vAction == "CreateTile") {
				gameObject = CreateTile (grid.OrderLayer, grid.vTileType, aligned);
			
				//positionnate the tilepreview to be shown at the mouse location
				foreach (GameObject vObject in gameObject) {
					Undo.IncrementCurrentGroup ();

					//vObject.transform.position = aligned;
					vObject.transform.parent = vTilesObjects.transform;

					Undo.RegisterCreatedObjectUndo (vObject, "Create" + vObject.name);
				}
			} 
				//destroy the same tiles!
				else if (vAction == "DeleteTile") {
				//delete the same tile that we got we found!
				List<GameObject> vFoundTile = GridHaveThisTile (prefab, aligned);

				//if we have found the same tiles, we can now delete it!
				if (vFoundTile != null)
				if (vFoundTile.Count > 0)
					foreach (GameObject vTileToDestroy in vFoundTile)
						GameObject.DestroyImmediate (vTileToDestroy);
			} 
		}//allow right mouse
		else if (e.type == EventType.MouseUp && vLastEventType != null && e.button == 0) {
			GUIUtility.hotControl = 0;
			vLastEventType = EventType.Ignore; //use ignore EventType to skip it
		}
	}
}
