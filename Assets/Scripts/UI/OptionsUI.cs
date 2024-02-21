using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class OptionsUI : MonoBehaviour {

	public List<Button> qualityButtons;
    public Button shadowsOnButton;
    public Button shadowsOffButton;

	void Awake(){
		for (int i = 0; i < qualityButtons.Count; i++) {
			qualityButtons [i].onClick.AddListener (ChangeQuality);
		}
        
        shadowsOnButton.onClick.AddListener(TurnShadowsOn);   
        shadowsOffButton.onClick.AddListener(TurnShadowsOff);   
	}
		
	void ChangeQuality()
	{
		GameObject selected = EventSystem.current.currentSelectedGameObject;

		int quality = 1;
		for (int i = 0; i < qualityButtons.Count; i++) {
			if (qualityButtons[i].name == selected.name) {
				ChangePressedQualityButton (i);
				quality = i;
			}
		}

		QualitySettings.SetQualityLevel (quality, false);
	}

	void ChangePressedQualityButton(int index)
	{
		for (int i = 0; i < qualityButtons.Count; i++) {
			ColorBlock cb = qualityButtons [i].colors;
			if (i == index) {
				cb.normalColor = Color.gray;
			} else {
				cb.normalColor = Color.white;
			}
			qualityButtons [i].colors = cb;
		}
	}

    private void TurnShadowsOn()
    {
        LightsManager.Instance.ShadowsOn();
    }
    
    private void TurnShadowsOff()
    {
        LightsManager.Instance.ShadowsOff();
    }
}