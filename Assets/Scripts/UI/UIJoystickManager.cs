using System;
using UnityEngine;

public class UIJoystickManager : MonoBehaviour {

	public enum JoystickType
	{
		SHOOTER,
		GRANADE,
		BOOMERANG
	};

	public static UIJoystickManager Instance;

	public GameObject joystickMovement;
	public GameObject joystickRotation;
	
	public GameObject joystickMovementAutoAimOn;
	public GameObject joystickMovementAutoAimOff;
	public GameObject joystickMovementBackground;

	public GameObject joystickRotationShooter;
	public GameObject joystickRotationGranade;
	public GameObject joystickRotationBoomerang;
	
	public Joystick RotationJoystick =>  currentRotation.GetComponentInChildren<Joystick>();
	public Joystick MovementJoystick =>  currentMovement.GetComponentInChildren<Joystick>();
	
	private GameObject currentMovement;
	private GameObject currentRotation;
	public JoystickType CurrentType { get; private set; }

	private PlayerGuns localPlayerGuns;
	private bool wasAutoaim;

	private void Awake()
	{
		Instance = this;

		MultiplayerManager.Instance.OnLocalPlayerReady += OnPlayerReady;
	}

	private void OnPlayerReady(GameObject player)
	{
		localPlayerGuns = player.GetComponent<PlayerGuns>();
		localPlayerGuns.onGunChange -= RefreshRotationJoystick;
		localPlayerGuns.onGunChange += RefreshRotationJoystick;
		UpdateAutoAim();
	}

	void RefreshRotationJoystick()
	{
		switch (localPlayerGuns.GetCurrentGun().GetGunType())
		{
			case GunData.GunType.SHOTGUN:
				ChangeJoystick(JoystickType.SHOOTER);
				break;
			case GunData.GunType.GRANADE:
				ChangeJoystick(JoystickType.GRANADE);
				break;
			case GunData.GunType.BOOMERANG:
				ChangeJoystick(JoystickType.BOOMERANG);
				break;
		}
	}

	private void ChangeJoystick(JoystickType type)
	{
		CurrentType = type;
		switch (type) {
		case JoystickType.SHOOTER:
			joystickRotationShooter.SetActive (true);
			joystickRotationGranade.SetActive (false);
			joystickRotationBoomerang.SetActive (false);
			currentRotation = joystickRotationShooter;
			break;
		case JoystickType.GRANADE:
			joystickRotationShooter.SetActive (false);
			joystickRotationGranade.SetActive (true);
			joystickRotationBoomerang.SetActive (false);
			currentRotation = joystickRotationGranade;
			break;
		case JoystickType.BOOMERANG:
			joystickRotationShooter.SetActive (false);
			joystickRotationGranade.SetActive (false);
			joystickRotationBoomerang.SetActive (true);
			currentRotation = joystickRotationBoomerang;
			break;
		}
	}

	public void Update()
	{
		if (wasAutoaim == UserManager.Instance.AimingAutomatic)
			return;

		UpdateAutoAim();
	}

	private void UpdateAutoAim()
	{
		wasAutoaim = UserManager.Instance.AimingAutomatic;
		
		if (UserManager.Instance.AimingAutomatic)
		{
			joystickRotation.SetActive(false);
			joystickMovementAutoAimOff.SetActive(false);
			joystickMovementAutoAimOn.SetActive(true);
			currentMovement = joystickMovementAutoAimOn;
		}
		else
		{
			joystickRotation.SetActive(true);
			joystickMovementAutoAimOff.SetActive(true);
			joystickMovementAutoAimOn.SetActive(false);
			currentMovement = joystickMovementAutoAimOff;
		}

		var currentPos = currentMovement.GetComponent<RectTransform>().position;
		var backTransf = joystickMovementBackground.GetComponent<RectTransform>();
		backTransf.position = currentPos;
	}
}
