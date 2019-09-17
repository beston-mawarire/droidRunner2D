using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;

namespace AssemblyCSharp
{
	public enum WeaponName{Pistol, ZombieHand, EnergyRifle, Ak47, Shotgun, DemoGun1};

	public enum WeaponType{Melee, Ranged};
	
	[System.Serializable]
	public class Tds_Weapons {

		public string vName = "";
		public WeaponName vWeaponName;							//link the weapon for every NPC & Player by this field
		public WeaponType vWeaponType = WeaponType.Ranged;		//Melee = (knife, hand, club...) Ranged (all sort of rifle, pistol)
		public bool Is2Handed = false;							//check if we hold the weapon with 2 hand or not (Pistol or Rifle))
		public Sprite vWeaponIcon = null;						//show the right weapon at the bottom
		public GameObject vWeaponObj = null;					//Has the weapon + bullets position 
		public GameObject vAimObj = null;						//where the projectile come from
		public GameObject vProjectile = null;					//Projectile used in this weapon
		public List<float> vBulletAngleList;			//create as many bullet in the list with the specific angle (shotgun)
		public float vProjectileSpeed = 1f;						//how fast will the projectile go
		public float vTimeBtwShot = 0.5f;
		public float vTimeWaited = 0f;
		public GameObject vImpactFX = null;						//spawn something when impacting wall, npc. (explosion, spark...)
		public GameObject vShotFX = null;						//little spark when shooting 
		public int vDmg = 1;									//here we handle the dmg which will be transfered to the projectile
		public int Rebounce = 0;								//how many time the bullets will bounche on wall until destroyed
		public int vAmmoSize = 5;								//how many bullets are in this gun before recharging.
		public int vAmmoCur = 0;
		public GameObject vClipObj = null;						//leave a clip on the ground when reloading
		public string AttackAnimationUsed = "";					//each weapon can have their own shooting animation on the character. Put them here 
	}
}