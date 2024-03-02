using UnityEngine;

public abstract class EnemyAttack : ServerOnlyMonobehavior 
{
    public float attackRange = 2f;

    private bool wasInRange;
	
    private void Update()
    {
        var closestPlayer = MultiplayerManager.Instance.GetPlayerCloserTo(transform.position);
        bool inRange = Vector3.Distance(closestPlayer.transform.position, transform.position) <= attackRange;
        if (inRange && !wasInRange)
        {
            OnEnterAttackRange(closestPlayer);
        }
        else if (inRange && wasInRange)
        {
            OnStayAttackRange(closestPlayer);
        }
        else if (!inRange && wasInRange)
        {
            OnExitAttackRange();
        }

        wasInRange = inRange;
    }

    protected abstract void OnEnterAttackRange(GameObject player);

    protected abstract void OnStayAttackRange(GameObject player);

    protected abstract void OnExitAttackRange();
}
