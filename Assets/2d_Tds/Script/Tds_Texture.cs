using System;
using UnityEngine;
using System.Collections.Generic;
namespace AssemblyCSharp
{
	public class Tds_Texture
	{
		public Texture2D vTexture = null;
		public string vFilename="";
		public int vOrder = 0;

		//if we have something here, it mean we're animating this texture!
		public List<Tds_Texture> vAnimationList = new List<Tds_Texture>();	
	}

	public class Tds_Folder
	{
		public string vFolderName = "";
		public string vMasterFolderName = "";
		public List<Tds_Texture> vPixelTextureList = new List<Tds_Texture>();
	}
}

