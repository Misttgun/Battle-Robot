using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleRobo
{
	public class WeaponHolderScript : MonoBehaviour
	{
		//TODO Rendre le code plus propre avec le system d'inventaire et de pickup
		public WeaponScript[] equipWeapons;
		public int selectedWeapon;

		private void Start()
		{
			SelectWeapon();
		}

		private void Update()
		{
			int prevSelectedWeapon = selectedWeapon;

			if (Input.GetAxis("Mouse ScrollWheel") > 0f)
			{
				if (selectedWeapon >= equipWeapons.Length)
				{
					selectedWeapon = 0;
				}
				else
				{
					selectedWeapon++;
				}
			}

			if (Input.GetAxis("Mouse ScrollWheel") < 0f)
			{
				if (selectedWeapon <= 0)
				{
					selectedWeapon = equipWeapons.Length - 1;
				}
				else
				{
					selectedWeapon--;
				}
			}

			if (Input.GetKeyDown(KeyCode.Alpha1))
			{
				selectedWeapon = 0;
			}

			if (Input.GetKeyDown(KeyCode.Alpha2) && equipWeapons.Length >= 2)
			{
				selectedWeapon = 1;
			}

			if (Input.GetKeyDown(KeyCode.Alpha3) && equipWeapons.Length >= 3)
			{
				selectedWeapon = 2;
			}

			if (prevSelectedWeapon != selectedWeapon)
			{
				SelectWeapon();
			}
		}


		private void SelectWeapon()
		{
			int index = 0;
			foreach (WeaponScript weapon in equipWeapons)
			{
				if (index == selectedWeapon)
				{
					weapon.gameObject.SetActive(true);
				}
				else
				{
					weapon.gameObject.SetActive(false);
				}

				index++;
			}
		}
	}
}
