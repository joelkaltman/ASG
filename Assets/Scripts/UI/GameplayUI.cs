using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.SocialPlatforms;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
#if UNITY_ANDROID
using UnityEngine.Advertisements;
#endif

public class GameplayUI : MonoBehaviour {

	enum PanelType
	{
		MAINMENU,
		PAUSEMENU,
		GAME,
		GAMEOVER,
		OPTIONS,
		RANKING,
		MULTIPAYER
	};

	[Header("Panels")] 
	public GameObject panelPauseMenu;
	public GameObject panelGame;
	public GameObject panelGameOver;
	public GameObject panelOptions;
	public GameObject panelMultiplayer;

    [Header("UI")] 
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
	public Button gunButton;

	[Header("Gameplay Scripts")] 
	public EnemiesManager enemiesManager;

	[Header("GameOver")] 
	public GameObject newHighScoreText;

	[Header("Networking")] 
	public GameObject networkManagerSP;
	public GameObject networkManagerMP;
	
	private PlayerMovement playerMovement;
	private PlayerStats playerStats;
	private PlayerGuns playerGuns;
	
	private float objetiveFade;
	private float currentFade;
	private float speedFade;
	private bool usedContinue;

	private PanelType currentPanel;
	private PanelType lastPanel;

	private float elapsedTime;
	private int remainSeconds;
	private int remainMinutes;

	private NetworkManager netManager;

