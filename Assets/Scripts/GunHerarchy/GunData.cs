using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public abstract class GunData : ScriptableObject {

	public enum GunType
	{
		SHOTGUN,
		GRANADE,
		BOOMERANG
	};

	public enum ShootingType
	{
		NONE,
		NORMAL,
		MULTPLE,
		FAST
	};

	public int Id;
	
	[SerializeField] protected GameObject bullet;
	public GameObject Bullet{
		get{ return bullet; }
		set{ bullet = value; }
	}
	[SerializeField] protected GameObject weapon;
	public GameObject Weapon{
		get{ return weapon; }
		set{ weapon = value; }
	}
	[SerializeField] protected AudioClip shootAudio;
	public AudioClip ShootAudio{
		get{ return shootAudio; }
		set{ shootAudio = value; }
	}
	[SerializeField] protected AudioClip equipAudio;
	public AudioClip EquipAudio{
		get{ return equipAudio; }
		set{ equipAudio = value; }
	}
	[SerializeField] protected float frecuency;
	public float Frecuency{
		get{ return frecuency; }
		set{ frecuency = value; }
	}
	[SerializeField] protected int initialCount;
	public int InitialCount{
		get{ return initialCount; }
		set{ initialCount = value; }
	}
	[SerializeField] protected int damage;
	public int Damage{
		get{ return damage; }
		set{ damage = value; }
	}
	[SerializeField] protected int price;
	public int Price{
		get{ return price; }
		set{ price = value; }
	}
	[SerializeField] protected Sprite sprite;
	public Sprite Sprite{
		get{ return sprite; }
		set{ sprite = value; }
	}
	[SerializeField] protected string gunName;
	public string GunName{
		get{ return gunName; }
		set{ gunName = value; }
	}
	[SerializeField] protected string description;
	public string Description{
		get{ return description; }
		set{ description = value; }
	}
	[SerializeField] protected bool showTarget;
	public bool ShowTarget{
		get{ return showTarget; }
		set{ showTarget = value; }
	}

	protected int currentCount;
	public int CurrentCount{
		get{ return currentCount; }
		set{ currentCount = value; }
	}

	protected float timeElapsed;

	protected GameObject player;
	protected PlayerStats playerStats;
	protected PlayerGuns playerGuns;
	protected Animator animator;

	protected GameObject shooter => playerGuns.shooter;
	protected GameObject hand => playerGuns.hand;
	
	protected GameObject weaponInstance;
	protected ShootingType shootingType;

	public virtual void Initialize(GameObject player)
	{
		this.player = player;
		playerStats = player.GetComponent<PlayerStats>();
		playerGuns = player.GetComponent<PlayerGuns>();
		animator = player.GetComponent<Animator>();
	}

	public abstract bool Shoot ();

	public abstract void Equip ();

	public abstract void Discard ();

	public abstract GunType GetGunType ();

	public void ChangeShootingType(ShootingType type)
	{
		this.shootingType = type;
	}

	public void AddElapsedTime (float deltaTime) {
		timeElapsed += deltaTime;
	}
}
