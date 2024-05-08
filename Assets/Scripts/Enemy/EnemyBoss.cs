using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.ProBuilder;
using UnityEngine.SceneManagement;

public class EnemyBoss : MonoBehaviour
{
    private enum State
    {
        Idle,
        Chase,
        Attack,
        Return,
        Dead,
    }

    private State state;
    private Vector3 startingPosition;

    [SerializeField] private GameObject target;
    [SerializeField] private GameObject weapon;
    [SerializeField] private GameObject DarkOrb;
    [SerializeField] private Collider weapon_collider;

    private PlayerController playerManager;
    private HealthSystem healthSystem;
    private NavMeshAgent agent;
    private Animator animator;
    private AudioSource audioSource;
    public AudioClip attackSound;
    public AudioClip spellCastSound;

    [SerializeField] private float enemyDmg = 30;
    [SerializeField] private float enemyDetectRange = 5f;
    [SerializeField] private float meeleAttackRange = 3f;
    [SerializeField] private float cooldownCloseAttack = 0;
    [SerializeField] private float cooldownLongAttack = 0;

    [SerializeField] private bool UseCombo = false;
    [SerializeField] private bool CastingSpell = false;
    [SerializeField] private bool Attacking = false;
    [SerializeField] private bool isDead = false;
    [SerializeField] private bool isHit = false;

    private float rotationSpeed = 180;
    private bool isReturning = false;
    private bool isAttacking = false;
    public int DmgReceive;
    public float throwForce;
    public GameObject castPoint;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        healthSystem = GetComponent<HealthSystem>();
        weapon_collider = weapon.GetComponent<BoxCollider>();
        target = GameObject.FindGameObjectWithTag("Player");
        playerManager = FindObjectOfType<PlayerController>();
        audioSource = GetComponent<AudioSource>();

        startingPosition = gameObject.transform.position;
        state = State.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        cooldownCloseAttack = Mathf.Clamp(cooldownCloseAttack - Time.deltaTime, 0, 10);
        cooldownLongAttack = Mathf.Clamp(cooldownLongAttack - Time.deltaTime, 0, 20);
        FaceTarget();

        if (healthSystem.GetHealth() == 0 && !isDead)
        {
            StartCoroutine(StartDecaying());
        }


        if (playerManager.isDead && !isDead)
        {
            isReturning = true;
            state = State.Return;
        }
        else if (isDead)
        {
            state = State.Dead;
        }
        else if (Vector3.Distance(transform.position, target.transform.position) < enemyDetectRange && !playerManager.isDead && !isDead)
        {
            if (!isReturning)
            {
                if (!isAttacking && Vector3.Distance(transform.position, target.transform.position) > meeleAttackRange)
                {
                    if (!UseCombo && !CastingSpell)
                    {
                        state = State.Chase;
                    }
                }

                if (!isAttacking && Vector3.Distance(transform.position, target.transform.position) > 10)
                {
                    if (cooldownLongAttack <= 0 && !UseCombo && !Attacking)
                    {
                        StartCoroutine(LongRangeSkill());

                        //Set Cooldown
                        cooldownLongAttack = 20;
                    }
                }

                if (Vector3.Distance(transform.position, target.transform.position) < meeleAttackRange)
                {
                    state = State.Attack;
                }
            }
        }






