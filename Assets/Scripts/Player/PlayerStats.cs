using Unity.Collections;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;
using Task = System.Threading.Tasks.Task;

public class PlayerStats : NetworkBehaviour 
{
	[HideInInspector] public NetworkVariable<int> Life = new(100);
	[HideInInspector] public NetworkVariable<int> Score = new(0);
	[HideInInspector] public NetworkVariable<float> Speed = new(5);
	[HideInInspector] public NetworkVariable<int> Caps = new(0);
	[HideInInspector] public NetworkVariable<FixedString64Bytes> Username = new();

    private UserManager user;
    public AuthManager.UserData userData => user.UserData;

	public AudioClip damageSound;

	private int initialLife;
	[HideInInspector] public float initialSpeed;

	bool inmuneToFire;
	public bool IsDead => Life.Value <= 0;

	private void Start()
	{
		if (IsOwner)
		{
			user = UserManager.Instance();
			user.ResetKills();

			string username = user.UserData.username;
			if (IsHost)
			{
				Username.Value = username;
			}
			else
			{
				SendUsernameServerRpc(username);
			}
		}

		MultiplayerManager.Instance.RegisterPlayer(gameObject);

		Caps.OnValueChanged += AddCap;
		Life.OnValueChanged += OnLifeChange;
		Score.OnValueChanged += OnKill;
		
		initialLife = Life.Value;
		initialSpeed = Speed.Value;
	}
	
	[ServerRpc]
	private void SendUsernameServerRpc(string username)
	{
		if (IsOwner)
			return;
		
		Username.Value = username;
	}


	private void AddCap(int prevCaps, int newCaps)
	{
		if (!IsOwner)
			return;
		
		user.AddCap();
	}
	
	private void OnKill(int prevScore, int newScore)
	{
		if (!IsOwner)
			return;
		
		user.SetKills(Score.Value);
	}

	public void RecieveDamage(int damage)
	{
		Life.Value = math.max(Life.Value - damage, 0);
	}

	private void OnLifeChange(int prevLife, int newLife)
	{
		GetComponent<Animator> ().SetInteger ("Life", newLife);

		if (IsDead)
		{
			Die();
		}
		else
		{
			AudioSource audio = GetComponent<AudioSource> ();
			audio.clip = damageSound;
			if (!audio.isPlaying) {
				audio.Play ();
			}
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
		Life.Value = math.min(initialLife, Life.Value + amount);
	}

	public void IncreaseSpeed(float amount, int time)
	{
		if (Speed.Value > initialSpeed)
			return;
		
		Speed.Value *= amount;
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

	void Die()
	{
		GetComponentInChildren<PlayerGuns> ().enabled = false;
		GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.FreezeAll;
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
