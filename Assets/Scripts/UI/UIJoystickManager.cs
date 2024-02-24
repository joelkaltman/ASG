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
	private JoystickType currentType;

	private void Awake()
	{
		Instance = this;
	}

	void Start ()
	{
		PlayerSpawn.Instance.AddListener(OnPlayerSpawn);
		this.changeJoystick (JoystickType.SHOOTER);
	}

	private void OnDestroy()
	{
		PlayerSpawn.Instance.RemoveListener(OnPlayerSpawn);
	}

	private void OnPlayerSpawn(GameObject spawned)
	{
		if (!GameData.Instance.isOnline || spawned.GetComponent<NetworkObject>().IsOwner)
		{
			PlayerGuns.Instance.onGunChange -= RefreshRotationJoystick;
			PlayerGuns.Instance.onGunChange += RefreshRotationJoystick;
		}
	}

	void RefreshRotationJoystick(){
		if (GameData.Instance.isMobile) {
			switch (PlayerGuns.Instance.GetCurrentGun ().GetGunType ()) {
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
	}

	public void changeJoystick(JoystickType type){
		currentType = type;
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

	public Joystick getCurrentJoystick(){
		return current.GetComponent<Joystick>();
	}

	public JoystickType getCurrentJoystickType(){
		return currentType;
	}
}
