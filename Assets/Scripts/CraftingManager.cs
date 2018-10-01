using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingManager : MonoBehaviour
{
    [SerializeField]
    private Dungeon currentDungeon;

    [SerializeField]
    private List<Recipe> recipes;

    [SerializeField]
    private Inventory inventory;

    [SerializeField]
    private CrystalsPouch crystalsPouch;

    private bool crafting = false;

    private void Start()
    {
        for(int iFull = 0; iFull < 500; ++iFull)
        {
            List<Item> rewards = currentDungeon.GetRewards();
            foreach (Item iReward in rewards)
            {
                inventory.AddItem(iReward);
            }

            for (int iSlot = 0; iSlot < (int)Crystal.Type.COUNT; ++iSlot)
            {
                crystalsPouch.AddCrystal((Crystal.Type)iSlot, 1);
            }
        }
    }

    /*private void OnGUI()
    {
        GUI.backgroundColor = Color.black;

        GUI_TopRow();
        GUI_Actions();
        GUI_Inventory();
    }*/

    /*private void GUI_TopRow()
    {
        if (GUI.Button(new Rect(Screen.width * 0.05f, Screen.height * 0.05f, Screen.width * 0.2f, Screen.height * 0.1f), "Get Rewards"))
        {
            List<Item> rewards = currentDungeon.GetRewards();

            Debug.Log(" - - - ");
            Debug.Log("New rewards:");
            foreach (Item iReward in rewards)
            {
                Debug.Log(iReward.name);

                inventory.AddItem(iReward);
            }
        }

        if (crafting)
        {
            GUI.backgroundColor = Color.green;
        }
        if (GUI.Button(new Rect(Screen.width * 0.3f, Screen.height * 0.05f, Screen.width * 0.2f, Screen.height * 0.1f), "Crafting"))
        {
            crafting = true;
        }
        GUI.backgroundColor = Color.black;
    }

    private void GUI_Actions()
    {
        int addedFields = 0;
        for (int iRecipe = 0; iRecipe < recipes.Count; ++iRecipe)
        {
            int countedFields = 0;
            bool craftable = false;
            // check recipe item availability in inventory and set gui element background red for not and green for available
            if (inventory.HasItems(recipes[iRecipe].required) && crystalsPouch.HasCrystals(recipes[iRecipe].crystal.crystal, recipes[iRecipe].crystal.num))
            {
                craftable = true;
                GUI.backgroundColor = Color.green;
            }
            else
            {
                GUI.backgroundColor = Color.red;
            }

            if (GUI.Button(new Rect(Screen.width * 0.05f, Screen.height * 0.05f * addedFields + Screen.height * 0.2f + Screen.height * 0.05f * iRecipe, Screen.width * 0.4f, Screen.height * 0.05f), recipes[iRecipe].produces.name.ToString()) && craftable)
            {
                inventory.RemoveItems(recipes[iRecipe].required);
                crystalsPouch.RemoveCrystal(recipes[iRecipe].crystal.crystal, recipes[iRecipe].crystal.num);
                inventory.AddItem(recipes[iRecipe].produces);
            }
            ++countedFields;

            GUI.Box(new Rect(Screen.width * 0.1f, Screen.height * 0.05f * addedFields + Screen.height * 0.05f * countedFields + Screen.height * 0.2f + Screen.height * 0.05f * iRecipe, Screen.width * 0.3f, Screen.height * 0.05f), recipes[iRecipe].crystal.crystal.ToString());
            GUI.Box(new Rect(Screen.width * 0.4f, Screen.height * 0.05f * addedFields + Screen.height * 0.05f * countedFields + Screen.height * 0.2f + Screen.height * 0.05f * iRecipe, Screen.width * 0.05f, Screen.height * 0.05f), recipes[iRecipe].crystal.num.ToString());

            ++countedFields;

            for (int iItem = 0; iItem < recipes[iRecipe].required.Count; ++iItem)
            {
                GUI.Box(new Rect(Screen.width * 0.15f, Screen.height * 0.05f * addedFields + Screen.height * 0.05f * countedFields + Screen.height * 0.2f + Screen.height * 0.05f * iRecipe, Screen.width * 0.25f, Screen.height * 0.05f), recipes[iRecipe].required[iItem].item.name.ToString());
                GUI.Box(new Rect(Screen.width * 0.4f, Screen.height * 0.05f * addedFields + Screen.height * 0.05f * countedFields + Screen.height * 0.2f + Screen.height * 0.05f * iRecipe, Screen.width * 0.05f, Screen.height * 0.05f), recipes[iRecipe].required[iItem].num.ToString());

                ++countedFields;
            }

            GUI.backgroundColor = Color.black;

            addedFields += countedFields;
        }
    }

    private void GUI_Inventory()
    {
        for (int iSlot = 0; iSlot < (int)Crystal.Type.COUNT; ++iSlot)
        {
            if(GUI.Button(new Rect(Screen.width * 0.5f, Screen.height * 0.2f + Screen.height * 0.05f * iSlot, Screen.width * 0.2f, Screen.height * 0.05f), crystalsPouch.slots[(Crystal.Type)iSlot].type.ToString()))
            {
                crystalsPouch.AddCrystal((Crystal.Type)iSlot, 1);
            }
            GUI.Box(new Rect(Screen.width * 0.7f, Screen.height * 0.2f + Screen.height * 0.05f * iSlot, Screen.width * 0.05f, Screen.height * 0.05f), crystalsPouch.slots[(Crystal.Type)iSlot].crystals.Count.ToString());
        }
        for (int iSlot = 0; iSlot < inventory.slots.Count; ++iSlot)
        {
            GUI.Box(new Rect(Screen.width * 0.8f, Screen.height * 0.2f + Screen.height * 0.05f * iSlot, Screen.width * 0.1f, Screen.height * 0.05f), inventory.slots[iSlot].item.name);
            GUI.Box(new Rect(Screen.width * 0.9f, Screen.height * 0.2f + Screen.height * 0.05f * iSlot, Screen.width * 0.1f, Screen.height * 0.05f), inventory.slots[iSlot].current.ToString() + "/" + inventory.slots[iSlot].item.maxStack.ToString());
        }
    }*/
}