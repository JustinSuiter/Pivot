using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyBlocker : EnemyBase
{
    [Header("Blocker Settings")]
    public float pushSpeed = 1.5f;
    public float shieldedDamageMultiplier = 0.1f;
    public float attackWindupTime = 1.2f;
    public float vulnerableWindow = 0.4f;
    public Color shieldedColor = new Color(0.2f, 0.6f, 1f);
    public Color windupColor = Color.yellow;
    public Color vulnerableColor = Color.red;

    private bool _isVulnerable;
    private bool _isWindingUp;
    protected override void TryAttack() { }

    protected override void Start()
    {
        base.Start();
        if (_agent != null)
            _agent.speed = pushSpeed;

        StartCoroutine(AttackCycle());
    }

    protected override void Pursue()
    {
        if (_agent == null || !_agent.isOnNavMesh) return;
        _agent.SetDestination(player.position);
    }

    public override void TakeDamage(float amount)
    {
        if (_isDead) return;

        float actualDamage = _isVulnerable ? amount : amount * shieldedDamageMultiplier;
        base.TakeDamage(actualDamage);
    }

    IEnumerator AttackCycle()
    {
        while (!_isDead)
        {
            while (Vector3.Distance(transform.position, player.position) > 4f)
                yield return null;

            if (_renderer != null)
                _renderer.material.color = shieldedColor;

            yield return new WaitForSeconds(2f);

            if (Vector3.Distance(transform.position, player.position) > 4f)
                continue;

            _isWindingUp = true;
            if (_renderer != null)
                _renderer.material.color = windupColor;

            yield return new WaitForSeconds(attackWindupTime);

            if (Vector3.Distance(transform.position, player.position) <= 4f)
            {
                _isVulnerable = true;
                if (_renderer != null)
                    _renderer.material.color = vulnerableColor;

                yield return new WaitForSeconds(vulnerableWindow);

                Attack();
            }

            _isWindingUp = false;
            _isVulnerable = false;

            if (_renderer != null)
                _renderer.material.color = _originalColor;
        }
    }
}