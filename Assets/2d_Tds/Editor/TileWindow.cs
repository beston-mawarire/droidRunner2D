using UnityEngine;
using UnityEditor;
using System.Collections;

[System.Serializable]
public class TileWindow : EditorWindow {
	Tds_Grid grid;

	public void init(){
		grid = (Tds_Grid)FindObjectOfType(typeof(Tds_Grid));
	}

	void OnGUI()
	{
		//make sure we have something selected before
		if (grid.tileSet != null)
		{
			Texture2D vTexture = grid.tilePrefab.vTexture;

			//Rect textureCrop = new Rect (vXcpt*vDim, vYcpt*vDim,vDimTakenX, vDimTakenY);
			Rect textureCrop = new Rect (0, 0f,64, 64f);
			GUI.BeginGroup( new Rect(0f, 0f, vTexture.width * textureCrop.width, vTexture.height * textureCrop.height ) );
			GUI.DrawTexture( new Rect( -vTexture.width * textureCrop.x, -vTexture.height * textureCrop.y, vTexture.width, vTexture.height ), vTexture );
			GUI.EndGroup();
			
			//every sprite will be for example 8x8 = 64pixels by 64pixels
			int vDim = (int)(grid.width * grid.height);

			//reinitalize the Y variable
			int vYLeft = grid.tilePrefab.vTexture.height;
			int vYcpt = 0;
			
			while (vYLeft  > 0) 
			{
				int vXLeft = grid.tilePrefab.vTexture.width;
				int vXcpt = 0;

				GUILayout.BeginHorizontal();

				while (vXLeft> 0)
				{					
					//make sure we get the last pixel even if it's not standard
					int vDimTakenX = vDim;
					if (vDimTakenX > vXLeft)
						vDimTakenX = vXLeft;
					
					//make sure we get the last pixel even if it's not standard
					int vDimTakenY = vDim;
					if (vDimTakenY > vYLeft)
						vDimTakenY = vYLeft;
					
					Sprite vsprite = Sprite.Create (grid.tilePrefab.vTexture, new Rect (vXcpt*vDim, vYcpt*vDim,vDimTakenX, vDimTakenY), new Vector2 (0f, 0f), 128f);

					//remove pixels X
					vXLeft-= vDim;
					
					vXcpt++;
				}

				GUILayout.EndHorizontal();				
				
				//remove pixels Y
				vYLeft-= vDim;
				
				vYcpt++;

			}
		}
	}
}
