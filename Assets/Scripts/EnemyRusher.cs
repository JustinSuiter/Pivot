using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyRusher : EnemyBase
{
    [Header("Rusher Settings")]
    public float approachSpeed = 4f;
    public float lungeSpeed = 14f;
    public float lungeRange = 6f;
    public float lungePause = 0.4f;

    private bool _isLunging;
    private bool _isPausing;

    protected override void Start()
    {
        base.Start();
        if (_agent != null)
            _agent.speed = approachSpeed;
    }

    protected override void Pursue()
    {
        if (player == null || _agent == null || !_agent.isOnNavMesh) return;
        if (_isLunging || _isPausing) return;

        float dist = Vector3.Distance(transform.position, player.position);

        if (dist < lungeRange)
            StartCoroutine(Lunge());
        else
            _agent.SetDestination(player.position);
    }

    IEnumerator Lunge()
    {
        _isPausing = true;
        _agent.ResetPath();
        _agent.speed = 0f;

        yield return new WaitForSeconds(lungePause);

        _isPausing = false;
        _isLunging = true;
        _agent.speed = lungeSpeed;
        _agent.SetDestination(player.position);

        yield return new WaitForSeconds(0.4f);

        _isLunging = false;
        _agent.speed = approachSpeed;
    }
}