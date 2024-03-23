using TMPro;
using UnityEngine;

public class OptionsUI : MonoBehaviour 
{
    public TMP_Dropdown qualityDropdown;
    public TMP_Dropdown shadowsDropdown;

    private void Awake()
	{
		qualityDropdown.onValueChanged.AddListener(OnChangeQuality);
		shadowsDropdown.onValueChanged.AddListener(OnChangeShadows);
	}

	private void Start()
	{
		int indexShadows = LightsManager.Instance.Shadows ? 1 : 0;
		shadowsDropdown.SetValueWithoutNotify(indexShadows);
		qualityDropdown.SetValueWithoutNotify(QualitySettings.GetQualityLevel());
	}

	private void OnChangeQuality(int index)
	{
		QualitySettings.SetQualityLevel (index, false);
	}
	
	private void OnChangeShadows(int index)
	{
		if (index == 0)
		{
			LightsManager.Instance.ShadowsOff();
		}
		else
		{
			LightsManager.Instance.ShadowsOn();
		}
	}
}