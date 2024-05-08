using StarterAssets;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    public event EventHandler<OnSelectedInteractionChangedEventArgs> OnSelectedInteractionChanged;
    public class OnSelectedInteractionChangedEventArgs : EventArgs
    {
        public NPC selectedInteraction;
    }


    [SerializeField] private Animator animator;
    private const string IS_WALKING = "isWalking";
    private const string IS_BLOCKING = "isBlocking";

    private StarterAssetsInputs movementController;
    private FirstPersonController firstPersonController;
    public CharacterController controller;
    private List<EnemyBehaviour> enemies = new List<EnemyBehaviour>();
    EnemyBoss enemyBoss;

    [Header("Health, Mana & Level System")]
    public HealthSystem healthSystem;
    public HealthBar healthBar;
    public ManaSystem manaSystem;
    public ManaBar manaBar;
    public LevelSystem levelSystem;
    public LevelWindow levelWindow;
    private PlayerSkills playerSkills;

    [Header("Potion System")]
    [SerializeField] private int runes;
    [SerializeField] private TextMeshProUGUI runeText;

    [Header("Weapon & Slashes")]
    public List<Slash> slashes;
    [SerializeField] private int slashType;

    private BoxCollider swordCollider;
    public GameObject swordObject;

    [SerializeField] private GameObject respawnParticle;

    [SerializeField]
    public int clickCount = 0;
    private float nextFireTime = 0;
    private float lastClickedTime = 0;
    private float maxComboDelay = 0.41f;

    [SerializeField] private GameObject Sword;
    private bool isEquipped = false;
    private bool unlockedSword = false;

    private float turnSmoothVelocity;
    private float turnSmoothTime = 0.1f;
    public float speed = 15f;

    public AudioSource audioSource;
    public AudioClip slashSound;
    public AudioClip LightOrbSound;

    [Header("Checkpoint")]
    public Checkpoint cp;

    [Header("Player Settings")]
    [SerializeField] private bool CanAttack;
    public bool isDead;
    private bool isAttacking = false;
    private bool isCasting = false;
    private bool isBlocking = false;
    [SerializeField] private Vector3 respawnPoint;

    [SerializeField]
    private float playerDEF = 40;
    public float playerBaseDmg = 10;
    public float playerSpellDmg;

    private float invincibleTime = 0.5f;
    private float dodgeCooldown = 0.8f;
    private float ActCooldown = 0.8f;
    private bool isInvincible = false;

    [Header("Light Orb Settings")]
    public Transform playerDirection;
    public Transform attackPoint;
    public GameObject objectToThrow;
    public float throwCooldown;
    public float throwForce;
    [SerializeField] private bool readyToThrow;

    [Header("Holy Beam Settings")]
    public bool beamIsActivated = false;

    [Header("Summon Wolf Settings")]
    private WolfAtkTrigger wolfAtk;
    private WolfAudio wolfAudio;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private LayerMask npcLayerMask;
    [SerializeField] private Vector3 moveDirection;

    private bool isWalking;
    private Vector3 lastInteractDir;
    private NPC selectedInteraction;

    [Header("References")]
    [SerializeField] private Transform orientation;
    public Rigidbody rb;

    private static List<GameObject> PickAble = new List<GameObject>();
    private GameObject nearestObject = null;

    private void Awake()
    {
        Instance = this;
        levelSystem = GetComponent<LevelSystem>();
        levelWindow.SetLevelSystem(levelSystem);
        playerSkills = new PlayerSkills();
    }

    // Start is called before the first frame update
    void Start()
    {
        firstPersonController = FindObjectOfType<FirstPersonController>();
        movementController = FindObjectOfType<StarterAssetsInputs>();
        controller = GetComponent<CharacterController>();
        healthSystem = GetComponent<HealthSystem>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        wolfAtk = GetComponent<WolfAtkTrigger>();
        wolfAudio = GetComponent<WolfAudio>();
        manaSystem = GetComponent<ManaSystem>();
        cp = GetComponent<Checkpoint>();
        enemyBoss = FindObjectOfType<EnemyBoss>();

        gameInput.OnInteractAction += GameInput_OnInteractAction;

        //gameObject.GetComponent<HealthSystem>().Damage(98);
        //Debug.Log(gameObject.GetComponent<HealthSystem>().GetHealth());

        GameObject[] enemyObjects = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemyObject in enemyObjects)
        {
            EnemyBehaviour enemyScript = enemyObject.GetComponent<EnemyBehaviour>();
            if (enemyScript != null)
            {
                enemies.Add(enemyScript);
            }
        }

        wolfAtk.enabled = false;
        wolfAudio.enabled = false;

        swordCollider = swordObject.GetComponent<BoxCollider>();
        swordCollider.enabled = false;

        
        healthBar.Setup(healthSystem);
        manaBar.Setup(manaSystem);

        CanAttack = true;
        readyToThrow = true;


        DisableSlash();
    }

    private void Update()
    {
        //===================================== HEALTH ===================================== //
        if (healthSystem.GetHealth() == 0 && CanAttack)
        {
            StartCoroutine(Respawning());
        }

        //===================================== ATTACK ===================================== //

        if (CanAttack)
        {
            #region Auto Aiming Attack
            //if (isAttacking)
            //{
            //    foreach (EnemyBehaviour enemy in enemies)
            //    {
            //        GameObject nearestEnemy = FindNearestEnemy();
            //        if (nearestEnemy != null)
            //        {
            //            // Look at the nearest enemy when attacking
            //            Vector3 lookAtDir = nearestEnemy.transform.position - transform.position;
            //            lookAtDir.y = 0; // Keep the rotation in the horizontal plane
            //            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookAtDir), Time.deltaTime * 10f);
            //        }
            //    }
            //}
            #endregion

            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.4f && animator.GetCurrentAnimatorStateInfo(0).IsName("hit1"))
            {
                animator.SetBool("hit1", false);
            }
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.4f && animator.GetCurrentAnimatorStateInfo(0).IsName("hit2"))
            {
                animator.SetBool("hit2", false);
            }
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.4f && animator.GetCurrentAnimatorStateInfo(0).IsName("hit3"))
            {
                animator.SetBool("hit3", false);
            }
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.6f && animator.GetCurrentAnimatorStateInfo(0).IsName("hit4"))
            {
                animator.SetBool("hit4", false);
                clickCount = 0;
            }
        }

        if (Time.time - lastClickedTime >= maxComboDelay)
        {
            clickCount = 0;
            slashType = 0;
            isAttacking = false;
        }

        if (Input.GetMouseButtonDown(0) && !beamIsActivated)
        {
            OnClick();
        }

        //===================================== MOVEMENT ===================================== //

        Vector2 theInputValue = movementController.MoveInputValue();
        if (theInputValue.x > 0)
        {
            animator.SetFloat("Xvelocity", 1);
        }
        else if (theInputValue.x < 0)
        {
            animator.SetFloat("Xvelocity", -1);
        }
        else if (theInputValue.x == 0)
        {
            animator.SetFloat("Xvelocity", 0);
        }

        if (theInputValue.y > 0)
        {
            animator.SetFloat("Yvelocity", 1);
        }
        else if (theInputValue.y < 0)
        {
            animator.SetFloat("Yvelocity", -1);
        }
        else if (theInputValue.y == 0)
        {
            animator.SetFloat("Yvelocity", 0);
        }

        if (theInputValue.y == 1 && Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(StartInvincible(invincibleTime));
            animator.SetTrigger("EvadeForward");
        }
        else if (theInputValue.y == -1 && Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(StartInvincible(invincibleTime));
            animator.SetTrigger("EvadeBackward");
        }

        if (theInputValue.x == 1 && Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(StartInvincible(invincibleTime));
            animator.SetTrigger("EvadeRight");
        }
        else if (theInputValue.x == -1 && Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(StartInvincible(invincibleTime));
            animator.SetTrigger("EvadeLeft");
        }


        //===================================== BLOCK ===================================//
        if (Input.GetMouseButtonDown(1))
        {
            if (isWalking)
            {
                isWalking = false;
                animator.SetBool(IS_WALKING, isWalking);
            }

            isBlocking = true;
            animator.SetBool(IS_BLOCKING, true);
        }
        else if (Input.GetMouseButtonUp(1))
        {
            animator.SetBool(IS_BLOCKING, false);
            isBlocking = false;
        }

        //===================================== INTERACTION ===================================//
        HandleInteractions();
        #region Character Controller Movement
        //if (CanMove && !isCasting && !isAttacking)
        //{

        //float horizontalInput = Input.GetAxisRaw("Horizontal");
        //float verticalInput = Input.GetAxisRaw("Vertical");
        //Vector3 direction = new Vector3(horizontalInput, 0f, verticalInput).normalized;

        //if (direction.magnitude >= 0.1f)
        //{
        //    animator.SetBool("isWalking", true);
        //    clickCount = 0;
        //    slashType = 0;

        //    float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        //    float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
        //    transform.rotation = Quaternion.Euler(0f, angle, 0f);

        //    Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        //    controller.Move(moveDir.normalized * speed * Time.deltaTime);

        //}
        //else
        //{
        //    animator.SetBool("isWalking", false);
        //}
        //}
        #endregion

        //===================================== SKILLS ===================================== //
        if (Input.GetKeyDown(KeyCode.Q) && readyToThrow && CanUseLightOrb() && !isCasting && !isAttacking && manaSystem.GetMana() > 0)
        {
            readyToThrow = false;
            isCasting = true;
            isAttacking = false;
            manaSystem.Consume(30);
            StartCoroutine(CastingLightOrb());
        }

        if (Input.GetKeyDown(KeyCode.E) && (CanUseHolyBeam() || CanUseDarkBeam()) && !isCasting && !isAttacking)
        {
            if (!beamIsActivated)
            {
                Debug.Log("Beam Is Activated");
                beamIsActivated = true;
            }
            else if (beamIsActivated)
            {
                Debug.Log("Beam Is Inactivated");
                beamIsActivated = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.Q) && readyToThrow && CanUseSummonWolf() && !isCasting && !isAttacking && manaSystem.GetMana() > 0)
        {
            isCasting = true;
            isAttacking = false;
            wolfAtk.enabled = true;
            wolfAudio.enabled = true;
            StartCoroutine(SummonWolf());
        }

        //===================================== HEALTH RUNES ================================//
        if (Input.GetKeyDown(KeyCode.R) && runes != 0)
        {
            runes--;
            runeText.SetText("Runes: " + runes);
            healthSystem.Heal(35);
        }
    }

    private void LateUpdate()
    {
        if (firstPersonController.IsWalking())
        {
            isWalking = true;
            animator.SetBool(IS_WALKING, firstPersonController.IsWalking());

        }
        else
        {
            isWalking = false;
            animator.SetBool(IS_WALKING, firstPersonController.IsWalking());
        }
    }



    // Player Mechanic 
    IEnumerator StartInvincible(float invincibleLength)
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibleLength);
        isInvincible = false;
        yield return new WaitForSeconds(0.5f);

    }
    IEnumerator CastingLightOrb()
    {
        animator.SetBool("isCasting", true);
        yield return new WaitForSeconds(0.5f);
        Throw();
        audioSource.PlayOneShot(LightOrbSound);
        animator.SetBool("isCasting", false);
        isCasting = false;
        isAttacking = true;

    }

    IEnumerator SummonWolf()
    {
        animator.SetBool("isCasting", true);
        yield return new WaitForSeconds(0.5f);
        animator.SetBool("isCasting", false);
        isCasting = false;
        isAttacking = true;
    }

    IEnumerator Respawning()
    {
        CanAttack = false;
        isDead = true;
        controller.enabled = false;  // Disable the CharacterController temporarily
        animator.SetTrigger("isDead");
        // Wait for the dead animation to complete before respawning
        yield return new WaitForSeconds(3.5f);

        controller.transform.position = cp.GetCheckPoint();
        controller.enabled = true;   // Re-enable the CharacterController
        respawnParticle.SetActive(true);

        healthSystem.SetHealth(healthSystem.GetMaxHealth() / 2);
        CanAttack = true;
        isDead = false;

        yield return new WaitForSeconds(3);
        respawnParticle.SetActive(false);
    }
    IEnumerator SlashAttack()
    {
        //for (int i=0; i<slashes.Count; i++) 
        //{
        //    yield return new WaitForSeconds(slashes[i].delay);
        //    slashes[i].slashObj.SetActive(true);
        //}

        if (slashType == 1)
        {
            yield return new WaitForSeconds(slashes[0].delay);
            slashes[0].slashObj.SetActive(true);
            yield return new WaitForSeconds(0.4f);
            DisableSlash();
        }
        else if (slashType == 2)
        {
            yield return new WaitForSeconds(slashes[1].delay);
            slashes[1].slashObj.SetActive(true);
            yield return new WaitForSeconds(0.4f);
            DisableSlash();
        }
        else if (slashType == 3)
        {
            yield return new WaitForSeconds(slashes[2].delay);
            slashes[2].slashObj.SetActive(true);
            yield return new WaitForSeconds(0.4f);
            DisableSlash();
        }
        else if (slashType == 4)
        {
            yield return new WaitForSeconds(slashes[3].delay);
            slashes[3].slashObj.SetActive(true);
            yield return new WaitForSeconds(0.4f);
            DisableSlash();
        }



    }
    public void DisableSlash()
    {
        for (int i = 0; i < slashes.Count; i++)
        {
            slashes[i].slashObj.SetActive(false);
        }
    }

    void OnClick()
    {
        isWalking = false;

        isAttacking = true;
        clickCount++;
        lastClickedTime = Time.time;

        if (clickCount == 1)
        {
            slashType = 1;
            animator.SetBool("hit1", true);
        }
        clickCount = Mathf.Clamp(clickCount, 0, 4);


        if (clickCount >= 2 && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.4f && animator.GetCurrentAnimatorStateInfo(0).IsName("hit1"))
        {
            slashType = 2;
            animator.SetBool("hit1", false);
            animator.SetBool("hit2", true);

        }
        if (clickCount >= 3 && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.6f && animator.GetCurrentAnimatorStateInfo(0).IsName("hit2"))
        {
            slashType = 3;
            animator.SetBool("hit2", false);
            animator.SetBool("hit3", true);
        }
        if (clickCount >= 4 && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.5f && animator.GetCurrentAnimatorStateInfo(0).IsName("hit3"))
        {
            slashType = 4;
            animator.SetBool("hit3", false);
            animator.SetBool("hit4", true);
        }


    }
    public void StartAttack()
    {
        audioSource.PlayOneShot(slashSound);
        manaSystem.RecoverMana(10);
        swordCollider.enabled = true;
        StartCoroutine(SlashAttack());
    }

    public void EndAttack()
    {
        swordCollider.enabled = false;
        foreach (EnemyBehaviour enemy in enemies)
        {
            enemy.isHit = false;
        }
    }


    // Check Player Actions
    public bool IsAttacking()
    {
        return isAttacking;
    }

    public bool IsBlocking()
    {
        return isBlocking;
    }

    public bool IsCasting()
    {
        return isCasting;
    }

    public void SetBoolCasting(bool value)
    {
        isCasting = value;
    }


    // Get Player Attributes 
    public float GetPlayerDamage()
    {
        return playerBaseDmg;
    }
    public void SetPlayerDamage(float value)
    {
        playerBaseDmg += value;
    }

    public float GetPlayerDefense()
    {
        return playerDEF;
    }
    public void SetPlayerDefense(float value)
    {
        playerDEF += value;
    }

    public float GetPlayerSpellDamage()
    {
        return playerBaseDmg * 2;
    }


    // Receive Reward from NPC
    public void ReceiveReward(int value)
    {
        runes += value;
        runeText.SetText("Runes: " + runes);
    }

    // Change Weapon
    public void ChangeWeapon(GameObject sword)
    {
        swordObject = sword;
        swordCollider = swordObject.GetComponent<BoxCollider>();
        swordCollider.enabled = false;
        Debug.Log(swordObject);
    }
    private GameObject FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length == 0)
        {
            return null; // No enemies found
        }

        GameObject nearestEnemy = null;
        float nearestDistance = float.MaxValue;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < nearestDistance)
            {
                nearestEnemy = enemy;
                nearestDistance = distance;
            }
        }

        return nearestEnemy;
    }

    // Player Movement and Interaction
    private void GameInput_OnInteractAction(object sender, System.EventArgs e)
    {
        if (selectedInteraction != null)
        {
            selectedInteraction.Interact();
        }
    }

    private void HandleInteractions()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();

        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        if (moveDir != Vector3.zero)
        {
            lastInteractDir = moveDir;
        }

        float interactDistance = 4f;
        // Check if the player is moving or if the last interact direction is non-zero
        if (isWalking || lastInteractDir != Vector3.zero)
        {
            if (Physics.Raycast(transform.position, lastInteractDir, out RaycastHit raycastHit, interactDistance, npcLayerMask))
            {
                if (raycastHit.transform.TryGetComponent(out NPC npc))
                {
                    // is NPC
                    if (npc != selectedInteraction)
                    {
                        SetSelectedInteraction(npc);
                    }
                }
                else
                {
                    SetSelectedInteraction(null);
                }
            }
            else
            {
                SetSelectedInteraction(null);
            }
        }
        else
        {
            SetSelectedInteraction(null);
        }

        //Debug.Log(selectedInteraction);
    }

    private void HandleMovement()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();

        moveDirection = orientation.forward * inputVector.y + orientation.right * inputVector.x;
        rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        #region Old HandleMovement()
        //float moveDistance = moveSpeed * Time.deltaTime;
        //float playerRadius = .7f;
        //float playerHeight = 2f;


        //bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDir, moveDistance);

        //if (!canMove)
        //{
        //    // Cannot move towards moveDir

        //    // Attempt only X movement
        //    Vector3 moveDirX = new Vector3(moveDir.x, 0, 0).normalized;
        //    canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirX, moveDistance);

        //    if (canMove)
        //    {
        //        // Can move only on the x
        //        moveDir = moveDirX;
        //    }
        //    else
        //    {
        //        // Cannot move only on the x

        //        // Attempt only z movement
        //        Vector3 moveDirZ = new Vector3(0, 0, moveDir.z).normalized;
        //        canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirZ, moveDistance);

        //        if (canMove)
        //        {
        //            // Can move only on the z
        //            moveDir = moveDirZ;
        //        }
        //        else
        //        {
        //            // Cannot move in any direction
        //        }
        //    }
        //}

        //if (canMove)
        //{
        //    transform.position += moveDir * moveDistance;
        //}
        //float rotateSpeed = 10f;
        //transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);
        #endregion

        isWalking = moveDirection.magnitude > 0.2f && !isCasting && !isBlocking;
    }

    private void SetSelectedInteraction(NPC selectedInteraction)
    {
        this.selectedInteraction = selectedInteraction;

        // OnSelectedInteractionChanged is from this class
        OnSelectedInteractionChanged?.Invoke(this, new OnSelectedInteractionChangedEventArgs
        {
            selectedInteraction = selectedInteraction
        }); ;

    }


    // Calculate Damage Received from Enemy
    private int DamageCalculation(float theDamage)
    {
        float reductionFactor = 0.05f;
        int reducedDamage = Mathf.Max(1, Mathf.RoundToInt(theDamage - playerDEF * reductionFactor));
        return reducedDamage;
    }



    // ========================== Collider System ========================== //
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "PickAble" || other.gameObject.tag == "HealPotion")
        {
            if (nearestObject == null)
            {
                nearestObject = other.gameObject;
                nearestObject.GetComponent<PickUpObject>().text.SetActive(true);
            }
            else if (nearestObject != null)
            {

            }
        }

        if (other.gameObject.tag == "EnemySkill")
        {
            healthSystem.Damage(enemyBoss.GetEnemyBossDamage());
            Vector3 random = new Vector3(Random.Range(-2f, 2f), Random.Range(0, 2f), Random.Range(-2f, 2f));
            DamagePopUpGenerator.current.CreatePopUp(transform.position + random, enemyBoss.GetEnemyBossDamage().ToString());
        }

        if (other.gameObject.tag == "EnemyWeapon")
        {
            foreach (EnemyBehaviour enemy in enemies)
            {
                // Check if the collider's GameObject is in the enemies list
                if (enemy != null && other.gameObject.transform.root != null)
                {
                    if (isBlocking && manaSystem.GetMana() > 0)
                    {
                        manaSystem.Consume(15);
                        Vector3 random = new Vector3(Random.Range(-2f, 2f), Random.Range(0, 2f), Random.Range(-2f, 2f));
                        DamagePopUpGenerator.current.CreatePopUp(transform.position + random, "Blocked");
                    }
                    else if (isBlocking && manaSystem.GetMana() <= 0)
                    {
                        float calculatedDamage = DamageCalculation(enemy.GetEnemyDamage());
                        healthSystem.Damage(calculatedDamage);
                        Vector3 random = new Vector3(Random.Range(-2f, 2f), Random.Range(0, 2f), Random.Range(-2f, 2f));
                        DamagePopUpGenerator.current.CreatePopUp(transform.position + random, calculatedDamage.ToString());
                    }

                    if (!isInvincible && !isBlocking)
                    {
                        float calculatedDamage = DamageCalculation(enemy.GetEnemyDamage());
                        healthSystem.Damage(calculatedDamage);
                        Vector3 random = new Vector3(Random.Range(-2f, 2f), Random.Range(0, 2f), Random.Range(-2f, 2f));
                        DamagePopUpGenerator.current.CreatePopUp(transform.position + random, calculatedDamage.ToString());
                    }
                    else if (isInvincible)
                    {
                        Vector3 random = new Vector3(Random.Range(-2f, 2f), Random.Range(0, 2f), Random.Range(-2f, 2f));
                        DamagePopUpGenerator.current.CreatePopUp(transform.position + random, "Dodged");
                    }
                    
                    
                    break; // Exit the loop after processing the damage for the specific enemy
                }
            }
        }

        if (other.gameObject.tag == "GolemATK")
        {
            if (isBlocking)
            {
                manaSystem.Consume(15);
                Vector3 random = new Vector3(Random.Range(-2f, 2f), Random.Range(0, 2f), Random.Range(-2f, 2f));
                DamagePopUpGenerator.current.CreatePopUp(transform.position + random, "Blocked");
            }

            if (!isInvincible)
            {
                int calculatedDamage = DamageCalculation(10);
                healthSystem.Damage(calculatedDamage);
                Vector3 random = new Vector3(Random.Range(-2f, 2f), Random.Range(0, 2f), Random.Range(-2f, 2f));
                DamagePopUpGenerator.current.CreatePopUp(transform.position + random, calculatedDamage.ToString());
            }
            else if (isInvincible)
            {
                Vector3 random = new Vector3(Random.Range(-2f, 2f), Random.Range(0, 2f), Random.Range(-2f, 2f));
                DamagePopUpGenerator.current.CreatePopUp(transform.position + random, "Dodged");
            }
            

        }

        if (other.gameObject.tag == "BossWeapon")
        {
            if (isBlocking && manaSystem.GetMana() > 0)
            {
                manaSystem.Consume(15);
                Vector3 random = new Vector3(Random.Range(-2f, 2f), Random.Range(0, 2f), Random.Range(-2f, 2f));
                DamagePopUpGenerator.current.CreatePopUp(transform.position + random, "Blocked");
            }
            else if (isBlocking && manaSystem.GetMana() <= 0)
            {
                float calculatedDamage = DamageCalculation(enemyBoss.GetEnemyBossDamage());
                healthSystem.Damage(calculatedDamage);
                Vector3 random = new Vector3(Random.Range(-2f, 2f), Random.Range(0, 2f), Random.Range(-2f, 2f));
                DamagePopUpGenerator.current.CreatePopUp(transform.position + random, calculatedDamage.ToString());
            }

            if (!isInvincible && !isBlocking)
            {
                float calculatedDamage = DamageCalculation(enemyBoss.GetEnemyBossDamage());
                healthSystem.Damage(calculatedDamage);
                Vector3 random = new Vector3(Random.Range(-2f, 2f), Random.Range(0, 2f), Random.Range(-2f, 2f));
                DamagePopUpGenerator.current.CreatePopUp(transform.position + random, calculatedDamage.ToString());
            }
            else if (isInvincible)
            {
                Vector3 random = new Vector3(Random.Range(-2f, 2f), Random.Range(0, 2f), Random.Range(-2f, 2f));
                DamagePopUpGenerator.current.CreatePopUp(transform.position + random, "Dodged");
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (nearestObject != null)
        {
            if (Input.GetKey(KeyCode.F))
            {
                nearestObject.GetComponent<PickUpObject>().PickedUp();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "PickAble")
        {
            nearestObject = other.gameObject;
            nearestObject.GetComponent<PickUpObject>().text.SetActive(false);
            nearestObject = null;
            //onCollide = false;
        }
    }




    // Player Skills Management
    #region Player Skills

    public PlayerSkills GetPlayerSkill()
    {
        return playerSkills;
    }




    // Dark Skills //
    public bool CanUseSummonWolf()
    { return playerSkills.IsSkillUnlocked(PlayerSkills.SkillType.SummonWolf); }

    public bool CanUseDarkBeam()
    { return playerSkills.IsSkillUnlocked(PlayerSkills.SkillType.DarkBeam); }


    // Light Skills //
    public bool CanUseLightOrb()
    { return playerSkills.IsSkillUnlocked(PlayerSkills.SkillType.LightOrb); }

    private void Throw()
    {
        //instantiate object to throw
        GameObject projectile = Instantiate(objectToThrow, attackPoint.position, playerDirection.rotation);
        //get rigidbody component
        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();

        //calculate direction
        Vector3 forceDirection = playerDirection.transform.forward;

        RaycastHit hit;

        if (Physics.Raycast(playerDirection.position, playerDirection.forward, out hit, 500f))
        {
            forceDirection = (hit.point - attackPoint.position).normalized;
        }

        //add force
        Vector3 forceToAdd = forceDirection * throwForce;
        projectileRb.AddForce(forceToAdd, ForceMode.Impulse);

        //calculate direction
        Vector3 forcedirection = playerDirection.transform.forward;

        //cooldown
        Invoke(nameof(ResetThrow), throwCooldown);
    }

    private void ResetThrow()
    {
        readyToThrow = true;
    }





    public bool CanUseHolyBeam()
    { return playerSkills.IsSkillUnlocked(PlayerSkills.SkillType.HolyBeam); }
    #endregion



    // Slash Particle Control
    [System.Serializable]
    public class Slash
    {
        public GameObject slashObj;
        public float delay;
    }

}
