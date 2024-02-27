using System;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerStats : MonoBehaviour 
{
	public event Action onInitialized;
	public event Action onScoreAdd;
	public event Action onLifeChange;
	public event Action onDie;
	public event Action onGranadesThrow;

	private NetworkVariable<int> Life = new(100);

	public int life
	{
		get { return Life.Value; }
		private set { if(MultiplayerManager.Instance.IsHostReady) Life.Value = value; }
	}

	public int speed;
	[HideInInspector] public int score;

    private UserManager user;
    public AuthManager.UserData userData => user.UserData;

	public bool Initialized { get; private set; }

	public AudioClip damageSound;

	private int initialLife;
	private int initialSpeed;
	private int previousLife;

	bool inmuneToFire;
	public bool IsDead => life <= 0;
	

	public void ResetEvents()
	{
		onScoreAdd = null;
		onLifeChange = null;
		onDie = null;
		onGranadesThrow = null;
	}

	private void Start()
	{
		MultiplayerManager.Instance.RegisterPlayer(gameObject);
	}

	public async void Initialize()
	{
		initialLife = life;
		previousLife = life;
		initialSpeed = speed;
		inmuneToFire = false;

        user = UserManager.Instance();

		Initialized = true;
		onInitialized?.Invoke ();
	}
	
	private void Update()
	{
		if (!Initialized)
			return;
		
		if (previousLife != life)
		{
			previousLife = life;
			onLifeChange?.Invoke();
		}

		if (life == 0)
		{
			onDie?.Invoke();
		}
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

	public void RecieveDamage(int damage)
	{
		bool wasDead = IsDead;
		life -= damage;
		life = math.max(life, 0);
		
		if (IsDead && !wasDead)
			Die();
		
		this.GetComponent<Animator> ().SetInteger ("Life", life);

		AudioSource audio = this.GetComponent<AudioSource> ();
		audio.clip = this.damageSound;
		if (!audio.isPlaying) {
			audio.Play ();
		}
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

	public void AddLife(int amount)
	{
		life += amount;
		if (life > initialLife) {
			life = initialLife;
		}
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

	void Die()
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
		this.GetComponent<Animator> ().SetInteger ("Life", life);

		this.GetComponentInChildren<PlayerGuns> ().enabled = true;

		float rndX = Random.Range (-8, 8);
		float rndZ = Random.Range (-8, 8);
		this.transform.position = new Vector3 (rndX, 5.5f, rndZ);

		this.GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

		//Camera.main.GetComponent<CameraFollow> ().CurrentMoveSpeed = 2;

		onScoreAdd?.Invoke ();
	}
}
