using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LootTable : MonoBehaviour
{
    [System.Serializable]
    private struct Loot
    {
        public Item item;
        public LootTable table;
        [Range(0.0001f, 100.0f)]
        public float percentile;
    }

    [SerializeField]
    private List<Loot> loot;

    public List<Item> GetLoot()
    {
        List<Item> randomLoot = new List<Item>();

        foreach (Loot iLoot in loot)
        {
            if (Random.Range(0.0f, 100.0f) < iLoot.percentile)
            {
                randomLoot.Add(iLoot.item);
                if(iLoot.table)
                {
                    List<Item> additionalLoot = iLoot.table.GetLoot();
                    randomLoot.AddRange(additionalLoot);
                }
            }
        }

        return randomLoot;
    }
}