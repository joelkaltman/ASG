using System;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;
using Task = System.Threading.Tasks.Task;

public class PlayerStats : NetworkBehaviour 
{
	public event Action onInitialized;
	public event Action onGranadesThrow;

	public NetworkVariable<int> Life = new(100);
	public NetworkVariable<int> Score = new(0);
	public NetworkVariable<int> Speed = new(10);

    private UserManager user;
    public AuthManager.UserData userData => user.UserData;

	public bool Initialized { get; private set; }

	public AudioClip damageSound;

	private int initialLife;
	private int initialSpeed;

	bool inmuneToFire;
	public bool IsDead => Life.Value <= 0;
	

	public void ResetEvents()
	{
		onGranadesThrow = null;
	}

	private void Start()
	{
		MultiplayerManager.Instance.RegisterPlayer(gameObject);
	}

	public void Initialize()
	{
		initialLife = Life.Value;
		initialSpeed = Speed.Value;
		inmuneToFire = false;

        user = UserManager.Instance();

		Initialized = true;
		onInitialized?.Invoke ();
	}
	
	public bool CheckNewHighScore()
	{
		bool newHighScore = Score.Value > userData.maxKills;
		if (newHighScore)
		{
			userData.maxKills = Score.Value;
            user.SaveKills();
		}

		return newHighScore;
	}

	public void RecieveDamage(int damage)
	{
		bool wasDead = IsDead;
		Life.Value -= damage;
		Life.Value = math.max(Life.Value, 0);
		
		if (IsDead && !wasDead)
			Die();
		
		GetComponent<Animator> ().SetInteger ("Life", Life.Value);

		AudioSource audio = GetComponent<AudioSource> ();
		audio.clip = damageSound;
		if (!audio.isPlaying) {
			audio.Play ();
		}
	}

	public void RecieveDamageByFire(int damage)
	{
		if (!inmuneToFire) {
			inmuneToFire = true;
			Invoke ("makeVulnerableToFire", 1);
			RecieveDamage (damage);
		}
	}

	void makeVulnerableToFire()
	{
		inmuneToFire = false;
	}

	public void AddLife(int amount)
	{
		Life.Value += amount;
		if (Life.Value > initialLife) {
			Life.Value = initialLife;
		}
	}

	public void SetSpeed(int amount, int time)
	{
		Speed.Value = amount;
		RestoreSpeed(time);
	}

	async void RestoreSpeed(int sec)
	{
		await Task.Delay(sec * 1000);
		Speed.Value = initialSpeed;
	}

	public void AddPoints(int points)
	{
		Score.Value += points;
	}

	public void AddCap()
	{
        user.AddCap();
	}

	void Die()
	{
		GetComponentInChildren<PlayerGuns> ().enabled = false;
		GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.FreezeAll;
	}

	public void ThrowGranade()
	{
		onGranadesThrow?.Invoke ();
	}

	public void Revive()
	{
		Score.Value = math.max(Score.Value - 15, 0);
		Life.Value = 100;
		GetComponent<Animator> ().SetInteger ("Life", Life.Value);

		GetComponentInChildren<PlayerGuns> ().enabled = true;

		float rndX = Random.Range (-8, 8);
		float rndZ = Random.Range (-8, 8);
		transform.position = new Vector3 (rndX, 5.5f, rndZ);

		GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
	}
}
