using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PlayerGuns : MonoBehaviour {

	public static PlayerGuns Instance;

	public event Action onGunChange;
	public event Action onShoot;

	[HideInInspector] public List<int> gunsIndices;
	[HideInInspector] public int currentIndex;

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
		Instance = this;

		instanceGroundTarget = GameObject.Instantiate (groundTarget, new Vector3 (), Quaternion.identity);
		instanceGroundTarget.SetActive (false);
	}

	public void InitializeGuns()
	{		
		this.gunsIndices = DataBase.Instance.LoadGunIndices ();
		for (int i = 0; i < gunsIndices.Count; i++) {
			GameData.Instance.guns [gunsIndices[i]].Initialize ();
		}
		GameData.Instance.guns [gunsIndices[0]].Equip ();

		this.enabled = false;

		Initialized = true;
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
				if (currentIndex == this.gunsIndices.Count) {
					currentIndex = 0;
				}
				this.EquipGun ();
				onGunChange?.Invoke ();
			} else if (Input.GetAxis ("Mouse ScrollWheel") < 0f) {
				this.DiscardGun ();
				currentIndex--;
				if (currentIndex < 0) {
					currentIndex = this.gunsIndices.Count - 1;
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
			if (currentIndex == this.gunsIndices.Count) {
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
		GunData currentGun = GameData.Instance.guns [this.gunsIndices [currentIndex]];
		if (currentGun.ShowTarget == true && currentGun.CurrentCount > 0 && Time.timeScale > 0) {
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

	public GunData GetCurrentGun()
	{
		return GameData.Instance.guns[this.gunsIndices [currentIndex]];
	}
	
	public void ChangeShootingType(GunData.ShootingType newType, int duration)
	{
		for (int i = 0; i < this.gunsIndices.Count; i++) {
			GameData.Instance.guns[this.gunsIndices[i]].ChangeShootingType (newType);
		}
		Invoke ("ChangeShootingTypeToNormal", duration);
	}

	public void ChangeShootingTypeToNormal()
	{
		for (int i = 0; i < this.gunsIndices.Count; i++) {
			GameData.Instance.guns[this.gunsIndices[i]].ChangeShootingType (GunData.ShootingType.NORMAL);
		}
	}

	public void BoomerangReturned(string weaponName, bool addCount){
		for (int i = 0; i < this.gunsIndices.Count; i++) {
			GunData gun = GameData.Instance.guns [this.gunsIndices [i]];
			if (gun.CurrentCount < gun.InitialCount && gun.GetGunType() == GunData.GunType.BOOMERANG && 
				weaponName.Contains(gun.name)) {
				BoomerangData boomerang = (BoomerangData)gun;
				boomerang.bumerangReturned (addCount);
			}
		}
		onGunChange?.Invoke ();
	}
}
