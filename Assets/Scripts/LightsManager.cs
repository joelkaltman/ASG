using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using RenderSettings = UnityEngine.RenderSettings;

public class LightsManager : MonoBehaviour
{
    public Light directionalLight;
    
    void Start()
    {
        var shadows = PlayerPrefs.GetInt("shadows", 0) == 1;
        if (shadows) {
            this.ShadowsOn ();
        } else {
            this.ShadowsOff ();
        }
    }

    public void ShadowsOn()
    {
        directionalLight.enabled = true;
        RenderSettings.ambientLight = Color.black;
        PlayerPrefs.SetInt("shadows", 1);
    }

    public void ShadowsOff()
    {
        directionalLight.enabled = false;
        RenderSettings.ambientLight = Color.white;
        PlayerPrefs.SetInt("shadows", 0);
    }
}
