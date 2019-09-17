using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;
using System.IO;

[System.Serializable]
public class Tds_TileSet : ScriptableObject {
	public string TileSetPath = "";
	public List<Tds_Folder> prefabs = new List<Tds_Folder>();
}
