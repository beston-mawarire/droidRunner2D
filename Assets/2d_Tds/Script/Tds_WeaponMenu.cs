using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;
using UnityEngine.UI;

namespace AssemblyCSharp
{
	[System.Serializable]
	public class Tds_WeaponMenu {
		public Tds_Weapons vWeapons = null;

		public Image WepPanel = null;
		public Image WeaponSprite = null;
		public Image AmmoBar = null;
		public Text AmmoValue = null;
		public Image ReloadImg = null;
	}
}