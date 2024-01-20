using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
#if UNITY_ANDROID
using UnityEngine.Advertisements;
#endif

public class UIManager : MonoBehaviour {

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
	public GameObject panelPauseMenu;
	public GameObject panelGame;
	public GameObject panelGameOver;
	public GameObject panelOptions;
	public GameObject panelRanking;
	public GameObject panelNewRank;

	public Light directionalLight;

	public Text textRandomTextMenu;
	public List<Text> textCapCount;
	public Text textScore;
	public Text textTime;
	public Text textGiantScore;
	public Text textGunCount;
	public Text textWave;
	public Image imageLife;
	public Image imageGun;
	public Image imageFrecuency;
	public List<Image> buttonsSound;
	public Sprite soundOn;
	public Sprite soundOff;
	public GameObject joystickMovement;
	public GameObject joystickRotation;
	public List<Button> qualityButtons;
	public List<Text> rankingNames;
	public List<Text> rankingScores;
	public Text rankInputName;
	public GameObject watchAdButton;

	[Header("Gameplay Scripts")] 
	public CameraController cameraController;
	public PlayerMovement playerMovement;
	public PlayerStats playerStats;
	public PlayerGuns playerGuns;
	public EnemiesManager enemiesManager;
	public PowerUpsManager powerUpsManager;

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
		playerStats.ResetEvents();
		playerGuns.ResetEvents();
		enemiesManager.ResetEvents();
		
		playerStats.onScoreAdd += RefreshScore;
		playerStats.onCapCountChange += RefreshCaps;
		playerStats.onLifeChange += RefreshLife;
		playerStats.onDie += RefreshGameOver;
		
		playerGuns.onGunChange += RefreshGun;
		playerGuns.onGunChange += RefreshGunCount;
		
		playerGuns.onShoot += RefreshGunCount;
		playerGuns.onShoot += ResetFrecueny;
		
		enemiesManager.onWaveChange += ShowWave;
		enemiesManager.onWaveChange += RefreshWaveTime;

		for (int i = 0; i < qualityButtons.Count; i++) {
			qualityButtons [i].onClick.AddListener (ChangeQuality);
		}

