using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recipe : MonoBehaviour
{
    [System.Serializable]
    public struct ItemStack
    {
        public Item item;
        public int num;
    }

    public List<ItemStack> required;

    //public SmithingManager.State craftingType = SmithingManager.State.SMELTING;

    [System.Serializable]
    public struct StrikePointState
    {
        public float strikePointsRequired;
        public LootTable producesLoot;

        public float shakeEffectStrength;
    }
    public List<StrikePointState> strikePointStates;

    public bool unlocked = true;
}