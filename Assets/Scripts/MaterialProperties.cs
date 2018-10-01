using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialProperties : MonoBehaviour
{
    public float meltingPoint;
    public float meltingPointKnowledge;

    public Color normalColor = Color.black;

    public float baseStrikePoints;

    [System.Serializable]
    public struct TemperatureState
    {
        public float degrees;
        public float cooldownSpeed;

        public float optimalStrikingDegree;

        public float worstStressPointModifier;
        public float bestStressPointModifier;

        public float worstStrikePointModifier;
        public float bestStrikePointModifier;

        public Color color;

        public bool causeHitEffects;
    }
    public List<TemperatureState> states;

    public float strikeKnowledge;

    public float maxStress;
    public float stressKnowledge;

    public float TotalKnowlegeFraction()
    {
        return (meltingPointKnowledge + strikeKnowledge) / 2;
    }
}