	void Awake()
	{
		MultiplayerManager.Instance.OnGameReady += OnGameReady;
        UserManager.Instance().OnCapCountChange += RefreshCaps;
        
		enemiesManager.onWaveChange += ShowWave;
		enemiesManager.onWaveChange += RefreshWaveTime;
        
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
	void Start () 
	{
		if (!GameData.Instance.isOnline)
		{
			ShowCanvas(PanelType.GAME);
			MultiplayerManager.Instance.InitializeSinglePlayer();
		}
		else
		{
			ShowCanvas(PanelType.MULTIPAYER);
			MultiplayerManager.Instance.InitializeMultiplayer();
		}

		joystickMovement.SetActive (GameData.Instance.isMobile);
		joystickRotation.SetActive (GameData.Instance.isMobile);
    }

	void Update ()
	{
		if (!playerStats || !playerStats.Initialized)
			return;
		
		float frecuency = playerGuns.GetCurrentGun ().Frecuency;
		imageFrecuency.fillAmount += Time.deltaTime / frecuency;

		FadeWave ();

		if (currentPanel == PanelType.GAME) {
			TakeTime (Time.deltaTime);
		}
	}

	private void OnGameReady()
	{
		var spawned = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
		
		playerMovement = spawned.GetComponent<PlayerMovement>();
		playerStats = spawned.GetComponent<PlayerStats>();
		playerGuns = spawned.GetComponent<PlayerGuns>();
		
		playerMovement.joystickMovement = joystickMovement.GetComponentInChildren<Joystick>();
		
		playerStats.onInitialized += OnUserInitialized;
		playerStats.onScoreAdd += RefreshScore;
		playerStats.onLifeChange += RefreshLife;
		playerStats.onDie += RefreshGameOver;
	
		playerGuns.onGunChange += RefreshGun;
		playerGuns.onGunChange += RefreshGunCount;
	
		playerGuns.onShoot += RefreshGunCount;
		playerGuns.onShoot += ResetFrecueny;
	
		playerStats.Initialize();
		
		gunButton.onClick.AddListener(playerGuns.SelectGunMobile);
    
		StartGame();
	}

	private void ShowCanvas(PanelType type)
	{
		switch (type) {
		    case PanelType.PAUSEMENU:
			    panelPauseMenu.SetActive (true);
			    panelGame.SetActive (false);
                panelGameOver.SetActive (false);
                panelOptions.SetActive (false);
			    panelMultiplayer.SetActive(false);
			    break;
		    case PanelType.GAME:
			    panelPauseMenu.SetActive (false);
			    panelGame.SetActive (true);
			    panelGameOver.SetActive (false);
                panelOptions.SetActive (false);
			    panelMultiplayer.SetActive(false);
			    break;
            case PanelType.GAMEOVER:
                panelPauseMenu.SetActive (false);
                panelGame.SetActive (false);
                panelGameOver.SetActive (true);
                panelOptions.SetActive (false);
                panelMultiplayer.SetActive(false);
                break;
		    case PanelType.OPTIONS:
			    panelPauseMenu.SetActive (false);
			    panelGame.SetActive (false);
			    panelGameOver.SetActive (false);
			    panelOptions.SetActive (true);
			    panelMultiplayer.SetActive(false);
			    break;
		    case PanelType.MULTIPAYER:
			    panelPauseMenu.SetActive (false);
			    panelGame.SetActive (false);
			    panelGameOver.SetActive (false);
			    panelOptions.SetActive (false);
			    panelMultiplayer.SetActive(true);
			    break;
		}

		lastPanel = currentPanel;
		currentPanel = type;
	}

	private void StartGame()
	{
		transform.parent.gameObject.SetActive (true);

		elapsedTime = 0;

		int durationWaveSeconds = enemiesManager.getWaveDuration ();
		remainMinutes = (int)Mathf.Floor (durationWaveSeconds / 60);
		remainSeconds = Mathf.RoundToInt(durationWaveSeconds % 60);
	}

	public void PauseGame()
	{
		Time.timeScale = 0;
		ShowCanvas (PanelType.PAUSEMENU);
	}

	public void ResumeGame()
	{
		Time.timeScale = 1;
        ShowCanvas (PanelType.GAME);
	}
		
	public void GoToMainMenu()
	{
		Time.timeScale = 1;
		
		if (NetworkManager.Singleton != null)
		{
			NetworkManager.Singleton.Shutdown();
			Destroy(NetworkManager.Singleton);
		}

		SceneManager.LoadScene ("MainMenu");
	}

	public void GoToLastPanel()
	{
        ShowCanvas (lastPanel);
	}
    
    public void GoToOptions()
    {
        ShowCanvas (PanelType.OPTIONS);
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

	public void Continue()
	{
		#if UNITY_ANDROID
		// Watch add
		if (Advertisement.isInitialized)
		{
		}
		#endif

		#if UNITY_STANDALONE
		usedContinue = true;
		playerStats.Revive ();
		joystickMovement.GetComponentInChildren<Joystick> ().Reset ();
		joystickRotation.GetComponentInChildren<Joystick> ().Reset ();
		showCanvas (PanelType.GAME);
		#endif
	}

	#if UNITY_ANDROID
	private void HandleShowResult(ShowResult result)
	{
		switch (result)
		{
		case ShowResult.Finished:
			usedContinue = true;
			playerStats.Revive ();
			joystickMovement.GetComponentInChildren<Joystick> ().Reset ();
			joystickRotation.GetComponentInChildren<Joystick> ().Reset ();
            ShowCanvas (PanelType.GAME);
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
		imageFrecuency.fillAmount = 0;
	}

	public void OnUserInitialized()
	{
		playerGuns.InitializeGuns(playerStats);
		
		RefreshLife ();
		RefreshScore ();
		RefreshCaps ();
		RefreshGun ();
		RefreshGunCount ();
	}
	
	public void RefreshCaps()
	{
		for (int i = 0; i < textCapCount.Count; i++) {
			if(textCapCount[i] == null)
				continue;
			
			textCapCount[i].text = "x" + playerStats.userData.caps;
			BounceText bounce = textCapCount[i].GetComponent<BounceText> ();
			if (bounce != null) {
				bounce.Bounce (0.65f);
			}
		}
	}

	public void RefreshGameOver()
	{
        ShowCanvas (PanelType.GAMEOVER);
		textGiantScore.text = "You killed " + playerStats.score + " enemies!";

		bool newMaxScore = playerStats.CheckNewHighScore();
		newHighScoreText.SetActive(newMaxScore);
	}

	void ShowWave(){
		int wave = enemiesManager.wave;
		textWave.text = "Wave " + wave.ToString ();
		objetiveFade = 1;

		Invoke ("HideWave", 3);
	}

	void HideWave(){
		objetiveFade = 0;
	}

	void FadeWave(){
		if (currentFade < objetiveFade) {
			currentFade += speedFade;
			if (currentFade > objetiveFade) {
				currentFade = objetiveFade;
			}
		}
		if (currentFade > objetiveFade) {
			currentFade -= speedFade;
			if (currentFade < objetiveFade) {
				currentFade = objetiveFade;
			}
		}
			
		Color curr = textWave.color;
		curr.a = (float)currentFade;
		textWave.color = curr;
	}

	void RefreshWaveTime(){
		int durationWaveSeconds = enemiesManager.getWaveDuration ();
		remainMinutes = (int)Mathf.Floor (durationWaveSeconds / 60);
		remainSeconds = Mathf.RoundToInt(durationWaveSeconds % 60);
	}

	void TakeTime(float deltaTime){
		elapsedTime += Time.deltaTime;

		if (elapsedTime > 1) {
			elapsedTime -= 1;
			remainSeconds--;
		}
		if (remainSeconds < 0) {
			remainSeconds = 59;
			remainMinutes--;
		}

		string strMin = remainMinutes.ToString ();
		string strSec = remainSeconds.ToString ();
		if(remainMinutes < 10) {
			strMin = "0" + remainMinutes.ToString();
		}
		if(remainSeconds < 10) {
			strSec = "0" + remainSeconds.ToString();
		}

		textTime.text = strMin + ":" + strSec;
	}
}
