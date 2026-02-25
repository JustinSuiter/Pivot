using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class EnemyBase : MonoBehaviour
{
    [Header("Stats")]
    public float maxHealth = 100f;
    public float damage = 10f;
    public float attackRange = 1.5f;
    public float attackCooldown = 1.2f;

    [Header("References")]
    public Transform player;

    protected float _currentHealth;
    protected NavMeshAgent _agent;
    protected Renderer _renderer;
    protected Color _originalColor;
    protected float _attackTimer;
    protected bool _isDead;

    protected virtual void Start()
    {
        StartCoroutine(DebugPlayerPos());
        Debug.Log("Found player: " + player.name + " ID: " + player.GetInstanceID());
        Debug.Log(gameObject.name + " player ref: " + (player == null ? "NULL" : player.name + " at " + player.position));
        _currentHealth = maxHealth;
        _agent = GetComponent<NavMeshAgent>();
        _renderer = GetComponentInChildren<Renderer>();

        if (_renderer != null)
            _originalColor = _renderer.material.color;

        if (player == null)
        {
            CharacterController cc = FindFirstObjectByType<CharacterController>();
            if (cc != null) player = cc.transform;
}
    }

    protected virtual void Update()
    {
        if (_isDead || player == null) return;
        if (GameManager.Instance != null && GameManager.Instance.isGameOver) return;

        Debug.Log("Player world pos: " + player.position + " | My pos: " + transform.position + " | Same object? " + (player.gameObject == this.gameObject));

        _attackTimer -= Time.deltaTime;
        Pursue();
        TryAttack();
    }

    protected virtual void Pursue()
    {
        if (_agent != null && _agent.isOnNavMesh)
        {
            _agent.SetDestination(player.position);
            Debug.Log("Setting destination to: " + player.position + " agent pos: " + transform.position);
        }
    }

    protected virtual void TryAttack()
    {
        if (_attackTimer > 0f) return;

        float dist = Vector3.Distance(transform.position, player.position);
        if (dist <= attackRange)
        {
            Attack();
            _attackTimer = attackCooldown;
        }
    }

    protected virtual void Attack()
    {
        Vector3 attackOrigin = transform.position + Vector3.up * 1f;
        Collider[] hits = Physics.OverlapSphere(attackOrigin, attackRange);

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                GameManager.Instance?.TakeDamage(damage);
                break;
            }
        }
    }

    public virtual void TakeDamage(float amount)
    {
        if (_isDead) return;
        _currentHealth -= amount;
        StartCoroutine(FlashOnHit());
        if (_currentHealth <= 0f)
            Die();
    }

    IEnumerator FlashOnHit()
    {
        if (_renderer == null) yield break;
        _renderer.material.color = Color.white;
        yield return new WaitForSeconds(0.05f);
        if (!_isDead)
            _renderer.material.color = _originalColor;
    }

    protected virtual void Die()
    {
        _isDead = true;
        AudioManager.Instance?.PlayEnemyDeath();
        if (_agent != null) _agent.enabled = false;
        WaveManager.Instance?.HandleEnemyDeath();
        GameManager.Instance?.AddScore(100);
        SyncMeter.Instance?.AddSyncOnKill();
        GameObject deathFX = Resources.Load<GameObject>("FX_EnemyDeath");
        if (deathFX != null)
            Instantiate(deathFX, transform.position + Vector3.up, Quaternion.identity);
        Destroy(gameObject, 0.1f);
    }

    public void ApplyStagger(float duration)
    {
        StartCoroutine(StaggerCoroutine(duration));
    }

    IEnumerator StaggerCoroutine(float duration)
    {
        if (_agent != null) _agent.isStopped = true;
        yield return new WaitForSeconds(duration);
        if (_agent != null && !_isDead) _agent.isStopped = false;
    }

    IEnumerator DebugPlayerPos()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            if (player != null)
                Debug.Log("PLAYER POS CHECK: " + player.position + " | instance ID: " + player.GetInstanceID());
        }
    }
    
}