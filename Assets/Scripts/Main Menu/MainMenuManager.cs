using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    private string sceneToLoad = "";

    private bool fading = false;
    public MeshRenderer fadeObjectRenderer;
    public float fadeTime;
    private float currentFadeTime = 0.0f;

    public Button smithingButton;
    public Button miningButton;
    public Button knowledgeButton;
    public Button recipesButton;

    public AudioSource buttonAudioObject;

    public AudioSource fireAudioObject;
    public AudioSource backgroundAudioObject;

    private void Start()
    {
        SwitchButtonsOnOff(false);
    }

    public void LoadSmithing()
    {
        fading = true;
        currentFadeTime = 0.0f;
        sceneToLoad = "scenes/smithing";
        SwitchButtonsOnOff(false);
        buttonAudioObject.Play();
    }

    public void LoadMining()
    {
        fading = true;
        currentFadeTime = 0.0f;
        sceneToLoad = "scenes/mining";
        SwitchButtonsOnOff(false);
        buttonAudioObject.Play();
    }

    public void LoadKnowledge()
    {
        fading = true;
        currentFadeTime = 0.0f;
        sceneToLoad = "scenes/knowledge";
        SwitchButtonsOnOff(false);
        buttonAudioObject.Play();
    }

    public void LoadRecipes()
    {
        fading = true;
        currentFadeTime = 0.0f;
        sceneToLoad = "scenes/recipes";
        SwitchButtonsOnOff(false);
        buttonAudioObject.Play();
    }

    private void Update()
    {
        if(fading)
        {
            currentFadeTime += Time.deltaTime;
            float fadeFractal = Mathf.Lerp(0.0f, 1.0f, currentFadeTime / fadeTime);

            Color fadingColor = fadeObjectRenderer.sharedMaterial.color;
            fadingColor.a = fadeFractal;
            fadeObjectRenderer.sharedMaterial.color = fadingColor;

            fireAudioObject.volume = 1 - fadeFractal;
            backgroundAudioObject.volume = 1 - fadeFractal;

            if (currentFadeTime >= fadeTime)
            {
                SceneManager.LoadScene(sceneToLoad);
            }
        }
        else
        {
            currentFadeTime += Time.deltaTime;
            float fadeFractal = Mathf.Lerp(1.0f, 0.0f, currentFadeTime / fadeTime);

            Color fadingColor = fadeObjectRenderer.sharedMaterial.color;
            fadingColor.a = fadeFractal;
            fadeObjectRenderer.sharedMaterial.color = fadingColor;

            fireAudioObject.volume = 1 - fadeFractal;
            backgroundAudioObject.volume = 1 - fadeFractal;

            if (currentFadeTime >= fadeTime)
            {
                SwitchButtonsOnOff(true);
            }
        }
    }

    private void SwitchButtonsOnOff(bool switchFlag)
    {
        smithingButton.interactable = switchFlag;
        miningButton.interactable = switchFlag;
        knowledgeButton.interactable = switchFlag;
        recipesButton.interactable = switchFlag;
    }
}