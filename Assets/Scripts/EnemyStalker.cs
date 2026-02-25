using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyStalker : EnemyBase
{
    [Header("Stalker Settings")]
    public float strafeSpeed = 2.5f;
    public float strikeSpeed = 9f;
    public float keepDistance = 6f;
    public float strafeTime = 4f;
    public float strafeRadius = 7f;

    private bool _isStriking;
    private float _strafeTimer;
    private int _strafeDirection = 1;

    protected override void Start()
    {
        base.Start();
        _strafeTimer = strafeTime;
        damage = 25f;

        if (_agent != null)
        {
            _agent.speed = strafeSpeed;
            _agent.stoppingDistance = 0.5f;
        }
    }

    protected override void Pursue()
    {
        if (player == null || _agent == null || !_agent.isOnNavMesh) return;
        if (_isStriking) return;

        float dist = Vector3.Distance(transform.position, player.position);
        _strafeTimer -= Time.deltaTime;

        if (_strafeTimer > 0f)
        {
            Vector3 dirToPlayer = (player.position - transform.position).normalized;
            Vector3 sideways = Vector3.Cross(dirToPlayer, Vector3.up) * _strafeDirection;

            Vector3 strafeTarget = player.position
                + (-dirToPlayer * keepDistance)
                + (sideways * 3f);

            _agent.SetDestination(strafeTarget);

            if (_strafeTimer < strafeTime * 0.5f && _strafeDirection == 1)
                _strafeDirection = -1;
        }
        else
        {
            StartCoroutine(Strike());
        }
    }

    IEnumerator Strike()
    {
        _isStriking = true;
        _agent.speed = strikeSpeed;
        _agent.SetDestination(player.position);

        yield return new WaitForSeconds(1.2f);

        _isStriking = false;
        _strafeTimer = strafeTime;
        _strafeDirection = Random.value > 0.5f ? 1 : -1;
        _agent.speed = strafeSpeed;
    }
}