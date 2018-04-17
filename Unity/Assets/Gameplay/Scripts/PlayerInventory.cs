using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BattleRobo;
using UnityEngine;

/// <summary>
/// A class which represent the player inventory
/// </summary>
public class PlayerInventory
{
	[SerializeField] 
	private int inventorySize = 5;

	// - the inventory is just a List of object owned by the player
	private PlayerInventorySlot[] inventory;

	private PlayerUIScript playerUI;
	
	// - camera is the origin of the raycast to loot an object
	private Camera camera;
	
	// current active item
	private int currentSlotIndex = 0;

	/// <summary>
	/// Constructor : instatiate the inventory slots
	/// </summary>
	public PlayerInventory(GameObject camera, PlayerUIScript ui)
	{
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
		{
			if (inventory[i].IsEmpty())
			{
				Debug.Log(inventory[i]);
				return i;
			}
		}

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
			if (inventory[i].GetItemId() == itemId)
				return i;

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
	}

	/// <summary>
	/// Update the invetory UI
	/// </summary>
	public void UpdateInventoryUI(int slotIndex)
	{
		playerUI.SetActiveUISlot(slotIndex);
	}

	/// <summary>
	/// Drop object on the floor
	/// </summary>
	public void DropObject(int slotIndex)
	{
		inventory[slotIndex] = null;
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
			playerObject.Take();
			playerUI.SetItemUISlot(playerObject, slotIndex);
		}
	}

	public void Drop(Vector3 position)
	{
		inventory[currentSlotIndex].Drop(position);
	}

	public void SetActiveItem(int index)
	{
		currentSlotIndex = index;
		UpdateInventoryUI(index);
	}
	
}
