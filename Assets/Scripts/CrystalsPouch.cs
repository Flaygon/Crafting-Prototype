using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalsPouch : MonoBehaviour
{
    public struct CrystalSlot
    {
        public Crystal.Type type;
        public List<Crystal> crystals;
    }

    public Dictionary<Crystal.Type, CrystalSlot> slots = new Dictionary<Crystal.Type, CrystalSlot>();

    public CrystalsPouch()
    {
        for(int iType = 0; iType < (int)Crystal.Type.COUNT; ++iType)
        {
            slots[(Crystal.Type)iType] = new CrystalSlot() { type = (Crystal.Type)iType, crystals = new List<Crystal>() };
        }
    }

    public void AddCrystal(Crystal toAdd)
    {
        slots[toAdd.type].crystals.Add(toAdd);
    }

    public void AddCrystal(Crystal.Type toAdd, int num)
    {
        for(int iCrystal = 0; iCrystal < num; ++iCrystal)
        {
            slots[toAdd].crystals.Add(new Crystal() { type = toAdd });
        }
    }

    public void RemoveCrystal(Crystal.Type toAdd, int num)
    {
        slots[toAdd].crystals.RemoveRange(0, num);
    }

    public bool HasCrystals(Crystal.Type type, int num)
    {
        return slots[type].crystals.Count >= num;
    }
}