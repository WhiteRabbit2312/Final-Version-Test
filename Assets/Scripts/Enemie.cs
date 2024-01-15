using System.Collections;
using UnityEngine;
using UnityEngine.AI;
public class Enemie : MonoBehaviour, IAction
{
    public float Hp;
    [SerializeField] private float Damage;
    [SerializeField] private float _attackSpeed;
    [SerializeField] private float _attackRange;

    [SerializeField] private bool _isSplit = false;
    [SerializeField] private Animator AnimatorController;
    [SerializeField] private NavMeshAgent Agent;
    [SerializeField] private GameObject _smallEnemy;
    private float _lastAttackTime;
    private bool _isDead;
    private int _twoSmallEnemies;

    private void Awake()
    {
        _lastAttackTime = 0;
        _twoSmallEnemies = 2;
        _isDead = false;
    }

    private void Start()
    {
        SceneManager.Instance.AddEnemie(this);
        
        Move();
    }

    private void Update()
    {
        if(_isDead)
        {
            return;
        }

        if (Hp <= 0)
        {
            Die();
            Agent.isStopped = true;
            return;
        }

        Attack();
    }

    public void Move()
    {
        Agent.SetDestination(SceneManager.Instance.Player.transform.position);
    }

    public void Attack()
    {
        var distance = Vector3.Distance(transform.position, SceneManager.Instance.Player.transform.position);

        if (distance <= _attackRange)
        {
            Agent.isStopped = true;
            if (Time.time - _lastAttackTime > _attackSpeed)
            {
                _lastAttackTime = Time.time;
                SceneManager.Instance.Player.Hp -= Damage;
                AnimatorController.SetTrigger("Attack");
                AnimatorController.SetBool("Move", false);
            }
        }
        else
        {
            Agent.isStopped = false;
            Agent.SetDestination(SceneManager.Instance.Player.transform.position);
            AnimatorController.SetBool("Move", true);

        }
    }
    
    private void SpawnSmallEnemy(Vector3 bigEnemyPos)
    {
        for(int i = 0; i < _twoSmallEnemies; ++i)
        {
            //I add +i to bigEnemyPos.x so the second small enemy would be aside the first one so they would not be on the same coordinates
            Instantiate(_smallEnemy, new Vector3(bigEnemyPos.x + i, bigEnemyPos.y, bigEnemyPos.z), Quaternion.identity);
            
        }
    }

    public void Die()
    {
        if (_isSplit)
        {
            SpawnSmallEnemy(transform.position);
        }

        SceneManager.Instance.Player.Hp += 3;//add hp if Player kills enemy
        SceneManager.Instance.RemoveEnemie(this);
        _isDead = true;
        AnimatorController.SetTrigger("Die");
        AnimatorController.SetBool("Dead", true);
        StartCoroutine("DestroyEnemy");
    }

    private IEnumerator DestroyEnemy()
    {
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
}
