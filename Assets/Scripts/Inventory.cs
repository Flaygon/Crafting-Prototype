using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public class ItemStack
    {
        public Item item;
        public int current;
    }

    public List<ItemStack> slots = new List<ItemStack>();

    public bool AddItem(Item toAdd)
    {
        ItemStack foundSlot = null;
        foreach(ItemStack iStack in slots)
        {
            if (iStack.item.name.Equals(toAdd.name))
            {
                foundSlot = iStack;
            }
        }

        if(foundSlot != null)
        {
            foundSlot.current = System.Math.Min(foundSlot.current + 1, foundSlot.item.maxStack);
            return true;
        }
        else
        {
            ItemStack newSlot = new ItemStack();
            newSlot.item = toAdd;
            newSlot.current = 1;
            slots.Add(newSlot);
        }

        return false;
    }

    public void RemoveItems(List<Recipe.ItemStack> items)
    {
        foreach(Recipe.ItemStack iItem in items)
        {
            ItemStack foundSlot = null;
            foreach (ItemStack iStack in slots)
            {
                if (iStack.item.name.Equals(iItem.item.name))
                {
                    foundSlot = iStack;
                }
            }

            if (foundSlot != null)
            {
                foundSlot.current -= iItem.num;

                if(foundSlot.current < 0)
                {
                    Debug.LogError("Removed too many items of a given stack and put it in an invalid state");
                }
            }
            else
            {
                Debug.LogWarning("Tried to remove items that are not present in inventory");
            }
        }
    }

    public bool HasItems(List<Recipe.ItemStack> items)
    {
        foreach (Recipe.ItemStack iItem in items)
        {
            bool hasItem = false;
            foreach (ItemStack iSlot in slots)
            {
                if (iSlot.item.name.Equals(iItem.item.name))
                {
                    hasItem = true;

                    if (iSlot.current < iItem.num)
                    {
                        return false;
                    }

                    break;
                }
            }

            if (!hasItem)
            {
                return false;
            }
        }

        return true;
    }
}