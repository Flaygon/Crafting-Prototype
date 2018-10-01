using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SmithingManager : MonoBehaviour
{
    public bool debugUIFlag = false;

    public GameObject debugUI;

    public Text forgingUIHeating;
    public Text forgingUIStrikeCurrent;
    public Text forgingUIStrikeNew;
    public Text forgingUIStressCurrent;
    public Text forgingUIStressNew;

    public MaterialDatabase materialDatabase;
    public RecipeDatabase recipeDatabase;

    public Inventory inventory;

    private Recipe recipe = null;
    private float currentDegrees = 0.0f;
    private int currentTemperatureState = 0;
    private float currentStressPoints = 0.0f;
    private float currentStrikePoints = 0.0f;

    public MeshRenderer smithableObjectRenderer;
    public Light smithableLightObject;

    public Transform smithableAnvilPosition;
    public Transform smithableForgeCheckingPosition;
    public Transform smithableForgeHeatingPosition;

    public float heatingTime;
    private float currentHeatingTime = 0.0f;

    private enum State
    {
        HEATING,
        FORGING,

        TRANSITIONING,
    }
    private State state = State.HEATING;
    private State fromState = State.HEATING;
    private State toState = State.HEATING;

    public float transitionTime;
    private float currentTransitionTime = 0.0f;

    public Transform cameraLookAtForge;
    public Transform cameraLookAtAnvil;

    public Transform hammerObject;
    public Transform hammerHitPosition;
    public Transform hammerChargeMinPosition;
    public Transform hammerChargeMaxPosition;
    public AudioSource hammerHitAudio;
    public ParticleSystem hammerHitParticles;
    private ParticleSystem.MainModule particleHitMainModule;

    private bool canHit = true;
    private bool hitting = false;
    public float hitTime;
    private float currentHitTime;

    private bool chargingHit = false;
    public float chargingHitTimer;
    private float currentNextChargingHitTimer = 0.0f;
    private float currentChargingHitTimer = 0.0f;
    public float minChargeHitSpeedModifier;
    public float maxChargeHitSpeedModifier;
    public float minChargeHitStrikeStressModifier;
    public float maxChargeHitStrikeStressModifier;
    public float minChargeHitAudioPitchModifier;
    public float maxChargeHitAudioPitchModifier;
    public float randomChargeHitAudioPitchAlteration;

    public Image chargeMeterUI;

    private void Start()
    {
        particleHitMainModule = hammerHitParticles.main;

        debugUI.SetActive(debugUIFlag);

        recipe = recipeDatabase.recipes[0];

        Camera.main.transform.rotation = Quaternion.Slerp(cameraLookAtAnvil.localRotation, cameraLookAtForge.localRotation, 1);
        smithableObjectRenderer.transform.position = Vector3.Lerp(smithableAnvilPosition.position, smithableForgeCheckingPosition.position, 1);
        smithableObjectRenderer.transform.rotation = Quaternion.Slerp(smithableAnvilPosition.rotation, smithableForgeCheckingPosition.rotation, 1);
    }

    public void Update()
    {
        switch(state)
        {
            case State.HEATING:
                Update_Heating();
                break;
            case State.FORGING:
                Update_Forging();
                break;
            case State.TRANSITIONING:
                Update_Transitioning();
                break;
        }
    }

    private void Update_Heating()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            SwitchState(State.FORGING);
            return;
        }

        if(Input.GetMouseButton(0))
        {
            currentHeatingTime = Mathf.Min(heatingTime, currentHeatingTime + Time.deltaTime);
        }
        else
        {
            currentHeatingTime = Mathf.Max(0.0f, currentHeatingTime - Time.deltaTime);
        }

        smithableObjectRenderer.transform.position = Vector3.Lerp(smithableForgeCheckingPosition.position, smithableForgeHeatingPosition.position, currentHeatingTime / heatingTime);
        smithableObjectRenderer.transform.rotation = Quaternion.Slerp(smithableForgeCheckingPosition.rotation, smithableForgeHeatingPosition.rotation, currentHeatingTime / heatingTime);

        if (currentHeatingTime >= heatingTime)
        {
            currentDegrees += 200.0f * Time.deltaTime;

            if (currentDegrees >= recipe.required[0].item.material.states[currentTemperatureState].degrees)
            {
                currentDegrees -= recipe.required[0].item.material.states[currentTemperatureState].degrees;
                ++currentTemperatureState;

                if (recipe.required[0].item.material.states.Count <= currentTemperatureState)
                {
                    Debug.LogWarning("Your material melted :(");
                    SceneManager.LoadScene("scenes/menu");
                    return;
                }
            }
        }
        else
        {
            currentDegrees = currentDegrees - recipe.required[0].item.material.states[currentTemperatureState].cooldownSpeed * Time.deltaTime;

            if (currentTemperatureState > 0 && currentDegrees < 0.0f)
            {
                --currentTemperatureState;
                currentDegrees += recipe.required[0].item.material.states[currentTemperatureState].degrees;
            }

            currentDegrees = Mathf.Max(0.0f, currentDegrees);
        }

        Color previousStateColor = recipe.required[0].item.material.normalColor;
        if (currentTemperatureState > 0)
            previousStateColor = recipe.required[0].item.material.states[currentTemperatureState - 1].color;
        Color currentColor = Color.Lerp(previousStateColor, recipe.required[0].item.material.states[currentTemperatureState].color, currentDegrees / recipe.required[0].item.material.states[currentTemperatureState].degrees);
        smithableObjectRenderer.sharedMaterial.SetColor("_EmissionColor", currentColor);
        smithableLightObject.color = currentColor;

        if(debugUIFlag)
        {
            CollectDebugUIStats();
        }
    }

    private void Update_Forging()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            SwitchState(State.HEATING);
            return;
        }

        if (Input.GetKey(KeyCode.Escape))
        {
            for (int iStrikePointStates = recipe.strikePointStates.Count - 1; iStrikePointStates >= 0; --iStrikePointStates)
            {
                if (currentStrikePoints >= recipe.strikePointStates[iStrikePointStates].strikePointsRequired)
                {
                    List<Item> producedItems = recipe.strikePointStates[iStrikePointStates].producesLoot.GetLoot();
                    foreach (Item iItem in producedItems)
                    {
                        inventory.AddItem(iItem);
                    }
                    break;
                }
            }
            SceneManager.LoadScene("scenes/menu");
        }

        currentDegrees = currentDegrees - recipe.required[0].item.material.states[currentTemperatureState].cooldownSpeed * Time.deltaTime;

        if (currentTemperatureState > 0 && currentDegrees < 0.0f)
        {
            --currentTemperatureState;
            currentDegrees += recipe.required[0].item.material.states[currentTemperatureState].degrees;
        }

        currentDegrees = Mathf.Max(0.0f, currentDegrees);

        currentHitTime += Time.deltaTime;

        if (!canHit)
        {
            if(hitting)
            {
                Vector3 beginPosition = Vector3.Lerp(hammerChargeMinPosition.position, hammerChargeMaxPosition.position, Mathf.Lerp(0.0f, 1.0f, currentChargingHitTimer / chargingHitTimer));
                Quaternion beginRotation = Quaternion.Slerp(hammerChargeMinPosition.rotation, hammerChargeMaxPosition.rotation, Mathf.Lerp(0.0f, 1.0f, currentChargingHitTimer / chargingHitTimer));
                hammerObject.position = Vector3.Lerp(beginPosition, hammerHitPosition.position, (currentHitTime / hitTime) * Mathf.Lerp(minChargeHitSpeedModifier, maxChargeHitSpeedModifier, currentChargingHitTimer / chargingHitTimer));
                hammerObject.rotation = Quaternion.Slerp(beginRotation, hammerHitPosition.rotation, (currentHitTime / hitTime) * Mathf.Lerp(minChargeHitSpeedModifier, maxChargeHitSpeedModifier, currentChargingHitTimer / chargingHitTimer));

                if(currentHitTime >= hitTime / Mathf.Lerp(minChargeHitSpeedModifier, maxChargeHitSpeedModifier, currentChargingHitTimer / chargingHitTimer))
                {
                    currentHitTime = 0.0f;

                    hammerHitAudio.Stop();
                    hammerHitAudio.pitch = Mathf.Lerp(minChargeHitAudioPitchModifier, maxChargeHitAudioPitchModifier, currentChargingHitTimer / chargingHitTimer) + Random.Range(-randomChargeHitAudioPitchAlteration, randomChargeHitAudioPitchAlteration);
                    hammerHitAudio.Play();

                    if(recipe.required[0].item.material.states[currentTemperatureState].causeHitEffects)
                    {
                        hammerHitParticles.Stop();
                        hammerHitParticles.Play();
                    }

                    float distanceToOptimal = currentDegrees / recipe.required[0].item.material.states[currentTemperatureState].degrees;
                    currentStrikePoints = Mathf.Max(0.0f, currentStrikePoints + (recipe.required[0].item.material.baseStrikePoints * Mathf.Lerp(recipe.required[0].item.material.states[currentTemperatureState].worstStrikePointModifier, recipe.required[0].item.material.states[currentTemperatureState].bestStrikePointModifier, distanceToOptimal) * Mathf.Lerp(minChargeHitStrikeStressModifier, maxChargeHitStrikeStressModifier, currentChargingHitTimer / chargingHitTimer)));
                    currentStressPoints = Mathf.Max(0.0f, currentStressPoints + (Mathf.Lerp(recipe.required[0].item.material.states[currentTemperatureState].worstStressPointModifier, recipe.required[0].item.material.states[currentTemperatureState].bestStressPointModifier, (1 - distanceToOptimal) * Mathf.Lerp(minChargeHitStrikeStressModifier, maxChargeHitStrikeStressModifier, currentChargingHitTimer / chargingHitTimer))));

                    if (currentStressPoints >= recipe.required[0].item.material.maxStress)
                    {
                        Debug.LogWarning("Too much stress has ruptured your item and rendered it useless");
                        SceneManager.LoadScene("scenes/menu");
                        return;
                    }

                    bool bestQuality = true;
                    for (int iStrikePointStates = recipe.strikePointStates.Count - 1; iStrikePointStates >= 0; --iStrikePointStates)
                    {
                        if (currentStrikePoints < recipe.strikePointStates[iStrikePointStates].strikePointsRequired)
                        {
                            bestQuality = false;
                            break;
                        }
                    }
                    if (bestQuality)
                    {
                        Debug.LogWarning("You crafted the best item the recipe possibly could! Congratulations!");
                        List<Item> producedItems = recipe.strikePointStates[recipe.strikePointStates.Count - 1].producesLoot.GetLoot();
                        foreach (Item iItem in producedItems)
                        {
                            inventory.AddItem(iItem);
                        }
                        SceneManager.LoadScene("scenes/menu");
                        return;
                    }

                    hitting = false;
                }
            }
            else
            {
                Vector3 nextPosition = Vector3.Lerp(hammerChargeMinPosition.position, hammerChargeMaxPosition.position, Mathf.Lerp(0.0f, 1.0f, currentNextChargingHitTimer / chargingHitTimer));
                Quaternion nextRotation = Quaternion.Slerp(hammerChargeMinPosition.rotation, hammerChargeMaxPosition.rotation, Mathf.Lerp(0.0f, 1.0f, currentNextChargingHitTimer / chargingHitTimer));
                hammerObject.position = Vector3.Lerp(hammerHitPosition.position, nextPosition, (currentHitTime / hitTime) * Mathf.Lerp(minChargeHitSpeedModifier, maxChargeHitSpeedModifier, currentChargingHitTimer / chargingHitTimer));
                hammerObject.rotation = Quaternion.Slerp(hammerHitPosition.rotation, nextRotation, (currentHitTime / hitTime) * Mathf.Lerp(minChargeHitSpeedModifier, maxChargeHitSpeedModifier, currentChargingHitTimer / chargingHitTimer));

                if(currentHitTime >= hitTime / Mathf.Lerp(minChargeHitSpeedModifier, maxChargeHitSpeedModifier, currentChargingHitTimer / chargingHitTimer))
                {
                    canHit = true;
                    currentChargingHitTimer = 0.0f;
                    Vector2 anchorMax = chargeMeterUI.rectTransform.anchorMax;
                    anchorMax.x = 0.0f;
                    chargeMeterUI.rectTransform.anchorMax = anchorMax;
                }
            }
        }

        if (Input.GetMouseButton(0))
        {
            chargingHit = true;

            currentNextChargingHitTimer += Time.deltaTime;
            Vector2 anchorMax = chargeMeterUI.rectTransform.anchorMax;
            anchorMax.x = Mathf.Lerp(0.0f, 1.0f, currentNextChargingHitTimer / chargingHitTimer);
            chargeMeterUI.rectTransform.anchorMax = anchorMax;
        }
        else if(canHit)
        {
            if(chargingHit)
            {
                hitting = true;
                canHit = false;
                currentHitTime = 0.0f;

                currentChargingHitTimer = currentNextChargingHitTimer;
                currentNextChargingHitTimer = 0.0f;
            }

            chargingHit = false;
        }
            
        if(canHit && chargingHit)
        {
            hammerObject.position = Vector3.Lerp(hammerChargeMinPosition.position, hammerChargeMaxPosition.position, Mathf.Lerp(0.0f, 1.0f, currentNextChargingHitTimer / chargingHitTimer));
            hammerObject.rotation = Quaternion.Slerp(hammerChargeMinPosition.rotation, hammerChargeMaxPosition.rotation, Mathf.Lerp(0.0f, 1.0f, currentNextChargingHitTimer / chargingHitTimer));
        }

        Color previousStateColor = recipe.required[0].item.material.normalColor;
        if (currentTemperatureState > 0)
            previousStateColor = recipe.required[0].item.material.states[currentTemperatureState - 1].color;
        Color currentColor = Color.Lerp(previousStateColor, recipe.required[0].item.material.states[currentTemperatureState].color, currentDegrees / recipe.required[0].item.material.states[currentTemperatureState].degrees);
        smithableObjectRenderer.sharedMaterial.SetColor("_EmissionColor", currentColor);
        smithableLightObject.color = currentColor;

        ParticleSystem.MinMaxGradient color = particleHitMainModule.startColor;
        color.color = currentColor;
        particleHitMainModule.startColor = color;

        float effectStrength = 0.0f;
        for (int iStrikePointStates = 0; iStrikePointStates < recipe.strikePointStates.Count; ++iStrikePointStates)
        {
            if (currentStrikePoints < recipe.strikePointStates[iStrikePointStates].strikePointsRequired)
            {
                effectStrength = recipe.strikePointStates[iStrikePointStates].shakeEffectStrength;
                break;
            }
        }
        smithableObjectRenderer.transform.rotation = Quaternion.identity;
        smithableObjectRenderer.transform.Rotate(0.0f, 0.0f, Random.Range(-effectStrength, effectStrength));

        if (debugUIFlag)
        {
            CollectDebugUIStats();
        }
    }

    private void Update_Transitioning()
    {
        currentTransitionTime += Time.deltaTime;
        float transitionFraction = currentTransitionTime / transitionTime;

        if(toState == State.HEATING)
        {
            Camera.main.transform.rotation = Quaternion.Slerp(cameraLookAtAnvil.localRotation, cameraLookAtForge.localRotation, transitionFraction);
            smithableObjectRenderer.transform.position = Vector3.Lerp(smithableAnvilPosition.position, smithableForgeCheckingPosition.position, transitionFraction);
            smithableObjectRenderer.transform.rotation = Quaternion.Slerp(smithableAnvilPosition.rotation, smithableForgeCheckingPosition.rotation, transitionFraction);
        }
        else if(toState == State.FORGING)
        {
            Camera.main.transform.rotation = Quaternion.Slerp(cameraLookAtForge.localRotation, cameraLookAtAnvil.localRotation, transitionFraction);
            smithableObjectRenderer.transform.position = Vector3.Lerp(smithableForgeCheckingPosition.position, smithableAnvilPosition.position, transitionFraction);
            smithableObjectRenderer.transform.rotation = Quaternion.Slerp(smithableForgeCheckingPosition.rotation, smithableAnvilPosition.rotation, transitionFraction);
        }

        if(currentTransitionTime >= transitionTime)
        {
            currentTransitionTime = 0.0f;
            state = toState;
        }
    }

    private void SwitchState(State newState)
    {
        fromState = state;
        toState = newState;
        state = State.TRANSITIONING;
    }

    private void CollectDebugUIStats()
    {
        float collectedDegrees = currentDegrees;
        for (int iTemperatureState = 0; iTemperatureState < currentTemperatureState; ++iTemperatureState)
            collectedDegrees += recipe.required[0].item.material.states[iTemperatureState].degrees;
        forgingUIHeating.text = collectedDegrees.ToString();

        forgingUIStrikeCurrent.text = currentStrikePoints.ToString();
        forgingUIStressCurrent.text = currentStressPoints.ToString();
    }

    /*public void OnGUI()
    {
        if(state == State.MENU)
        {
            int addedFields = 0;
            for (int iRecipe = 0; iRecipe < recipeDatabase.recipes.Count; ++iRecipe)
            {
                int countedFields = 0;
                bool craftable = false;
                if (inventory.HasItems(recipeDatabase.recipes[iRecipe].required))
                {
                    craftable = true;
                    GUI.backgroundColor = Color.green;
                }
                else
                {
                    GUI.backgroundColor = Color.red;
                }

                if (GUI.Button(new Rect(Screen.width * 0.05f, Screen.height * 0.05f * addedFields + Screen.height * 0.2f + Screen.height * 0.05f * iRecipe, Screen.width * 0.4f, Screen.height * 0.05f), recipeDatabase.recipes[iRecipe].strikePointStates[0].producesLoot.name.ToString()) && craftable)
                {
                    inventory.RemoveItems(recipeDatabase.recipes[iRecipe].required);
                    //crystalsPouch.RemoveCrystal(recipes[iRecipe].crystal.crystal, recipes[iRecipe].crystal.num);
                    recipe = recipeDatabase.recipes[iRecipe];
                    SwitchState(recipe.craftingType);
                }
                ++countedFields;

                //GUI.Box(new Rect(Screen.width * 0.1f, Screen.height * 0.05f * addedFields + Screen.height * 0.05f * countedFields + Screen.height * 0.2f + Screen.height * 0.05f * iRecipe, Screen.width * 0.3f, Screen.height * 0.05f), recipes[iRecipe].crystal.crystal.ToString());
                //GUI.Box(new Rect(Screen.width * 0.4f, Screen.height * 0.05f * addedFields + Screen.height * 0.05f * countedFields + Screen.height * 0.2f + Screen.height * 0.05f * iRecipe, Screen.width * 0.05f, Screen.height * 0.05f), recipes[iRecipe].crystal.num.ToString());
                //++countedFields;

                for (int iItem = 0; iItem < recipeDatabase.recipes[iRecipe].required.Count; ++iItem)
                {
                    GUI.Box(new Rect(Screen.width * 0.15f, Screen.height * 0.05f * addedFields + Screen.height * 0.05f * countedFields + Screen.height * 0.2f + Screen.height * 0.05f * iRecipe, Screen.width * 0.25f, Screen.height * 0.05f), recipeDatabase.recipes[iRecipe].required[iItem].item.name.ToString());
                    GUI.Box(new Rect(Screen.width * 0.4f, Screen.height * 0.05f * addedFields + Screen.height * 0.05f * countedFields + Screen.height * 0.2f + Screen.height * 0.05f * iRecipe, Screen.width * 0.05f, Screen.height * 0.05f), recipeDatabase.recipes[iRecipe].required[iItem].num.ToString());

                    ++countedFields;
                }

                GUI.backgroundColor = Color.black;

                addedFields += countedFields;
            }

            for (int iSlot = 0; iSlot < inventory.slots.Count; ++iSlot)
            {
                GUI.Box(new Rect(Screen.width * 0.6f, Screen.height * 0.2f + Screen.height * 0.05f * iSlot, Screen.width * 0.2f, Screen.height * 0.05f), inventory.slots[iSlot].item.name);
                GUI.Box(new Rect(Screen.width * 0.81f, Screen.height * 0.2f + Screen.height * 0.05f * iSlot, Screen.width * 0.1f, Screen.height * 0.05f), inventory.slots[iSlot].current.ToString() + "/" + inventory.slots[iSlot].item.maxStack.ToString());
            }
        }
    }*/
}