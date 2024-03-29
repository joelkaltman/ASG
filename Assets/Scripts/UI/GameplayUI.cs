using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    public GameObject topPanel;
    public Transform locatorsContainer;
    public Text playerName;
	public Text textScore;
	public Text textTime;
	public Text textGameOverReason;
	public Text textGiantScore;
	public Text textGunCount;
	public Text textWave;
	public Text textJoinCode;
	public Image imageLife;
	public Image imageGun;
	public Image imageFrecuency;
	public List<Image> buttonsSound;
	public Sprite soundOn;
	public Sprite soundOff;
	public GameObject joystickMovement;
	public GameObject joystickRotation;
	public Button gunButton;
	
	[Header("RemotePlayer")]
	public GameObject remotePlayerPanel;
	public Text textRemoteUsername;
	public Image imageRemoteLife;

	[Header("GameOver")] 
	public GameObject newHighScoreText;
	
	private PlayerMovement playerMovement;
	private PlayerStats playerStats;
	private PlayerGuns playerGuns;
	
	private float objetiveFade;
	private float currentFade;
	private float speedFade;
	private bool usedContinue;

	private PanelType currentPanel;
	private PanelType lastPanel;

	private NetworkManager netManager;

	private string remoteUsername;

	void Awake()
	{
		objetiveFade = 0;
		currentFade = 0;
		speedFade = 0.01f;
		usedContinue = false;
		currentPanel = 0;
		lastPanel = 0;
	}

	// Use this for initialization
	void Start () 
	{
		MultiplayerManager.Instance.OnLocalPlayerReady += OnLocalPlayerReady;
		MultiplayerManager.Instance.OnRemotePlayerReady += OnRemotePlayerReady;
		MultiplayerManager.Instance.OnGameReady += StartGame;
		MultiplayerManager.Instance.OnGameOver += GameOver;

		
		TargetLocator.SetLocatorListener(OnLocatorSpawn);
		
		if (!GameData.Instance.isOnline)
		{
			ShowCanvas(PanelType.GAME);
			MultiplayerManager.Instance.InitializeSinglePlayer();
		}
		else
		{
			ShowCanvas(PanelType.MULTIPAYER);
			var mpUI = panelMultiplayer.GetComponent<MultiplayerUI>();
			mpUI.OnHostStarted += OnHostStarted;
			MultiplayerManager.Instance.OnLocalPlayerReady += OnClientStarted;
		}

		joystickMovement.SetActive (true);
		joystickRotation.SetActive (true);
		
		remotePlayerPanel.SetActive(false);
    }

	void Update ()
	{
		if (!MultiplayerManager.Instance.IsGameReady)
			return;
		
		GunReload();

		FadeWave ();
		
		RefreshWaveTime();
	}

	private void OnHostStarted(string code)
	{
		ShowCanvas(PanelType.GAME);
		topPanel.SetActive(false);
		textJoinCode.gameObject.SetActive(true);
		textJoinCode.text = code;
	}

	public void ShareCode()
	{
		var joinCode = textJoinCode.text;
		var username = UserManager.Instance.UserData.username;
		
		new NativeShare()
			.SetSubject("Another Shooting Game")
			.SetText($"{username} has invited you to kill some monsters!")
			.SetUrl($"https://asgame-6af5c.web.app?code={joinCode}")
			.Share();
	}
	
	private void OnClientStarted(GameObject player)
	{
		ShowCanvas(PanelType.GAME);
	}
	
	private void OnLocalPlayerReady(GameObject player)
	{
		playerMovement = player.GetComponent<PlayerMovement>();
		playerStats = player.GetComponent<PlayerStats>();
		playerGuns = player.GetComponent<PlayerGuns>();

		playerStats.Life.OnValueChanged += RefreshLife;
		playerStats.Score.OnValueChanged += RefreshScore;
	
		playerGuns.onGunChange += RefreshGun;
		playerGuns.onGunChange += RefreshGunCount;
	
		playerGuns.onShoot += RefreshGunCount;
		playerGuns.onShoot += ResetFrecueny;
	
		gunButton.onClick.AddListener(playerGuns.SelectGunMobile);
		
		playerGuns.Initialize();
		playerMovement.Initialize();

		playerName.text = UserManager.Instance.UserData.username;
		
		RefreshLife (playerStats.Life.Value, playerStats.Life.Value);
		RefreshScore (playerStats.Score.Value, playerStats.Score.Value);
		RefreshGun ();
		RefreshGunCount ();
	}

	private void OnRemotePlayerReady(GameObject player)
	{
		remotePlayerPanel.SetActive(true);
		var remotePlayerStats = player.GetComponent<PlayerStats>();
		SetUsername(remotePlayerStats.Username.Value, remotePlayerStats.Username.Value);
		remotePlayerStats.Username.OnValueChanged += SetUsername;
		remotePlayerStats.Life.OnValueChanged += RefreshRemoteLife;
	}

	private void SetUsername(FixedString64Bytes old, FixedString64Bytes username)
	{
		remoteUsername = username.ToString();
		textRemoteUsername.text = remoteUsername;
	}

	private void OnLocatorSpawn(GameObject locator)
	{
		locator.transform.SetParent(locatorsContainer);
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
		topPanel.SetActive(true);
		textJoinCode.gameObject.SetActive(false);

		MultiplayerManager.Instance.WavesManager.Wave.OnValueChanged += ShowWave;
	}

	public void PauseGame()
	{
		if(!GameData.Instance.isOnline)
			Time.timeScale = 0;

		ShowCanvas (PanelType.PAUSEMENU);
	}

	public void ResumeGame()
	{
		Time.timeScale = 1;
        ShowCanvas (PanelType.GAME);
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

	private void GunReload()
	{
		if (!playerGuns || !playerGuns.Initialized)
			return;
		
		float frecuency = playerGuns.GetCurrentGun ().Frecuency;
		imageFrecuency.fillAmount += Time.deltaTime / frecuency;
	}
	
	private void RefreshScore(int previousScore, int score)
	{
		textScore.text = playerStats.Score.Value.ToString();
		BounceText bounce = textScore.GetComponent<BounceText> ();
		if (bounce != null) {
			bounce.Bounce (0.65f);
		}
	}

	private void RefreshLife(int previousLife, int currentLife)
	{
		imageLife.fillAmount = (float)currentLife / 100;
	}
	
	private void RefreshRemoteLife(int previousLife, int currentLife)
	{
		imageRemoteLife.fillAmount = (float)currentLife / 100;
	}

	private void RefreshGun()
	{
		imageGun.GetComponent<Image> ().sprite = playerGuns.GetCurrentGun().Sprite;
		//imageGun.GetComponent<Image> ().SetNativeSize ();
	}

	private void RefreshGunCount()
	{
		int count = playerGuns.GetCurrentGun ().CurrentCount;
		if (count >= 0) {
			textGunCount.text = "x" + count.ToString();
		} else {
			textGunCount.text = "x99999...";
		}
	}

	private void ResetFrecueny()
	{
		imageFrecuency.fillAmount = 0;
	}

	private void GameOver(MultiplayerManager.GameOverReason reason)
	{
        ShowCanvas (PanelType.GAMEOVER);
        
        switch (reason)
        {
	        case MultiplayerManager.GameOverReason.Disconnected:
		        textGameOverReason.text = "You got disconnected";
		        break;
	        case MultiplayerManager.GameOverReason.PlayerDied:
		        textGameOverReason.text = "You DIED!";
		        break;
	        case MultiplayerManager.GameOverReason.OtherPlayerDied:
		        textGameOverReason.text = $"{remoteUsername ?? "Your partner"} has DIED!";
		        break;
        }
        
		textGiantScore.text = "Killed " + UserManager.Instance.Kills + " enemies!";


		bool newMaxScore = UserManager.Instance.CheckNewHighScore();
		newHighScoreText.SetActive(newMaxScore);
	}

	void ShowWave(int previousWave, int newWave)
	{
		// newWave = value of index, which is one less than wave number 
		textWave.text = $"Wave {newWave + 1}";
		objetiveFade = 1;

		HideWave(3);
	}

	void RefreshWaveTime()
	{
		var wavesManager = MultiplayerManager.Instance.WavesManager;

		var min = wavesManager.Minutes.Value;
		var sec = wavesManager.Seconds.Value;
		
		string strMin = min.ToString ();
		string strSec = sec.ToString ();
		if(min < 10) {
			strMin = "0" + min;
		}
		if(sec < 10) {
			strSec = "0" + sec;
		}
		
		this.textTime.text = strMin + ":" + strSec;
	}

	private async void HideWave(int seconds)
	{
		await Task.Delay(seconds * 1000);
		objetiveFade = 0;
	}

	void FadeWave()
	{
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

}
