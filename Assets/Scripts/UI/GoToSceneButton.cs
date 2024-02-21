using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GoToSceneButton : MonoBehaviour
{
    public string sceneName;
    
    void Start()
    {
        this.GetComponent<Button>().onClick.AddListener(GoToScene);
    }

    void GoToScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}
