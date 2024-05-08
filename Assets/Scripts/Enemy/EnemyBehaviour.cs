using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class EnemyBehaviour : MonoBehaviour
{
    // Behaviour State
    private enum State
    {
        Idle,
        Chase,
        Attack,
        Return,
        Dead,
    }

    private State state;

    // Quest Settings
    public QuestHandler quest;

    // TriggerBox Settings
    public TriggerBox triggerBox;

    // Initial position
    private Vector3 startingPosition;

    // Player target
    private GameObject target;

    // Enemy AI Weapon
    [SerializeField] private GameObject weapon;
    Collider weapon_collider;
    CapsuleCollider character_collider;

    //References
    private PlayerController playerManager;
    private HealthSystem healthSystem;
    private NavMeshAgent agent;
    private Animator animator;
    private AudioSource audioSource;
    public AudioClip attackSound;
    
    // Enemy AI Attributes
    [SerializeField] private int enemyDmg;
    private float rotationSpeed = 180;
    public float enemyDetectRange = 15f;
    [SerializeField] private float attackRange = 1.5f;
    private bool isReturning = false;
    private bool isAttacking = false;
    private bool isDead = false;
    public int DmgReceive;
    public bool isHit = false;


    // Start is called before the first frame update
    void Start()
    {
        // Get the reference
        target = GameObject.FindGameObjectWithTag("Player");
        playerManager = FindObjectOfType<PlayerController>();
        healthSystem = GetComponent<HealthSystem>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        weapon_collider = weapon.GetComponent<BoxCollider>();
        character_collider = GetComponent<CapsuleCollider>();
        audioSource = GetComponent<AudioSource>();

        // Set the initial value
        DmgReceive = 0;
        weapon_collider.enabled = false;
        state = State.Idle;
        startingPosition = transform.position;

    }

    // Update is called once per frame
    void Update()
    {
        if (healthSystem.GetHealth() == 0 && !isDead)
        {
            if (quest)
            {
                quest.enemiesLeft--;
            }
            
            if (triggerBox)
            {
                triggerBox.enemiesLeft--;
            }
            playerManager.levelSystem.AddExperience(500);
            StartCoroutine(StartDecaying());
        }


        if (playerManager.isDead && !isDead)
        {
            isReturning = true;
            state = State.Return;
        }
        else if(isDead)
        {
            state = State.Dead;
        }
        else if (Vector3.Distance(transform.position, target.transform.position) < enemyDetectRange && !playerManager.isDead && !isDead)
        {
            if (!isReturning)
            {
                if (!isAttacking && Vector3.Distance(transform.position, target.transform.position) > attackRange)
                {
                    state = State.Chase;
                }

                if (Vector3.Distance(transform.position, target.transform.position) < attackRange)
                {

                    state = State.Attack;
                }
            }
        }




        switch (state)
        {
            case State.Idle:
                animator.SetBool("isIdling", true);
                break;


            case State.Chase:
                if (playerManager.isDead)
                {
                    state = State.Return;
                }
                else if (!isDead)
                {
                    animator.SetBool("isIdling", false);
                    animator.SetBool("isAttacking", false);
                    animator.SetBool("isWalking", true);

                    agent.isStopped = false;
                    agent.SetDestination(target.transform.position);
                }
                else
                {
                    agent.isStopped = true;
                }

                if (Vector3.Distance(transform.position, target.transform.position) > enemyDetectRange)
                {
                    if (!isAttacking)
                    {
                        isReturning = true;
                        state = State.Return;
                    }
                }
                break;


            case State.Attack:
                if (isDead)
                {
                    animator.SetBool("isAttacking", false);
                    agent.isStopped = true;
                }
                else if (Vector3.Distance(transform.position, target.transform.position) > attackRange && !isAttacking)
                {
                    isReturning = true;
                    state = State.Return;
                }
                else if (!isDead)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(target.transform.position - transform.position);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
                    if (!isAttacking)
                    {
                        StartCoroutine(StartAttacking());
                    }
                }

                break;


            case State.Return:
                animator.SetBool("isWalking", true);
                agent.isStopped = false;
                agent.SetDestination(startingPosition);
                if (Vector3.Distance(transform.position, startingPosition) < 0.5f)
                {
                    isReturning = false;
                    agent.isStopped = true;
                    animator.SetBool("isWalking", false);
                    animator.SetBool("isIdling", true);
                    state = State.Idle;
                }
                break;

            case State.Dead:
                agent.isStopped = true;
                break;
        }
    }

    IEnumerator StartAttacking()
    {
        agent.isStopped = true;
        isAttacking = true;

        animator.SetBool("isWalking", false);
        animator.SetBool("isAttacking", true);

        yield return new WaitForSeconds(1);

        animator.SetBool("isAttacking", false);
        isAttacking = false;

    }
    IEnumerator StartDecaying()
    {
        isDead = true;
        agent.isStopped = true;
        character_collider.enabled = false;

        animator.SetBool("isAttacking", false);
        animator.SetBool("isWalking", false);
        animator.SetBool("isIdling", false);

        animator.SetTrigger("isDead");
        yield return new WaitForSeconds(2);
        
        Destroy(gameObject, 1f);
    }

    public void StartAttackTrigger()
    {
        audioSource.PlayOneShot(attackSound);
        weapon_collider.enabled = true;
    }

    public void EndAttackTrigger()
    {
        weapon_collider.enabled = false;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Weapon" && !isHit)
        {
            //DmgReceive = 1;
            isHit = true;
            healthSystem.Damage(playerManager.GetPlayerDamage());
            Vector3 random = new Vector3(Random.Range(0f, 0.5f), Random.Range(0f, 0.5f), Random.Range(0f, 0.5f));
            DamagePopUpGenerator.current.CreatePopUp(transform.position + random, playerManager.GetPlayerDamage().ToString());

        }
        else if (other.gameObject.tag == "PlayerSpells")
        {
            healthSystem.Damage(playerManager.GetPlayerSpellDamage());
            Vector3 random = new Vector3(Random.Range(0f, 0.5f), Random.Range(0f, 0.5f), Random.Range(0f, 0.5f));
            DamagePopUpGenerator.current.CreatePopUp(transform.position + random, playerManager.GetPlayerSpellDamage().ToString());
        }
    }

    private void OnTriggerExit(Collider other)
    {

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, enemyDetectRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }


    public int GetEnemyDamage()
    {
        return enemyDmg;
    }

}
