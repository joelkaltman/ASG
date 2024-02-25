using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PlayerGuns : MonoBehaviour {
	public event Action onGunChange;
	public event Action onShoot;

	private PlayerStats playerStats;
	private int currentIndex;

	public GameObject groundTarget;
	GameObject instanceGroundTarget;

	public bool Initialized;

	public void ResetEvents()
	{
		onGunChange = null;
		onShoot = null;
	}
	
	void Awake()
	{
		instanceGroundTarget = GameObject.Instantiate (groundTarget, new Vector3 (), Quaternion.identity);
		instanceGroundTarget.SetActive (false);
	}

	public void InitializeGuns(PlayerStats playerStats)
	{
		this.playerStats = playerStats;
		var ownedGuns = GameData.Instance.guns.Where(x => playerStats.userData.guns.Contains(x.Id));
		foreach (var owned in ownedGuns)
		{
			owned.Initialize();
		}
		ownedGuns.First().Equip();

		Initialized = true;
		onGunChange?.Invoke ();
	}

	void Update ()
	{
		if (!Initialized)
			return;
		
		this.SelectGunScroll();

		if (!GameData.Instance.isMobile) {
			this.UpdateTarget ();
		}

		this.GetCurrentGun().AddElapsedTime (Time.deltaTime);

		bool shooted = false;
		if (GameData.Instance.isMobile) {
			if (UIJoystickManager.Instance.getCurrentJoystick ().canShoot ()) {
				shooted = this.GetCurrentGun ().Shoot ();
			}
		}else{
			if (Input.GetMouseButton (0) && !EventSystem.current.IsPointerOverGameObject ()) {
				shooted = this.GetCurrentGun ().Shoot ();
			}
		}
		if (shooted) {
			AudioSource audio = this.GetComponentInChildren<AudioSource> ();
			audio.clip = this.GetCurrentGun ().ShootAudio;
			audio.Play ();

			onShoot?.Invoke ();
		}
	}

	private void SelectGunScroll()
	{
		if (!GameData.Instance.isMobile) {
			if (Input.GetAxis ("Mouse ScrollWheel") > 0f) {
				this.DiscardGun ();
				currentIndex++;
				if (currentIndex == playerStats.userData.guns.Count) {
					currentIndex = 0;
				}
				this.EquipGun ();
				onGunChange?.Invoke ();
			} else if (Input.GetAxis ("Mouse ScrollWheel") < 0f) {
				this.DiscardGun ();
				currentIndex--;
				if (currentIndex < 0) {
					currentIndex = playerStats.userData.guns.Count - 1;
				}
				this.EquipGun ();
				onGunChange?.Invoke ();
			}
		}
	}

	public void SelectGunMobile(){
		if (GameData.Instance.isMobile) {
			this.DiscardGun ();
			currentIndex++;
			if (currentIndex == playerStats.userData.guns.Count) {
				currentIndex = 0;
			}
			this.EquipGun ();
			onGunChange?.Invoke ();
		}
	}

	private void DiscardGun(){
		GunData currentGun = this.GetCurrentGun();
		currentGun.Discard ();
	}

	private void EquipGun (){
		GunData currentGun = this.GetCurrentGun();
		currentGun.Equip ();

		AudioSource audio = this.GetComponentInChildren<AudioSource> ();
		audio.clip = currentGun.EquipAudio;
		audio.Play ();
	}

	public void UpdateTarget()
	{
		GunData currentGun = GetCurrentGun();
		if (currentGun.ShowTarget && currentGun.CurrentCount > 0 && Time.timeScale > 0) {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			if (Physics.Raycast (ray, out hit)) {
				instanceGroundTarget.transform.position = new Vector3 (hit.point.x, 5, hit.point.z);
			}
			instanceGroundTarget.SetActive (true);
		} else {
			instanceGroundTarget.SetActive (false);
		}
	}

	private GunData GetGunFromId(int id)
	{
		return GameData.Instance.guns.First(x => x.Id == id);
	}
	
	public GunData GetCurrentGun()
	{
		int currentId = playerStats.userData.guns[currentIndex];
		return GetGunFromId(currentId);
	}
	
	public void ChangeShootingType(GunData.ShootingType newType, int duration)
	{
		foreach (var gunId in playerStats.userData.guns)
		{
			GetGunFromId(gunId).ChangeShootingType (newType);
		}
		Invoke ("ChangeShootingTypeToNormal", duration);
	}

	public void ChangeShootingTypeToNormal()
	{
		foreach (var gunId in playerStats.userData.guns)
		{
			GetGunFromId(gunId).ChangeShootingType (GunData.ShootingType.NORMAL);
		}
	}

	public void BoomerangReturned(string weaponName, bool addCount){
		foreach (var gun in GameData.Instance.guns)
		{
			if (gun.CurrentCount < gun.InitialCount && gun.GetGunType() == GunData.GunType.BOOMERANG && 
			    weaponName.Contains(gun.name)) {
				BoomerangData boomerang = (BoomerangData)gun;
				boomerang.bumerangReturned (addCount);
			}
		}
		onGunChange?.Invoke ();
	}
}
