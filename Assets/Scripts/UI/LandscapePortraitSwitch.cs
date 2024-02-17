using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LandscapePortraitSwitch : MonoBehaviour
{
    public Transform landscapeCanvas;
    public Transform portraitCanvas;

    public List<GameObject> elements;

    private ScreenOrientation lastOrientation;
    
    void Update()
    {
        if (Screen.orientation == lastOrientation)
            return;

        if (Screen.orientation == ScreenOrientation.Portrait)
        {
            foreach (var child in elements)
                child.transform.SetParent(portraitCanvas);
        }
        else
        {
            foreach (var child in elements)
                child.transform.SetParent(landscapeCanvas);
        }

        lastOrientation = Screen.orientation;
    }
}
