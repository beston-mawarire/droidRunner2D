using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;

namespace AssemblyCSharp
{
	public enum ItemName{Potion, Pistol, EnergyRifle, Ak47, Shotgun, DemoGun1};
	public enum WeaponValueType{Low, Average, High, GODLY};

	[System.Serializable]
	public class Tds_Items {

		//check what we give 
		public string vName = "";
		public ItemName vItemName = ItemName.Potion;
		public Sprite ItemIcon = null;						//how it will look like when dropping on ground
		public bool GiveWeapon = false;
		public bool Usable = true;							//if spawn debris = false, we just spawn a item on ground 
		public WeaponValueType vDmgType = WeaponValueType.Low;
		public WeaponValueType vAmmoType = WeaponValueType.Low;
		public WeaponName vWeaponName = WeaponName.Pistol;
	}
}