using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public float ViewingDistance = 10f;

    public float AttackDistance = 2f;

    public GameObject AttackPoint;

    public float AttackRange = 0.7f;

    public LayerMask PlayerLayer;

    public int AttackCountdownSeconds = 1;

    public int Health = 30;

    public ParticleSystem DamageParticle;

    private bool EnableAttack = true;
    private Transform _Target;
    private NavMeshAgent _Agent;
    private Animator _Animator;
    private GameManager _GameManager;

    private float DistanceToPlayer;

    private void Start()
    {
        _Target = GameManager.ManagerInstance.Player.transform;
        _Agent = GetComponent<NavMeshAgent>();
        _Animator = GetComponent<Animator>();
        _GameManager = FindObjectOfType<GameManager>();
    }

    public void DealDamage(int Count)
    {
        Health -= Count;
        DamageParticle.Play();
    }

    private void Attack()
    {
        Collider[] HitedColliders = Physics.OverlapSphere(AttackPoint.transform.position, AttackRange, PlayerLayer);
        EnableAttack = true;

        foreach (Collider collider in HitedColliders)
        {
            _GameManager.DamagePlayer(10);
            Debug.Log(_GameManager.Health);
        }
    }

    private IEnumerator AttackCountdown()
    { 
        EnableAttack = false;

        yield return new WaitForSeconds(1);

        Attack();
    }

    private void FixedUpdate()
    {
        DistanceToPlayer = Vector3.Distance(_Target.position, transform.position);

        if (DistanceToPlayer <= ViewingDistance)
        {
            _Agent.SetDestination(_Target.position);
            transform.LookAt(_Target.position);

            if (DistanceToPlayer <= ViewingDistance && EnableAttack) StartCoroutine(AttackCountdown());
        }

        if (Health <= 0) Death();
    }

    private void Death()
    {
        DamageParticle.transform.parent = null;
        DamageParticle.Play();

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(AttackPoint.transform.position, AttackRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, ViewingDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AttackDistance);
    }

    private void SetAnimation()
    {
        if (DistanceToPlayer <= AttackDistance && EnableAttack) _Animator.SetTrigger("Attack");
        else
        {
            if (DistanceToPlayer <= ViewingDistance) _Animator.SetInteger("Animation", 1);
            else _Animator.SetInteger("Animation", 0);
        }
    }
}
