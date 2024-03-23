using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class UIJoystickManager : MonoBehaviour {

	public enum JoystickType
	{
		SHOOTER,
		GRANADE,
		BOOMERANG
	};

	public static UIJoystickManager Instance;

	public GameObject joystickRotationShooter;
	public GameObject joystickRotationGranade;
	public GameObject joystickRotationBoomerang;

	private GameObject current;
	public JoystickType CurrentType { get; private set; }

	private PlayerGuns localPlayerGuns;

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
	}

	void RefreshRotationJoystick()
	{
		switch (localPlayerGuns.GetCurrentGun ().GetGunType ()) {
			case GunData.GunType.SHOTGUN:
				this.changeJoystick (UIJoystickManager.JoystickType.SHOOTER);
				break;
			case GunData.GunType.GRANADE:
				this.changeJoystick (UIJoystickManager.JoystickType.GRANADE);
				break;
			case GunData.GunType.BOOMERANG:
				this.changeJoystick (UIJoystickManager.JoystickType.BOOMERANG);
				break;
		}
	}

	public void changeJoystick(JoystickType type){
		CurrentType = type;
		switch (type) {
		case JoystickType.SHOOTER:
			joystickRotationShooter.SetActive (true);
			joystickRotationGranade.SetActive (false);
			joystickRotationBoomerang.SetActive (false);
			current = joystickRotationShooter;
			break;
		case JoystickType.GRANADE:
			joystickRotationShooter.SetActive (false);
			joystickRotationGranade.SetActive (true);
			joystickRotationBoomerang.SetActive (false);
			current = joystickRotationGranade;
			break;
		case JoystickType.BOOMERANG:
			joystickRotationShooter.SetActive (false);
			joystickRotationGranade.SetActive (false);
			joystickRotationBoomerang.SetActive (true);
			current = joystickRotationBoomerang;
			break;
		}
	}

	public Joystick CurrentJoystick =>  current.GetComponentInChildren<Joystick>();
}
