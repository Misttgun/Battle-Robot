using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleRobo
{
    public class PlayerInventorySlot
    {
        private int stack;
        private PlayerObject item;

        public PlayerInventorySlot()
        {
            stack = 0;
            item = null;
        }

        public bool IsEmpty()
        {
            return item == null;
        }

        public void Use()
        {
            if (stack > 0)
                stack--;

            if (stack == 0)
                item = null;
        }

        public void SetItem(PlayerObject obj)
        {
            item = obj;
            stack++;

            if (obj == null)
                stack = 0;
        }

        public PlayerObject GetItem()
        {
            return item;
        }

        public int GetItemId()
        {
            return (item != null) ? item.GetId() : -1;
        }

        public void Drop()
        {
            if (item)
            {
                SetItem(null);
            }
        }

        public int GetStackSize()
        {
            return stack;
        }
    }
}