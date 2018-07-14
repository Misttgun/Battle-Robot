using JetBrains.Annotations;
using UnityEngine;

namespace BattleRobo
{
    public class PlayerInventory
    {
        [SerializeField]
        private const int InventorySize = 5;

        // - the inventory is just a List of object owned by the player
        public readonly PlayerInventorySlot[] inventory;

        private readonly PlayerUIScript playerUI;

        // - cameraTransform is the origin of the raycast to loot an object
        private readonly Transform camera;

        // current active item, by default nothing is selected
        public int currentSlotIndex = -1;

        private readonly PhotonView playerView;

        private WeaponHolderScript weaponHolder;
        private ConsommableHolderScript consommableHolder;

        /// <summary>
        /// Constructor : instatiate the inventory slots
        /// </summary>
        public PlayerInventory(Transform cameraTransform, [CanBeNull] PlayerUIScript ui, PhotonView playerPhotonView, WeaponHolderScript weaponHolder, ConsommableHolderScript consommableHolder)
        {
            playerView = playerPhotonView;
            camera = cameraTransform;
            inventory = new PlayerInventorySlot[5];
            this.weaponHolder = weaponHolder;
            this.consommableHolder = consommableHolder;

            playerUI = ui;

            for (var i = 0; i < inventory.Length; i++)
                inventory[i] = new PlayerInventorySlot();
        }

