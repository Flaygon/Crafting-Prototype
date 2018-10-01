using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dungeon : MonoBehaviour
{
    [SerializeField]
    private LootTable lootTable;

    public List<Item> GetRewards()
    {
        return lootTable.GetLoot();
    }
}