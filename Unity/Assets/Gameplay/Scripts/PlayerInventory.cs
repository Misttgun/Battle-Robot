using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerInventory
{
	[SerializeField] 
	private int inventorySize = 5;

	// - the inventory is just a List of object owned by the player
	private PlayerInventorySlot[] inventory;
	
	// - the UI slot reference
	[SerializeField] 
	private GameObject[] inventorySlotUI;
	
	// - nearest items are the items that can be collected by clicking on F
	private List<PlayerObject> nearItems;
	// current active item
	private int currentSlotIndex = 0;

	public PlayerInventory()
	{
		inventory = new PlayerInventorySlot[5];
		nearItems = new List<PlayerObject>();

		for (var i = 0; i < inventory.Length; i++)
			inventory[i] = new PlayerInventorySlot();
	}

	public bool IsFull()
	{
		for (var i = 0; i < inventorySize; i++)
			if (inventory[i].IsEmpty())
				return false;

		return true;
	}

	public bool IsEmpty()
	{
		for (var i = 0; i < inventorySize; i++)
			if (!inventory[i].IsEmpty())
				return false;

		return true;
	}

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

		// no empty slot
		return -1;
	}

	public int FindSlot(int itemId)
	{
		for (var i = 0; i < inventorySize; i++)
			if (inventory[i].GetItemId() == itemId)
				return i;

		return FindFirstEmptySlot();

	}
	
	public void AddObject(PlayerObject obj, int slotIndex)
	{
		// can't add object if inventory is full

		if (slotIndex != -1)
			inventory[slotIndex].SetItem(obj);
	}

	public void SwitchActiveIndex(int index)
	{
		SetActiveItem(index);
	}

	public void UpdateInventoryUI(PlayerObject obj, int slotIndex)
	{
		
	}

	public void DropObject(int slotIndex)
	{
		inventory[slotIndex] = null;
	}

	public void AddNearItem(PlayerObject obj)
	{
		nearItems.Add(obj);
	}

	public void RemoveNearItem(PlayerObject obj)
	{
		nearItems.Remove(obj);
	}

	public void Collect()
	{
		if (nearItems.Count > 0)
		{
			var item = nearItems.First();
			var slotIndex = FindSlot(item.GetId());

			if (slotIndex != -1)
			{
				
				AddObject(item, slotIndex);
				nearItems.Remove(item);
				item.Hide();
			}
			
			else
				Debug.Log("INVENTORY IS FULL");
		}
	}

	public void Drop(Vector3 position)
	{
		inventory[currentSlotIndex].Drop(position);
	}

	public void SetActiveItem(int index)
	{
		currentSlotIndex = index;
	}
	
}