		this.objetiveFade = 0;
		this.currentFade = 0;
		this.speedFade = 0.01f;
		this.usedContinue = false;
		this.currentPanel = 0;
		this.lastPanel = 0;
		this.elapsedTime = 0;
		this.remainSeconds = 0;
		this.remainMinutes = 0;
	}

	// Use this for initialization
	void Start () {
		this.showCanvas (PanelType.MAINMENU);

		joystickMovement.SetActive (GameData.Instance.isMobile);
		joystickRotation.SetActive (GameData.Instance.isMobile);
		
		playerGuns.InitializeGuns();

		this.RefreshLife ();
		this.RefreshScore ();
		this.RefreshCaps ();
		this.RefreshGun ();
		this.RefreshGunCount ();
		//this.ShowWave ();

		int currentQuality = QualitySettings.GetQualityLevel ();
		if (currentQuality >= this.qualityButtons.Count) {
			currentQuality = this.qualityButtons.Count - 1;
		}
		this.ChangePressedQualityButton (currentQuality);

		if (DataBase.Instance.loadLights ()) {
			this.ShadowsOn ();
		} else {
			this.ShadowsOff ();
		}

		#if UNITY_ANDROID
		//Advertisement.Initialize ("36ef9efc-7ed1-4509-9e07-134218ad8936", true);
		#endif
	}

	void Update () {
		float frecuency = playerGuns.GetCurrentGun ().Frecuency;
		this.imageFrecuency.fillAmount += Time.deltaTime / frecuency;

		this.FadeWave ();

		if (this.currentPanel == PanelType.GAME) {
			this.takeTime (Time.deltaTime);
		}
	}

	private void showCanvas(PanelType type)
	{
		switch (type) {
		case PanelType.MAINMENU:
			panelMainMenu.SetActive (true);
			panelPauseMenu.SetActive (false);
			panelGame.SetActive (false);
			panelGameOver.SetActive (false);
			panelOptions.SetActive (false);
			panelRanking.SetActive (false);
			this.changeRandomText ();
			break;
		case PanelType.PAUSEMENU:
			panelMainMenu.SetActive (false);
			panelPauseMenu.SetActive (true);
			panelGame.SetActive (false);
			panelGameOver.SetActive (false);
			panelOptions.SetActive (false);
			panelRanking.SetActive (false);
			break;
		case PanelType.GAME:
			panelMainMenu.SetActive (false);
			panelPauseMenu.SetActive (false);
			panelGame.SetActive (true);
			panelGameOver.SetActive (false);
			panelOptions.SetActive (false);
			panelRanking.SetActive (false);
			break;
		case PanelType.GAMEOVER:
			panelMainMenu.SetActive (false);
			panelPauseMenu.SetActive (false);
			panelGame.SetActive (false);
			panelGameOver.SetActive (true);
			panelOptions.SetActive (false);
			panelRanking.SetActive (false);
			break;
		case PanelType.OPTIONS:
			panelMainMenu.SetActive (false);
			panelPauseMenu.SetActive (false);
			panelGame.SetActive (false);
			panelGameOver.SetActive (false);
			panelOptions.SetActive (true);
			panelRanking.SetActive (false);
			break;
		case PanelType.RANKING:
			panelMainMenu.SetActive (false);
			panelPauseMenu.SetActive (false);
			panelGame.SetActive (false);
			panelGameOver.SetActive (false);
			panelOptions.SetActive (false);
			panelRanking.SetActive (true);
			break;
		}

		this.lastPanel = this.currentPanel;
		this.currentPanel = type;
	}

	void changeRandomText()
	{
		List<string> texts = new List<string> ();
		texts.Add ("Made by Joel K... awesome guy");
		texts.Add ("Time to kill some madafakin monsters!");
		texts.Add ("Buckle up cowboy, you're up for a ride");
		texts.Add ("Prepare to kill. A lot.");
		texts.Add ("Hakuna Matata!");
		texts.Add ("Yippee-ki-yay madafaka!");
		texts.Add ("Are you afraid of monsters? I am.");
		texts.Add ("Oh... the horror... THE HORROR");
		texts.Add ("Check out the cool guns at the store");
		texts.Add ("Dinopianito se come la frutita");
		texts.Add ("This is a test... checking 1, 2, 3...");
		texts.Add ("Dude, this game is so stup--- GREAT");
		texts.Add ("I'm running out of ideas over here");
		texts.Add ("This game was made with love (and coffe)");
		texts.Add ("Cocaine is one hell of a drug");
		texts.Add ("Titanic is overrated. There, i said it.");
		texts.Add ("So, your a grammar nazi? I doesnt care");
		texts.Add ("if (noMoreIdeas) { text = this; }");
		texts.Add ("Thank god this game is free");
		texts.Add ("Stay tuned for next updates!");
		texts.Add ("Greatings from Argentina");
		texts.Add ("zzzzzzzzzzzzzzzzzzzzzzzzzzzzzzz");
		texts.Add ("Big Bambu presents:");
		texts.Add ("Do you know Fernet?");
		texts.Add ("Random line n°31415");

		int rnd = Random.Range (0, texts.Count);
		textRandomTextMenu.text = texts [rnd];
	}

	public void StartGame()
	{
		// Enables all scripts
		cameraController.enabled = true;
		playerMovement.enabled = true;
		playerGuns.enabled = true;
		playerStats.enabled = true;
		enemiesManager.enabled = true;
		powerUpsManager.enabled = true;
		
		this.transform.parent.gameObject.SetActive (true);

		this.showCanvas (PanelType.GAME);

		this.elapsedTime = 0;

		int durationWaveSeconds = enemiesManager.getWaveDuration ();
		this.remainMinutes = (int)Mathf.Floor (durationWaveSeconds / 60);
		this.remainSeconds = Mathf.RoundToInt(durationWaveSeconds % 60);
	}

	public void ExitGame()
	{
		Application.Quit ();
	}

	public void PauseGame()
	{
		Time.timeScale = 0;
		this.showCanvas (PanelType.PAUSEMENU);
	}

	public void ResumeGame()
	{
		Time.timeScale = 1;
		this.showCanvas (PanelType.GAME);
	}
		
	public void GoToMainMenu()
	{
		Time.timeScale = 1;
		Scene loadedScene = SceneManager.GetActiveScene ();
		SceneManager.LoadScene (loadedScene.buildIndex);
	}

	public void GoToStore()
	{
		SceneManager.LoadScene ("Store");
	}

	public void GoToOptions()
	{
		this.showCanvas (PanelType.OPTIONS);
	}

	public void GoToRanking()
	{
		this.LoadRanking (-1);
		this.showCanvas (PanelType.RANKING);
	}

	public void GoToLastPanel()
	{
		this.showCanvas (lastPanel);
	}

	public void MuteGame(){
		bool isMute = MusicManager.Instance.Mute ();
		for (int i = 0; i < this.buttonsSound.Count; i++) {
			if (isMute) {
				this.buttonsSound [i].sprite = soundOff;
			} else {
				this.buttonsSound [i].sprite = soundOn;
			}
		}
	}
		
	void ChangeQuality()
	{
		GameObject selected = EventSystem.current.currentSelectedGameObject;

		int quality = 1;
		for (int i = 0; i < qualityButtons.Count; i++) {
			if (qualityButtons[i].name == selected.name) {
				this.ChangePressedQualityButton (i);
				quality = i;
			}
		}

		QualitySettings.SetQualityLevel (quality, false);
	}

	public void ShadowsOn()
	{
		directionalLight.enabled = true;
		RenderSettings.ambientLight = Color.black;
		DataBase.Instance.saveLights (true);
	}

	public void ShadowsOff()
	{
		directionalLight.enabled = false;
		RenderSettings.ambientLight = Color.white;
		DataBase.Instance.saveLights (false);
	}

	void ChangePressedQualityButton(int index)
	{
		for (int i = 0; i < qualityButtons.Count; i++) {
			ColorBlock cb = this.qualityButtons [i].colors;
			if (i == index) {
				cb.normalColor = Color.gray;
			} else {
				cb.normalColor = Color.white;
			}
			this.qualityButtons [i].colors = cb;
		}
	}

	public void DeleteAllData()
	{
		DataBase.Instance.deleteAllData ();
	}

	public void Continue()
	{
		#if UNITY_ANDROID
		// Watch add
		if (Advertisement.isInitialized)
		{
			var listener = new AdsListener() { OnFinished = HandleShowResult};
			Advertisement.Show("rewardedVideo", listener);
		}
		#endif

		#if UNITY_STANDALONE
		this.usedContinue = true;
		playerStats.Revive ();
		this.joystickMovement.GetComponentInChildren<Joystick> ().Reset ();
		this.joystickRotation.GetComponentInChildren<Joystick> ().Reset ();
		this.showCanvas (PanelType.GAME);
		#endif
	}

	#if UNITY_ANDROID
	private void HandleShowResult(ShowResult result)
	{
		switch (result)
		{
		case ShowResult.Finished:
			this.usedContinue = true;
			playerStats.Revive ();
			this.joystickMovement.GetComponentInChildren<Joystick> ().Reset ();
			this.joystickRotation.GetComponentInChildren<Joystick> ().Reset ();
			this.showCanvas (PanelType.GAME);
			break;
		case ShowResult.Skipped:
			break;
		case ShowResult.Failed:
			break;
		}
	}
	#endif

	// ================================= Game Interface ==============================

	public void RefreshScore()
	{
		textScore.text = playerStats.score.ToString();
		BounceText bounce = textScore.GetComponent<BounceText> ();
		if (bounce != null) {
			bounce.Bounce (0.65f);
		}
	}

	public void RefreshLife()
	{
		imageLife.fillAmount = (float)playerStats.life / 100;
	}

	public void RefreshGun()
	{
		imageGun.GetComponent<Image> ().sprite = playerGuns.GetCurrentGun().Sprite;
		//imageGun.GetComponent<Image> ().SetNativeSize ();
	}

	public void RefreshGunCount()
	{
		int count = playerGuns.GetCurrentGun ().CurrentCount;
		if (count >= 0) {
			textGunCount.text = "x" + count.ToString();
		} else {
			textGunCount.text = "x99999...";
		}
	}

	public void ResetFrecueny()
	{
		this.imageFrecuency.fillAmount = 0;
	}

	public void RefreshCaps()
	{
		for (int i = 0; i < textCapCount.Count; i++) {
			textCapCount[i].text = "x" + playerStats.caps;
			BounceText bounce = textCapCount[i].GetComponent<BounceText> ();
			if (bounce != null) {
				bounce.Bounce (0.65f);
			}
		}
	}

	public void RefreshGameOver()
	{
		this.showCanvas (PanelType.GAMEOVER);
		textGiantScore.text = "You killed " + playerStats.score + " enemies!";
		if (DataBase.Instance.isMaxScore (playerStats.score)) {
			this.panelNewRank.SetActive (true);
		} else {
			this.panelNewRank.SetActive (false);
		}

		/*if (usedContinue) {
			watchAdButton.SetActive (false);
		} else {
			watchAdButton.SetActive (true);
		}*/
	}

	public void SaveNewRank()
	{
		if (this.rankInputName.text.Length < 3) {
			return;
		}

		int result = DataBase.Instance.SaveNewMaxScore (playerStats.score, this.rankInputName.text.ToUpper());
		this.LoadRanking(result);
		this.showCanvas (PanelType.RANKING);
	}

	void LoadRanking(int newScoreLine)
	{
		List<string> names = DataBase.Instance.LoadMaxScoresNames ();
		List<int> scores = DataBase.Instance.LoadMaxScores ();
		for (int i = 0; i < 10; i++) {
			rankingNames [i].text = names [i];
			rankingScores [i].text = scores [i].ToString ();

			if (i == newScoreLine) {
				rankingNames [i].color = Color.yellow;
				rankingScores [i].color = Color.yellow;
			} else {
				rankingNames [i].color = Color.white;
				rankingScores [i].color = Color.white;
			}
		}
	}

	void ShowWave(){
		int wave = enemiesManager.wave;
		this.textWave.text = "Wave " + wave.ToString ();
		//this.textWave.enabled = true;
		this.objetiveFade = 1;

		//Color target = new Color (255, 255, 255, 255);
		//this.textWave.CrossFadeColor (target, 2, false, true);

		Invoke ("HideWave", 3);
	}

	void HideWave(){
		//this.textWave.enabled = false;

		this.objetiveFade = 0;

		//Color target = new Color (255, 255, 255, 0);
		//this.textWave.CrossFadeColor (target, 2, true, true);

		//this.objetiveFade = 0;
	}

	void FadeWave(){
		if (this.currentFade < this.objetiveFade) {
			this.currentFade += this.speedFade;
			if (this.currentFade > this.objetiveFade) {
				this.currentFade = this.objetiveFade;
			}
		}
		if (this.currentFade > this.objetiveFade) {
			this.currentFade -= this.speedFade;
			if (this.currentFade < this.objetiveFade) {
				this.currentFade = this.objetiveFade;
			}
		}
			
		Color curr = this.textWave.color;
		curr.a = (float)this.currentFade;
		this.textWave.color = curr;
	}

	void RefreshWaveTime(){
		int durationWaveSeconds = enemiesManager.getWaveDuration ();
		this.remainMinutes = (int)Mathf.Floor (durationWaveSeconds / 60);
		this.remainSeconds = Mathf.RoundToInt(durationWaveSeconds % 60);
	}

	void takeTime(float deltaTime){
		elapsedTime += Time.deltaTime;

		if (elapsedTime > 1) {
			elapsedTime -= 1;
			this.remainSeconds--;
		}
		if (this.remainSeconds < 0) {
			this.remainSeconds = 59;
			this.remainMinutes--;
		}

		string strMin = this.remainMinutes.ToString ();
		string strSec = this.remainSeconds.ToString ();
		if(this.remainMinutes < 10) {
			strMin = "0" + this.remainMinutes.ToString();
		}
		if(this.remainSeconds < 10) {
			strSec = "0" + this.remainSeconds.ToString();
		}

		this.textTime.text = strMin + ":" + strSec;
	}
}

public class AdsListener : IUnityAdsShowListener
{
	public Action<ShowResult> OnFinished;
	
	public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
	{
		throw new System.NotImplementedException();
	}

	public void OnUnityAdsShowStart(string placementId)
	{
		throw new System.NotImplementedException();
	}

	public void OnUnityAdsShowClick(string placementId)
	{
		throw new System.NotImplementedException();
	}

	public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
	{
		OnFinished?.Invoke(ShowResult.Finished);
	}
}
