using System;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using Task = System.Threading.Tasks.Task;

public class PlayerGuns : NetworkBehaviour
{
	public NetworkVariable<GunData.ShootingType> ShootType = new(GunData.ShootingType.NORMAL); 
	
	public GameObject shooter;
	public GameObject hand;
	
	public event Action onGunChange;
	public event Action onShoot;

	private PlayerStats playerStats;
	private int currentIndex;

	public bool Initialized;

	public void ResetEvents()
	{
		onGunChange = null;
		onShoot = null;
	}

	private void Awake()
	{
		playerStats = gameObject.GetComponent<PlayerStats>();
	}

	public void Initialize()
	{
		if (!IsOwner)
			return;
		
		var ownedGuns = GameData.Instance.guns.Where(x => playerStats.userData.guns.Contains(x.Id)).ToList();
		foreach (var owned in ownedGuns)
		{
			owned.Initialize(gameObject);
		}
		ownedGuns.First().Equip();

		Initialized = true;
		onGunChange?.Invoke ();
	}

	void Update ()
	{
		if (!IsOwner)
			return;
		
		if (!Initialized)
			return;

		var gun = GetCurrentGun();
		gun.AddElapsedTime (Time.deltaTime);

		if (UIJoystickManager.Instance.getCurrentJoystick().canShoot()) 
		{
			if (gun.Shoot())
			{
				var audio = GetComponentInChildren<AudioSource> ();
				audio.clip = gun.ShootAudio;
				audio.Play ();

				onShoot?.Invoke ();
			}
		}
	}

	public void SelectGunMobile()
	{
		DiscardGun ();
		currentIndex++;
		if (currentIndex == playerStats.userData.guns.Count) {
			currentIndex = 0;
		}
		EquipGun ();
		onGunChange?.Invoke ();
	}

	private void DiscardGun()
	{
		GunData currentGun = GetCurrentGun();
		currentGun.Discard ();
	}

	private void EquipGun ()
	{
		GunData currentGun = GetCurrentGun();
		currentGun.Equip ();

		AudioSource audio = GetComponentInChildren<AudioSource> ();
		audio.clip = currentGun.EquipAudio;
		audio.Play ();
	}

	private GunData GetGun(int id)
	{
		return GameData.Instance.guns.First(x => x.Id == id);
	}
	
	public GunData GetCurrentGun()
	{
		int currentId = playerStats.userData.guns[currentIndex];
		return GetGun(currentId);
	}
	
	public void ChangeShootingType(GunData.ShootingType newType, int duration)
	{
		ShootType.Value = newType;
		ChangeShootingTypeToNormal(duration);
	}

	private async void ChangeShootingTypeToNormal(int duration)
	{
		await Task.Delay(duration * 1000);
		ShootType.Value = GunData.ShootingType.NORMAL;
	}

	[ClientRpc]
	public void BoomerangReturnedClientRpc(ulong clientId, string weaponName, bool addCount)
	{
		if(OwnerClientId != clientId)
			return;
		
		foreach (var gun in GameData.Instance.guns)
		{
			if (gun.CurrentCount < gun.InitialCount && gun.GetGunType() == GunData.GunType.BOOMERANG && 
			    weaponName.Contains(gun.name)) {
				BoomerangData boomerang = (BoomerangData)gun;
				boomerang.BoomerangReturned(addCount);
			}
		}
		onGunChange?.Invoke ();
	}

	[ServerRpc]
	public void ShootServerRpc(int id, Vector3 pos, Quaternion rot)
	{
		if (!MultiplayerManager.Instance.IsHostReady)
			return;
		
		if (playerStats.IsDead)
			return;
		
		var gun = GetGun(id);
		var spawnedBullet = Instantiate(gun.Bullet, pos, rot);
		
		var playerOwned = spawnedBullet.GetComponents<PlayerOwned>();
		foreach (var owned in playerOwned)
			owned.player = gameObject;
		
		spawnedBullet.GetComponent<NetworkObject>()?.Spawn(true);
	}
}
