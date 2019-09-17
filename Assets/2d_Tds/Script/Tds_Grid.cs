using UnityEngine;
using System.Collections;
using AssemblyCSharp;
using System.Collections.Generic;
public class Tds_Grid : MonoBehaviour {
	public float width = 8f;
	public float height = 8f;
	public int dimension = 16;

	public Color color = Color.white;
	public Color WallColor = Color.red;
	public Color GroundColor = Color.white;
	public Color TeleportColor = Color.green;

	public Tds_Texture tilePrefab;

	public Tds_TileSet tileSet;
	public Texture2D vTestText;

	public int OrderLayer = 0;
	public bool IsDoor = false;
	public float AnimationSpeed = 0.2f;
	public bool ShowParticle = false;

	public Tds_Tile.cTileType vTileType;
	public Tds_Texture teleportPrefab;
	public Tds_Texture selectionPrefab;

	void OnDrawGizmos()
	{
		Vector3 pos = Camera.current.transform.position;
		Gizmos.color = this.color;

		for (float y = pos.y - 800.0f; y < pos.y + 800.0f; y+= this.height) {
			Gizmos.DrawLine(new Vector3(-10000000, Mathf.Floor(y/this.height)*this.height, 0.0f), 
			                new Vector3(10000000, Mathf.Floor(y/this.height)*this.height, 0.0f));
		}

		for (float x = pos.x - 1200.0f; x < pos.x + 1200.0f; x+= this.width) {
			Gizmos.DrawLine(new Vector3(Mathf.Floor(x/this.width)*this.width, -10000000, 0.0f), 
			                new Vector3(Mathf.Floor(x/this.width)*this.width, 10000000, 0.0f));
		}
	}
}
