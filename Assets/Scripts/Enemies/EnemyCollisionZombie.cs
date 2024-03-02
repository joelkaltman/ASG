using UnityEngine;

public class EnemyCollisionZombie : EnemyAttack {

	public int damage;
	public float attackFrecuency;

	float acumTime = 0;

	private Animator animator;
	private EnemyFollow enemyFollow;
	
	private void Start()
	{
		animator = GetComponent<Animator>();
		enemyFollow = GetComponent<EnemyFollow>();
	}
	
	protected override void OnEnterAttackRange(GameObject player)
	{
		animator.SetBool ("Attack", true);
		enemyFollow.follow = false;
	}

	protected override void OnStayAttackRange(GameObject player)
	{
		acumTime += Time.deltaTime;
		bool canAttack = acumTime >= attackFrecuency;
		if (canAttack)
		{
			player.GetComponent<PlayerStats>().RecieveDamage (damage);
			acumTime = 0;
		}
	}

	protected override void OnExitAttackRange()
	{
		animator.SetBool ("Attack", false);
		enemyFollow.follow = true;
	}
}
