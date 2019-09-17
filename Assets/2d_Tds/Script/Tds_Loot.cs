using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AssemblyCSharp;
using UnityEngine.UI;

public class Tds_Loot : MonoBehaviour {

	public Tds_Items vItems;
	private Ray ray;
	private RaycastHit hit;
	private bool ShowItem = false;
	private Tds_GameManager vGameManager;
	private bool CursorIsAbove = false;

	Transform vDmgLabel;
	Transform vDmgValue;
	Transform vAmmoLabel;
	Transform vAmmoValue;
	Transform vPanel;
	Transform vNameValue;

	void Start()
	{
		ShowItem = false;
	}

	public Sprite GetAssocValue(WeaponValueType vType)
	{
		Sprite vIcon = vGameManager.vLowIcon;

		if (vType == WeaponValueType.Average)
			vIcon = vGameManager.vMediumIcon;
		else if (vType == WeaponValueType.High)
			vIcon = vGameManager.vHighIcon;
		else if (vType == WeaponValueType.GODLY)
			vIcon = vGameManager.vGodlyIcon;

		return vIcon;
	}

	//loot has the complete list for showing the right result on it's own
	public void InitialiseLoot(Tds_Items vNewItem, Tds_GameManager vTGameManager)
	{
		vGameManager = vTGameManager;
		//get the items
		vItems = vNewItem;

		//show or hide info about weapons
		vDmgLabel = transform.Find ("DmgLabel");
		vDmgValue = transform.Find ("DmgValue");
		vAmmoLabel = transform.Find ("AmmoLabel");
		vAmmoValue = transform.Find ("AmmoValue");
		vPanel = transform.Find ("Panel");
		vNameValue = transform.Find ("Name");

		//item name
		vNameValue.GetComponent<Text>().text = vNewItem.vName;

		//potions infos here

		//check if it's a weapon or a potion
		if (vNewItem.GiveWeapon) {
			//item name
			vDmgValue.GetComponent<Image> ().sprite = GetAssocValue(vNewItem.vDmgType);
			vAmmoValue.GetComponent<Image> ().sprite = GetAssocValue(vNewItem.vAmmoType);
		}

		//disable everything by default
		vDmgLabel.gameObject.SetActive(false);
		vDmgValue.gameObject.SetActive(false);
		vAmmoLabel.gameObject.SetActive(false);
		vAmmoValue.gameObject.SetActive(false);
		vPanel.gameObject.SetActive (false);
		vNameValue.gameObject.SetActive (false);

		//doens't show the item at first
		ShowItem = false;
	}

	public void ShowHide(bool vShow)
	{
		bool CanShow = false;
		if (vShow || CursorIsAbove)
			CanShow = true;

		//make sure we show/hide correctly (with player above items or with cursor)
		vNameValue.gameObject.SetActive (CanShow);
		vPanel.gameObject.SetActive (CanShow);

		if (vItems.GiveWeapon) {
			vDmgLabel.gameObject.SetActive (CanShow);
			vDmgValue.gameObject.SetActive (CanShow);
			vAmmoLabel.gameObject.SetActive (CanShow);
			vAmmoValue.gameObject.SetActive (CanShow);
		}
			
		//replace old value
		ShowItem = CanShow;

	}

	void OnMouseOver()
	{
		CursorIsAbove = true;

		if (!ShowItem)
			ShowHide(!ShowItem);
	}

	void OnMouseExit()
	{
		CursorIsAbove = false;

		if (ShowItem)
			ShowHide(!ShowItem);
	}
}