        /// <summary>
        /// Check if inventory is full
        /// </summary>
        public bool IsFull()
        {
            for (var i = 0; i < InventorySize; i++)
            {
                if (inventory[i].IsEmpty())
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Check if inventory is empty
        /// </summary>
        public bool IsEmpty()
        {
            for (var i = 0; i < InventorySize; i++)
            {
                if (!inventory[i].IsEmpty())
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Find the first empty slot. Return -1 if there is no empty slot.
        /// </summary>
        public int FindFirstEmptySlot()
        {
            for (var i = 0; i < InventorySize; i++)
            {
                if (inventory[i].IsEmpty())
                    return i;
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
            for (var i = 0; i < InventorySize; i++)
            {
                if (inventory[i].GetItemId() == itemId && inventory[i].GetStackSize() < inventory[i].GetItem().GetMaxStack())
                    return i;
            }

            return FindFirstEmptySlot();
        }

        /// <summary>    
        /// Put an object in an inventory slot
        /// </summary>
        public void AddObject(PlayerObjectScript obj, int slotIndex)
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
            //TODO Trouver une solution pour le switch des armes (quand c'est trop rapide, il mets 0)
            //set the current ammo of the weapon in the inventory before the switch
            var currentActive = GetCurrentActive();

            if (currentActive && currentActive.IsWeapon() && weaponHolder.currentWeapon)
            {
                currentActive.GetWeapon().SetCurrentAmmo(weaponHolder.currentWeapon.currentAmmo);
            }
            

            SetActiveItem(index);

            var item = inventory[index].GetItem();

            if (item && item.IsWeapon())
            {
                var weapon = item.GetWeapon();
                weaponHolder.SetWeapon(weapon, weapon.currentAmmo);
                consommableHolder.SetConsommable(null);
            }

            else if (item && item.isConsommable())
            {
                var consommable = item.GetConsommable();
                consommableHolder.SetConsommable(consommable);
                weaponHolder.SetWeapon(null, 0f);
            }

            else
            {
                weaponHolder.SetWeapon(null, 0f);
                consommableHolder.SetConsommable(null);
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
        /// Collect an item. Shoot a raycast to see if the player is targeting an item, then check distance.
        /// If the player can collect the item, try to find a slot on the inventory and add it if the inventory is not full
        /// </summary>
        public void Collect()
        {
            RaycastHit hit;

            Transform objectHit = null;

            if (Physics.Raycast(camera.position, camera.forward, out hit, 5f))
            {
                objectHit = hit.transform;
            }

            if(!objectHit)
                return;

            var playerGameObject = objectHit.gameObject;

            // - hit object is not from loot layer, exit
            if (playerGameObject.layer != 11)
                return;

            // - check distance
            if (hit.distance > 5f)
                return;


            var playerObject = playerGameObject.GetComponent<PlayerObjectScript>();

            var slotIndex = FindSlot(playerObject.GetId());

            // - inventory is not full
            if (slotIndex != -1 && playerObject.IsAvailable() && !playerObject.IsLooting())
            {
                // - the object is is looting state until the TakeObject RPC disable it
                playerObject.SetLooting(true);
                playerView.RPC("TakeObject", PhotonTargets.AllViaServer, playerObject.GetLootTrackerIndex(), slotIndex, playerView.ownerId);
                AddObject(playerObject, slotIndex);
                playerUI.SetItemUISlot(playerObject, slotIndex);

                // equip weapon if the index is already selected
                if (slotIndex == currentSlotIndex)
                {
                    if (playerObject.IsWeapon())
                    {
                        var weapon = playerObject.GetWeapon();
                        weaponHolder.SetWeapon(weapon, weapon.currentAmmo);
                        playerUI.SetAmmoCounter(weapon.currentAmmo);
                    }

                    else if(playerObject.isConsommable())
                    {
                        var consommableId = playerObject.GetConsommable().GetId();
                        weaponHolder.SetWeapon(null, 0f);
                        consommableHolder.EquipConsommable(consommableId);
                    }
                }
            }
        }

        public void CancelCollect(int lootTrackerIndex)
        {
            var slotToClear = FindObjectSlot(lootTrackerIndex);

            inventory[slotToClear] = null;

            // - update UI
            playerUI.SetItemUISlot(null, currentSlotIndex);
            playerUI.SetAmmoCounter(-1f);

            // - unequip weapon if necessary
            if (slotToClear == currentSlotIndex)
                weaponHolder.SetWeapon(null, 0f);
        }

        public int FindObjectSlot(int lootTrackerIndex)
        {
            for (int i = 0; i < InventorySize; i++)
                if (inventory[i].GetLootTrackerIndex() == lootTrackerIndex)
                    return i;

            return -1;
        }

        public void Drop(Vector3 position, int index)
        {
            Debug.Log("Object is drop");
            var playerObject = inventory[index].GetItem();

            if (!playerObject)
                return;

            // - Update ammo counter
            if (playerObject.IsWeapon())
            {
                playerView.RPC("UpdateWeapon", PhotonTargets.AllViaServer, playerObject.GetLootTrackerIndex(), playerObject.GetWeapon().currentAmmo);

                weaponHolder.SetWeapon(null, 0f);
                playerUI.SetAmmoCounter(-1f);
            }
            
            // - place the weapon on the map and show it
            playerView.RPC("DropObject", PhotonTargets.AllViaServer, playerObject.GetLootTrackerIndex(), position);

            // - remove object from player inventory
            inventory[index].Drop();

            // - update UI
            playerUI.SetItemUISlot(null, index);
        }

        public void SwapInventorySlot(int slotIndexSrc, int slotIndexDest)
        {
            PlayerInventorySlot tmp = inventory[slotIndexDest];
            inventory[slotIndexDest] = inventory[slotIndexSrc];
            inventory[slotIndexSrc] = tmp;
            playerUI.SetItemUISlot(inventory[slotIndexSrc].GetItem(), slotIndexSrc);
            playerUI.SetItemUISlot(inventory[slotIndexDest].GetItem(), slotIndexDest);
            SwitchActiveIndex(slotIndexSrc);
        }

        public void SetActiveItem(int index)
        {
            currentSlotIndex = index;
            UpdateInventoryUI(index);

            var item = inventory[currentSlotIndex].GetItem();

            if (item && item.GetWeapon() != null)
                playerUI.SetAmmoCounter(item.GetWeapon().currentAmmo);
            else
                playerUI.SetAmmoCounter(-1f);
        }

        public PlayerObjectScript GetCurrentActive()
        {
            if (currentSlotIndex != -1)
                return inventory[currentSlotIndex].GetItem();

            return null;
        }

        public void UseItem(int index)
        {
            if (index < 0 || index > inventory.Length)
                return;

            var itemSlot = inventory[index];
            var item = itemSlot.GetItem();
            
            if (item.isConsommable())
            {
                itemSlot.Use();
            
                // - remove ui sprite if stack is empty
                if (itemSlot.GetStackSize() == 0)
                {
                    playerUI.SetItemUISlot(null, currentSlotIndex);
                    consommableHolder.currentConsommable = null;
                } 
            }   
        }

        public int GetActiveIndex()
        {
            return currentSlotIndex;
        }

        public void Show()
        {
            for (var i = 0; i < InventorySize; i++)
            {
                var item = inventory[i].GetItem();
                Debug.Log("Slot[" + i + "] : " + item);
            }
        }

        public void DropAll(Vector3 position)
        {
            Vector3 newPosition = position;
            for (var i = 0; i < InventorySize; i++)
            {
                if (i == 1) newPosition = position + new Vector3(-2f, 0f, 0f);
                else if (i == 2) newPosition = position + new Vector3(2f, 0f, 0f);
                else if (i == 3) newPosition = position + new Vector3(0f, 0f, -2f);
                else if (i == 4) newPosition = position + new Vector3(0f, 0f, 2f);

                if (!inventory[i].IsEmpty())
                    Drop(newPosition, i);
            }
        }
    }
}