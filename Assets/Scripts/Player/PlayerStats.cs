using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class PlayerStats : MonoBehaviour {

	public static PlayerStats Instance;
	
	public event Action onInitialized;
	public event Action onScoreAdd;
	public event Action onLifeChange;
	public event Action onDie;
	public event Action onGranadesThrow;

	private GameObject shooter;
	private GameObject hand;

	public int life;
	public int speed;
	[HideInInspector] public int score;

    private UserManager user;
    public AuthManager.UserData userData => user.UserData;

	public bool Initialized { get; private set; }

	public AudioClip damageSound;

	int initialLife;
	int initialSpeed;

	bool inmuneToFire;
	bool dead;

	public void ResetEvents()
	{
		onScoreAdd = null;
		onLifeChange = null;
		onDie = null;
		onGranadesThrow = null;
	}
	
	private void Awake()
	{
		Instance = this;

		shooter = GameObject.FindGameObjectWithTag ("Shooter");
		hand = GameObject.FindGameObjectWithTag ("Hand");
	}

	public async void Initialize()
	{
		initialLife = life;
		initialSpeed = speed;
		inmuneToFire = false;
		dead = false;

        user = UserManager.Instance();

		Initialized = true;
		onInitialized?.Invoke ();
	}

	public bool CheckNewHighScore()
	{
		bool newHighScore = score > userData.maxKills;
		if (newHighScore)
		{
			userData.maxKills = score;
            user.SaveKills();
		}

		return newHighScore;
	}
	
	public GameObject getPlayer(){
		return this.gameObject;
	}

	public GameObject getShooter(){
		return this.shooter;
	}

	public GameObject getHand(){
		return this.hand;
	}

	public void RecieveDamage(int damage)
	{
		life -= damage;
		if (life < 0) {
			life = 0;
		}
		if (life == 0) {
			this.Dead ();
			if (!dead) {
				dead = true;
				onDie?.Invoke ();
			}
		}
		this.GetComponent<Animator> ().SetInteger ("Life", life);

		AudioSource audio = this.GetComponent<AudioSource> ();
		audio.clip = this.damageSound;
		if (!audio.isPlaying) {
			audio.Play ();
		}

		onLifeChange?.Invoke();
	}

	public void RecieveDamageByFire(int damage)
	{
		if (!inmuneToFire) {
			inmuneToFire = true;
			Invoke ("makeVulnerableToFire", 1);
			this.RecieveDamage (damage);
		}
	}

	void makeVulnerableToFire()
	{
		this.inmuneToFire = false;
	}

	public void addLife(int amount)
	{
		life += amount;
		if (life > initialLife) {
			life = initialLife;
		}
		onLifeChange?.Invoke();
	}

	public void setSpeed(int amount, int time)
	{
		CancelInvoke ();

		speed = amount;

		Invoke ("setNormalSpeed", time);
	}

	void setNormalSpeed()
	{
		speed = initialSpeed;
	}

	public void AddPoints(int points)
	{
		score += points;
		onScoreAdd?.Invoke ();
	}

	public void AddCap()
	{
        user.AddCap();
	}

	void Dead()
	{
		this.GetComponentInChildren<PlayerGuns> ().enabled = false;
		this.GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.FreezeAll;
	}

	public void ThrowGranade()
	{
		onGranadesThrow?.Invoke ();
	}

	public void Revive()
	{
		this.score -= 15;
		if (this.score < 0) {
			this.score = 0;
		}
		this.life = 100;
		this.dead = false;
		this.GetComponent<Animator> ().SetInteger ("Life", life);

		this.GetComponentInChildren<PlayerGuns> ().enabled = true;

		float rndX = Random.Range (-8, 8);
		float rndZ = Random.Range (-8, 8);
		this.transform.position = new Vector3 (rndX, 5.5f, rndZ);

		this.GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

		//Camera.main.GetComponent<CameraFollow> ().CurrentMoveSpeed = 2;

		onLifeChange?.Invoke ();
		onScoreAdd?.Invoke ();
	}
}
