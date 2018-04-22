using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BattleRobo;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Video;

/// <summary>
/// A class which represent the player inventory
/// </summary>

namespace BattleRobo
{
	public class PlayerInventory
	{
		[SerializeField] private int inventorySize = 5;

		// - the inventory is just a List of object owned by the player
		private PlayerInventorySlot[] inventory;

		private PlayerUIScript playerUI;

		// - camera is the origin of the raycast to loot an object
		private Camera camera;

		// current active item, by default nothing is selected
		private int currentSlotIndex = -1;

		private PhotonView playerView;

		private WeaponHolderScript weaponHolder;

		/// <summary>
		/// Constructor : instatiate the inventory slots
		/// </summary>
		public PlayerInventory(GameObject camera, [CanBeNull] PlayerUIScript ui, PhotonView playerPhotonView)
		{
			playerView = playerPhotonView;
			this.camera = camera.GetComponent<Camera>();
			inventory = new PlayerInventorySlot[5];

			playerUI = ui;

			for (var i = 0; i < inventory.Length; i++)
				inventory[i] = new PlayerInventorySlot();
		}

		/// <summary>
		/// Check if inventory is full
		/// </summary>
		public bool IsFull()
		{
			for (var i = 0; i < inventorySize; i++)
				if (inventory[i].IsEmpty())
					return false;

			return true;
		}

		/// <summary>
		/// Check if inventory is empty
		/// </summary>
		public bool IsEmpty()
		{
			for (var i = 0; i < inventorySize; i++)
				if (!inventory[i].IsEmpty())
					return false;

			return true;
		}

		/// <summary>
		/// Find the first empty slot. Return -1 if there is no empty slot.
		/// </summary>
		public int FindFirstEmptySlot()
		{
			for (var i = 0; i < inventorySize; i++)
				if (inventory[i].IsEmpty())
					return i;

			return -1;
		}

		/// <summary>
		/// Find the first slot which can contain a given item.
		/// If the object is stackable, return the slot which contain this item
		/// else, return the first empty slot
		/// </summary>
		public int FindSlot(int itemId)
		{
			for (var i = 0; i < inventorySize; i++)
			{
				if (inventory[i].GetItemId() == itemId && inventory[i].GetStackSize() < inventory[i].GetItem().GetMaxStack())
					return i;
			}

			return FindFirstEmptySlot();

		}

		/// <summary>    
		/// Put an object in an inventory slot
		/// </summary>
		public void AddObject(PlayerObject obj, int slotIndex)
		{
			// can't add object if inventory is full

			if (slotIndex != -1)
				inventory[slotIndex].SetItem(obj);
		}

		/// <summary>
		/// Set the curretn active slot of the inventory (which is used when the player click)
		/// </summary>
		public void SwitchActiveIndex(int index)
		{
			SetActiveItem(index);

			var item = inventory[index].GetItem();

			if (item && item.GetWeapon() != null)
			{
				var weapon = item.GetWeapon();
				weaponHolder.SetWeapon(weapon, weapon.GetCurrentAmmo());
			}

			else
			{
				weaponHolder.SetWeapon(null, 0f);
			}

		}

		/// <summary>
		/// Update the invetory UI
		/// </summary>
		public void UpdateInventoryUI(int slotIndex)
		{
			playerUI.SetActiveUISlot(slotIndex);
		}


		/// <summary>
		/// Collect an item. Shoot a raycast to see if the player
		/// is targeting an item, then check distance. If the player
		/// can collect the item, try to find a slot on the inventory
		/// and add it if the inventory is not full
		/// </summary>
		public void Collect()
		{
			RaycastHit hit;
			Ray ray = camera.ScreenPointToRay((Input.mousePosition));
			Transform objectHit = null;

			if (Physics.Raycast(ray, out hit))
				objectHit = hit.transform;

			var playerGameObject = objectHit.gameObject;

			// - hit object is not from loot layer, exit
			if (playerGameObject.layer != 11)
				return;

			// - check distance
			if (hit.distance > 5f)
				return;


			var playerObject = playerGameObject.GetComponent<PlayerObject>();

			var slotIndex = FindSlot(playerObject.GetId());

			// - inventory is not full
			if (slotIndex != -1)
			{
				AddObject(playerObject, slotIndex);
				playerView.RPC("TakeObject", PhotonTargets.AllViaServer, playerObject.GetLootTrackerIndex());

				playerUI.SetItemUISlot(playerObject, slotIndex);

				// equip weapon if the index is already selected
				if (slotIndex == currentSlotIndex)
				{
					var weapon = playerObject.GetComponent<WeaponScript>();
					weaponHolder.SetWeapon(weapon, weapon.GetCurrentAmmo());
					playerUI.SetAmmoCounter(weapon.GetCurrentAmmo(), weapon.GetMagazineSize());

				}
			}
		}

		public void Drop(Vector3 position)
		{
			var playerObject = inventory[currentSlotIndex].GetItem();

			if (!playerObject)
				return;

			if (!playerView.isMine)
				return;

			// - place the weapon on the map and show it
			playerView.RPC("DropObject", PhotonTargets.AllViaServer, playerObject.GetLootTrackerIndex(), position);

			// - Update ammo counter
			playerView.RPC("UpdateWeapon", PhotonTargets.AllViaServer, playerObject.GetLootTrackerIndex(),
				playerObject.GetWeapon().GetCurrentAmmo());

			// - remove object from player inventory
			inventory[currentSlotIndex].Drop();

			// - update UI
			playerUI.SetItemUISlot(null, currentSlotIndex);
			playerUI.SetAmmoCounter(-1f, -1f);

			// - unequip weapon
			weaponHolder.SetWeapon(null, 0f);
		}

		public void SetActiveItem(int index)
		{
			currentSlotIndex = index;
			UpdateInventoryUI(index);

			var item = inventory[currentSlotIndex].GetItem();

			if (item && item.GetWeapon() != null)
				playerUI.SetAmmoCounter(item.GetWeapon().GetCurrentAmmo(), item.GetWeapon().GetMagazineSize());
			else
				playerUI.SetAmmoCounter(-1f, -1f);
		}

		public PlayerObject getCurrentActive()
		{
			if (currentSlotIndex != -1)
				return inventory[currentSlotIndex].GetItem();

			return null;
		}

		public int GetActiveIndex()
		{
			return currentSlotIndex;
		}

		public void SetWeaponHolder(WeaponHolderScript weaponHolderScript)
		{
			weaponHolder = weaponHolderScript;
		}

		public void Show()
		{
			for (var i = 0; i < inventorySize; i++)
			{
				var item = inventory[i].GetItem();
				Debug.Log("Slot[" + i + "] : " + item);
			}
		}
	}
}
