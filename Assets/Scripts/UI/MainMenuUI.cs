using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class MainMenuUI : MonoBehaviour {

	enum PanelType
	{
		MAINMENU,
		PAUSEMENU,
		GAME,
		GAMEOVER,
		OPTIONS,
		RANKING
	};

	public GameObject panelMainMenu;
	public GameObject panelOptions;
	public GameObject panelRanking;

	public Button buttonPlay;
	public Text textUsername;
	public Text textRandomTextMenu;
	public List<Text> textCapCount;
	public List<Image> buttonsSound;
	public Sprite soundOn;
	public Sprite soundOff;
	public List<Button> qualityButtons;
	
	[Header("Ranking")] 
	public RankingUI rankingUi;
	
	private float objetiveFade;
	private float currentFade;
	private float speedFade;
	private bool usedContinue;

	private PanelType currentPanel;
	private PanelType lastPanel;

	private float elapsedTime;
	private int remainSeconds;
	private int remainMinutes;

	void Awake(){
		for (int i = 0; i < qualityButtons.Count; i++) {
			qualityButtons [i].onClick.AddListener (ChangeQuality);
		}

		objetiveFade = 0;
		currentFade = 0;
		speedFade = 0.01f;
		usedContinue = false;
		currentPanel = 0;
		lastPanel = 0;
		elapsedTime = 0;
		remainSeconds = 0;
		remainMinutes = 0;
	}

	// Use this for initialization
	async void Start () {
        
        if(!UserManager.Instance().Initialized)
            await DefaultUserFallback();
        
        ShowCanvas (PanelType.MAINMENU);
        
		int currentQuality = QualitySettings.GetQualityLevel ();
		if (currentQuality >= qualityButtons.Count) {
			currentQuality = qualityButtons.Count - 1;
		}
		ChangePressedQualityButton (currentQuality);
        
        OnUserInitialized();
    }

    private async Task DefaultUserFallback()
    {
        await AuthManager.Initialize();
        await AuthManager.Login("joelkalt@asg.com", "asdasd");
        await UserManager.Instance().Initialize();
    }
    
    private void OnUserInitialized()
    {
        textUsername.text = UserManager.Instance().UserData.username;
        RefreshCaps();
        buttonPlay.interactable = true;
    }
    
    private void RefreshCaps()
    {
        for (int i = 0; i < textCapCount.Count; i++) {
            if(textCapCount[i] == null)
                continue;
			
            textCapCount[i].text = "x" + UserManager.Instance().UserData.caps;
            BounceText bounce = textCapCount[i].GetComponent<BounceText> ();
            if (bounce != null) {
                bounce.Bounce (0.65f);
            }
        }
    }
    
	private void ShowCanvas(PanelType type)
	{
		switch (type) {
		case PanelType.MAINMENU:
			panelMainMenu.SetActive (true);
			panelOptions.SetActive (false);
			panelRanking.SetActive (false);
			ChangeRandomText ();
			break;
		case PanelType.PAUSEMENU:
			panelMainMenu.SetActive (false);
			panelOptions.SetActive (false);
			panelRanking.SetActive (false);
			break;
		case PanelType.GAME:
			panelMainMenu.SetActive (false);
			panelOptions.SetActive (false);
			panelRanking.SetActive (false);
			break;
		case PanelType.GAMEOVER:
			panelMainMenu.SetActive (false);
			panelOptions.SetActive (false);
			panelRanking.SetActive (false);
			break;
		case PanelType.OPTIONS:
			panelMainMenu.SetActive (false);
			panelOptions.SetActive (true);
			panelRanking.SetActive (false);
			break;
		case PanelType.RANKING:
			panelMainMenu.SetActive (false);
			panelOptions.SetActive (false);
			panelRanking.SetActive (true);
			break;
		}

		lastPanel = currentPanel;
		currentPanel = type;
	}

	void ChangeRandomText()
    {
        var texts = new List<string>()
        {
            "Time to kill some madafakin monsters!",
            "Buckle up cowboy, you're up for a ride",
            "Prepare to kill. A lot.",
            "Hakuna Matata!",
            "Yippee-ki-yay madafaka!",
            "Are you afraid of monsters? I am.",
            "Oh... the horror... THE HORROR",
            "Check out the cool guns at the store",
            "Dinopianito se come la frutita",
            "This is a test... checking 1, 2, 3...",
            "Dude, this game is so stup--- GREAT",
            "I'm running out of ideas over here",
            "This game was made with love (and coffe)",
            "Cocaine is one hell of a drug",
            "Titanic is overrated. There, i said it.",
            "if (noMoreIdeas) { text = this; }",
            "Thank god this game is free",
            "Stay tuned for next updates!",
            "Greatings from Argentina",
            "zzzzzzzzzzzzzzzzzzzzzzzzzzzzzzz",
            "Random line n°31415"
        };

		int rnd = Random.Range (0, texts.Count);
		textRandomTextMenu.text = texts [rnd];
	}

	public void StartGame()
	{
        SceneManager.LoadScene("Game", LoadSceneMode.Single);
	}

	public void ExitGame()
	{
		Application.Quit ();
	}
	
	public void LogOut()
	{
		AuthManager.Logout();
		SceneManager.LoadScene ("Login");
	}
		
	public void GoToMainMenu()
	{
		Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
	}

	public void GoToStore()
	{
		SceneManager.LoadScene ("Store");
	}

	public void GoToOptions()
	{
        ShowCanvas (PanelType.OPTIONS);
	}

	public void GoToRanking()
	{
        ShowCanvas (PanelType.RANKING);
		rankingUi.LoadRanking();
	}

	public void GoToLastPanel()
	{
        ShowCanvas (lastPanel);
	}

	public void MuteGame(){
		bool isMute = MusicManager.Instance.Mute ();
		for (int i = 0; i < buttonsSound.Count; i++) {
			if (isMute) {
				buttonsSound [i].sprite = soundOff;
			} else {
				buttonsSound [i].sprite = soundOn;
			}
		}
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
}