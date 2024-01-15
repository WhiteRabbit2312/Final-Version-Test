using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour, IAction
{
    public float Hp;
    
    [SerializeField] private Text _timer;
    [SerializeField] private Animator AnimatorController;

    private bool _isDead;
    private bool _canAttack;
    private bool _canSuperAttack;
    private float _damage;
    private float _attackRange;
    private float _speedMove;
    private float _degreeDelta;

    private Enemie _closestEnemy;

    void Awake()
    {
        _closestEnemy = null;
        _damage = 1;
        _attackRange = 2;
        _speedMove = 10f;
        _degreeDelta = 720f;

        _canAttack = true;
        _canSuperAttack = true;
        _isDead = false;
    }

    private void Update()
    {
        if (Hp <= 0)
        {
            Die();
            SceneManager.Instance.GameOver();
            return;
        }
        
        else
        {
            Move();
        }

        //button enables only if there is a enemy near the player
        if (CheckAttackEnemy(ref _closestEnemy)) 
        { 
            SuperAttackButton.onEnableButton?.Invoke();  
        }

        else
        {
            SuperAttackButton.onDisableButton?.Invoke();
        }
    }

    public void Move()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput);

        movement.Normalize();
        transform.Translate(movement * _speedMove * Time.deltaTime, Space.World);
        if(movement != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(movement, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, _degreeDelta * Time.deltaTime);
        }

        //enable walk animation if horizontalInput or verticalInput != 0
        AnimatorController.SetFloat("Speed", movement.magnitude);
    }

    
    public void Attack()
    {
        if(!AnimatorController.GetCurrentAnimatorStateInfo(0).IsName("sword attack") && _canAttack && !_isDead)
        {
            StartCoroutine("CoolDown");
            if (CheckAttackEnemy(ref _closestEnemy))
            {
                _closestEnemy.Hp -= _damage;
            }
            AnimatorController.SetTrigger("Attack");
        }
    }

    IEnumerator CoolDown()
    {
        _canAttack = false;
        yield return new WaitForSeconds(0.5f);
        _canAttack = true;
    }

    private bool CheckAttackEnemy(ref Enemie closestEnemie)
    {
        var enemies = SceneManager.Instance.Enemies;

        for (int i = 0; i < enemies.Count; ++i)
        {
            var enemie = enemies[i];
            if (enemie == null)
            {
                continue;
            }

            if (closestEnemie == null)
            {
                closestEnemie = enemie;
                continue;
            }

            var distance = Vector3.Distance(transform.position, enemie.transform.position);
            var closestDistance = Vector3.Distance(transform.position, closestEnemie.transform.position);

            if (distance < closestDistance)
            {
                closestEnemie = enemie;
            }
        }

        if (closestEnemie != null)
        {
            var distance = Vector3.Distance(transform.position, closestEnemie.transform.position);
            if (distance <= _attackRange)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        return false;
    }

    public void SuperAttack()
    {
        if (_canSuperAttack)
        {
            StartCoroutine("SuperCoolDown");
            if (CheckAttackEnemy(ref _closestEnemy))
            {
                _closestEnemy.Hp -= _damage * 2;//double damage from super attack
                SuperAttackButton.onSuperAttackPressed?.Invoke();
            }
            AnimatorController.SetTrigger("SuperAttack");
        }
    }

    
    IEnumerator SuperCoolDown()
    {
        _canSuperAttack = false;
        yield return new WaitForSeconds(1.5f);
        _canSuperAttack = true;

    }

    public void Die()
    {
        if (_isDead)
        {
            return;
        }
        
        AnimatorController.SetTrigger("Die");
        AnimatorController.SetBool("Dead", true);
        _isDead = true;

        SuperAttackButton.onDisableButton?.Invoke();
    }
}