        switch (state)
        {
            case State.Idle:
                animator.SetBool("IsIdle", true);
                break;


            case State.Chase:
                agent.isStopped = false;
                agent.SetDestination(target.transform.position);
                animator.SetBool("IsChasing", true);
                break;


            case State.Attack:
                if (cooldownCloseAttack <= 0 && !CastingSpell && !Attacking)
                {
                    StartCoroutine(CloseRangeSkill());

                    //Set Cooldown
                    cooldownCloseAttack = 10;
                }
                else
                {
                    StartCoroutine(BaseAttack());
                }
                break;


            case State.Return:
                animator.SetBool("IsChasing", true);
                agent.isStopped = false;
                agent.SetDestination(startingPosition);
                if (Vector3.Distance(transform.position, startingPosition) < 0.5f)
                {
                    isReturning = false;
                    agent.isStopped = true;
                    animator.SetBool("IsChasing", false);
                    animator.SetBool("IsIdle", true);
                    state = State.Idle;
                }
                break;

            case State.Dead:
                agent.isStopped = true;
                break;
        }
    }

    IEnumerator CloseRangeSkill()
    {
        agent.isStopped = true;
        UseCombo = true;

        animator.SetTrigger("UseCombo");

        yield return new WaitForSeconds(3.5f);
        UseCombo= false;
        agent.isStopped = false;
        animator.ResetTrigger("UseCombo");
    }

    IEnumerator LongRangeSkill()
    {
        agent.isStopped = true;
        CastingSpell = true;

        animator.SetTrigger("IsCasting");

        yield return new WaitForSeconds(3.5f);
        CastingSpell = false;
        agent.isStopped = false;
        animator.ResetTrigger("IsCasting");
    }

    IEnumerator BaseAttack()
    {
        agent.isStopped = true;
        Attacking = true;

        animator.SetTrigger("IsAttacking");

        yield return new WaitForSeconds(1.5f);
        Attacking = false;
        agent.isStopped = false;
        animator.ResetTrigger("IsAttacking");
    }

    IEnumerator StartDecaying()
    {
        isDead = true;
        agent.isStopped = true;

        animator.SetBool("IsAttacking", false);
        animator.SetBool("isChasing", false);
        animator.SetBool("IsIdle", false);

        animator.SetTrigger("IsDead");
        yield return new WaitForSeconds(5f);

        SceneManager.LoadScene("GameOver");
    }

    public void BossStartAttack()
    {
        audioSource.PlayOneShot(attackSound);
        weapon_collider.enabled = true;
    }

    public void BossEndAttack()
    {
        weapon_collider.enabled = false;
    }

    public void CastDarkOrb()
    {
        audioSource.PlayOneShot(spellCastSound);
        GameObject darkOrb = Instantiate(DarkOrb, castPoint.transform.position, Quaternion.identity);
        Rigidbody projectileRb = darkOrb.GetComponent<Rigidbody>();

        Vector3 forceToAdd = gameObject.transform.forward * throwForce;
        projectileRb.AddForce(forceToAdd, ForceMode.Impulse);
    }

    void FaceTarget()
    {
        Vector3 direction = (target.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    public float GetEnemyBossDamage()
    {
        return enemyDmg;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Weapon" && !isHit)
        {
            //DmgReceive = 1;
            isHit = true;
            healthSystem.Damage(playerManager.GetPlayerDamage());
            Vector3 random = new Vector3(Random.Range(0f, 3f), Random.Range(0f, 0.5f), Random.Range(0f, 0.5f));
            DamagePopUpGenerator.current.CreatePopUp(transform.position + random, playerManager.GetPlayerDamage().ToString());
        }
        if (other.gameObject.tag == "PlayerSpells")
        {
            healthSystem.Damage(playerManager.GetPlayerSpellDamage());
            Vector3 random = new Vector3(Random.Range(0f, 3f), Random.Range(0f, 0.5f), Random.Range(0f, 0.5f));
            DamagePopUpGenerator.current.CreatePopUp(transform.position + random, playerManager.GetPlayerSpellDamage().ToString());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Weapon" && isHit)
        {
            isHit = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "PlayerSpells")
        {
            healthSystem.Damage(playerManager.GetPlayerSpellDamage());
            Vector3 random = new Vector3(Random.Range(0f, 3f), Random.Range(0f, 0.5f), Random.Range(0f, 0.5f));
            DamagePopUpGenerator.current.CreatePopUp(transform.position + random, playerManager.GetPlayerSpellDamage().ToString());
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, enemyDetectRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, meeleAttackRange);
    }
}
