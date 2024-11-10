using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Horizontal Movement Settings: ")]
    [SerializeField] private float walkSpeed = 1; //sets the players movement speed on the ground
    [Space(5)]

    [Header("Vertical Movement Settings: ")]
    [SerializeField] private float jumpForce = 45f; //sets how hight the player can jump

    private int jumpBufferCounter = 0; //stores the jump button input
    [SerializeField] private int jumpBufferFrames; //sets the max amount of frames the jump buffer input is stored

    private float coyoteTimeCounter = 0; //stores the Grounded() bool
    [SerializeField] private float coyoteTime; //sets the max amount of frames the Grounded() bool is stored

    private int airJumpCounter = 0; //keeps track of how many times the player has jumped in the air
    [SerializeField] private int maxAirJumps; //the max no. of air jumps

    [SerializeField] private int maxFallingSpeed; //capping player's falling speed

    private float gravity; //stores the gravity scale at start
    [Space(5)]

    [Header("Wall Jump Settings: ")]
    [SerializeField] private float wallSlidingSpeed = 2f;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float wallJumpingDuration;
    [SerializeField] private Vector2 wallJumpingPower;
    float wallJumpingDirection;
    bool isWallSliding;
    bool isWallJumping;
    [Space(5)]


    [Header("Ground Check Settings: ")]
    [SerializeField] private Transform groundCheckPoint; //point at which ground check happens
    [SerializeField] private float groundCheckY = 0.2f; //how far down from ground check point is Grounded() checked
    [SerializeField] private float groundCheckX = 0.5f; //how far horizontally from ground check point to the edge of the player is

    [SerializeField] private LayerMask whatIsGround; //sets the ground layer
    [Space(5)]

    [Header("Dash Settings: ")]
    [SerializeField] private float dashSpeed; //speed of the dash
    [SerializeField] private float dashTime; //amount of time spent dashing
    [SerializeField] private float dashCooldown; //amount of time between dashes
    [SerializeField] GameObject dashEffect;
    [Space(5)]

    [Header("Attack Settings: ")]
    bool attack = false;
    float timeBetweenAttack, timeSinceAttack;

    [SerializeField] private Transform SideAttackTransform, UpAttackTransform, DownAttackTransform;  //the middle of the side/up/down attack area
    [SerializeField] private Vector2 SideAttackArea, UpAttackArea, DownAttackArea; //how large the area of side/up/down attack is

    [SerializeField] private LayerMask attackableLayer; //the layer the player can attack and recoil off of

    [SerializeField] private float damage; //the damage the player does to an enemy

    [SerializeField] private GameObject slashEffect; //the effect of the slashs
    bool restoreTime;
    float restoreTimeSpeed;
    [Space(5)]

    [Header("Recoil: ")]
    [SerializeField] private int recoilXSteps = 5; //how many FixedUpdates() the player recoils horizontally for
    [SerializeField] private int recoilYSteps = 5; //how many FixedUpdates() the player recoils vertically for

    [SerializeField] float recoilXSpeed = 100; //the speed of horizontal recoil
    [SerializeField] float recoilYSpeed = 100; //the speed of vertical recoil

    private int stepsXRecoiled, stepsYRecoiled; //the no. of steps recoiled horizontally and verticall
    [Space(5)]

    [Header("Health settings: ")]
    public int health;
    public int maxHealth;
    public int maxTotalHealth = 10;
    public int heartShards;


    [SerializeField] GameObject bloodSpurt;
    [SerializeField] float hitFlashSpeed;
    public delegate void OnHealthChangeDelegate();
    [HideInInspector] public OnHealthChangeDelegate onHealthChangeCallback;

    float healTimer;
    [SerializeField] float timeToHeal;
    [Space(5)]

    [Header("Mana settings: ")]
    [SerializeField] Image manaStorage;
    [SerializeField] float mana;
    [SerializeField] float manaDrainSpeed;
    [SerializeField] float manaGain;
    public bool halfMana;

    public ManaOrbsHandler manaOrbsHandler;
    public int orbShard;
    public int manaOrbs;
    [Space(5)]

    [Header("Spell cast settings: ")]
    // Spell stats
    [SerializeField] float manaSpellCost = 0.3f;
    [SerializeField] float timeBetweenCast = 0.5f;
    [SerializeField] float spellDamage; // upSpellExplosion and downSpellFireball
    [SerializeField] float downSpellForce;  // downSpellFireball only

    // Spell cast objects
    [SerializeField] GameObject sideSpellFireball;
    [SerializeField] GameObject upSpellExplosion;
    [SerializeField] GameObject downSpellFireball;

    float timeSinceCast;
    float castOrHealTimer;
    [Space(5)]

    [Header("Camera stuff: ")]
    [SerializeField] private float playerFallSpeedThrehold = -10;
    [Space(5)]

    [Header("Audio: ")]
    [SerializeField] AudioClip landingSound;
    [SerializeField] AudioClip jumpVoiceSound;
    [SerializeField] AudioClip dashSound;
    [SerializeField] AudioClip attackVoiceSound;
    [SerializeField] AudioClip spellCastSound;
    [SerializeField] AudioClip hurtSound;

    [HideInInspector] public PlayerStateList pState;
    Animator anim;
    public Rigidbody2D rb;
    private SpriteRenderer sr;
    private AudioSource audioSource;

    // Input variables
    private float xAxis, yAxis;
    private bool canDash = true;
    private bool dashed = true;
    bool openMap;
    bool openInventory;

    private bool canFlash = true;

    private bool landingSoundPlayed;


    public static PlayerController Instance;

    // Unlocking
    public bool unlockedWallJump;
    public bool unlockedDash;
    public bool unlockedVarJump;
    public bool unlockedSideCast;
    public bool unlockedUpCast;
    public bool unlockedDownCast;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        pState = GetComponent<PlayerStateList>();

        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        anim = GetComponent<Animator>();
        manaOrbsHandler = FindObjectOfType<ManaOrbsHandler>();

        gravity = rb.gravityScale;

        Mana = mana;
        manaStorage.fillAmount = Mana;

        Health = maxHealth;

        // SaveData.Instance.LoadPlayerData();
        SaveData.Instance.StartWithNewPlayerData();

        FindObjectOfType<HeartController>().InstantiateHeartContainers();

        if (halfMana)
        {
            UIManager.Instance.SwitchMana(UIManager.ManaState.HalfMana);
        }
        else
        {
            UIManager.Instance.SwitchMana(UIManager.ManaState.FullMana);
        }

        if (Health == 0)
        {
            pState.alive = false;
            GameManager.Instance.RespawnPlayer();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(SideAttackTransform.position, SideAttackArea);
        Gizmos.DrawWireCube(UpAttackTransform.position, UpAttackArea);
        Gizmos.DrawWireCube(DownAttackTransform.position, DownAttackArea);
    }

    // Update is called once per frame
    void Update()
    {
        if (pState.cutscene) return;
        if (pState.alive)
        {
            GetInputs();
            ToggleMap();
            ToggleInventory();
        }

        UpdateJumpVariables();
        UpdateCameraYDampForPlayerFall();
        RestoreTimeScale();

        if (pState.alive)
        {
            Heal();
        }

        if (pState.dashing || pState.healing) return;
        if (pState.alive)
        {
            if (!isWallJumping)
            {
                Flip();
                Move();
                Jump();
            }

            if (unlockedWallJump)
            {
                WallSlide();
                WallJump();
            }

            if (unlockedDash)
            {
                StartDash();
            }

            Attack();
            CastSpell();
        }
        FlashWhileInvincible();

        if (Input.GetKeyDown(KeyCode.L))
        {
            StartCoroutine(Death());
        }
    }

    private void OnTriggerEnter2D(Collider2D _other)    // For up and down cast spell
    {
        if (_other.GetComponent<Enemy>() != null && pState.casting)
        {
            _other.GetComponent<Enemy>().EnemyGetsHit(spellDamage, (_other.transform.position - transform.position).normalized, -recoilYSpeed);
        }
    }

    private void FixedUpdate()
    {
        if (pState.cutscene) return;

        if (pState.dashing) return;
        Recoil();
    }

    void GetInputs()
    {
        xAxis = Input.GetAxisRaw("Horizontal");
        yAxis = Input.GetAxisRaw("Vertical");
        attack = Input.GetButtonDown("Attack");
        openMap = Input.GetButton("Map");
        openInventory = Input.GetButton("Inventory");

        if (Input.GetButton("Cast/Heal"))
        {
            castOrHealTimer += Time.deltaTime;
        }
    }

    void ToggleMap()
    {
        if (openMap)
        {
            UIManager.Instance.mapHandler.SetActive(true);
        }
        else
        {
            UIManager.Instance.mapHandler.SetActive(false);
        }
    }

    void ToggleInventory()
    {
        if (openInventory)
        {
            UIManager.Instance.inventory.SetActive(true);
        }
        else
        {
            UIManager.Instance.inventory.SetActive(false);
        }
    }

    void Flip()
    {
        if (xAxis < 0)
        {
            transform.localScale = new Vector2(-1, transform.localScale.y);
            pState.lookingRight = false;
        }
        else if (xAxis > 0)
        {
            transform.localScale = new Vector2(1, transform.localScale.y);
            pState.lookingRight = true;
        }
    }

    private void Move()
    {
        if (pState.healing) rb.velocity = new Vector2(0, 0);
        rb.velocity = new Vector2(xAxis * walkSpeed, rb.velocity.y);
        anim.SetBool("Walking", rb.velocity.x != 0 && Grounded());
    }

    void UpdateCameraYDampForPlayerFall()
    {
        // If falling pass a certain speed threshold
        if (rb.velocity.y < playerFallSpeedThrehold && !CameraManager.Instance.isLerpingYDamping && !CameraManager.Instance.hasLerpedYDamping)
        {
            StartCoroutine(CameraManager.Instance.LerpYDamping(true));
        }
        // If standing still or moving up
        if (rb.velocity.y >= 0 && !CameraManager.Instance.isLerpingYDamping && CameraManager.Instance.hasLerpedYDamping)
        {
            // Reset camera function
            CameraManager.Instance.hasLerpedYDamping = false;
            StartCoroutine(CameraManager.Instance.LerpYDamping(false));
        }
    }

    void StartDash()
    {
        if (Input.GetButtonDown("Dash") && canDash && !dashed)
        {
            StartCoroutine(Dash());
            dashed = true;
            // Debug.Log("StartDash() Called");
        }

        if (Grounded())
        {
            dashed = false;
        }
    }

    IEnumerator Dash()
    {
        canDash = false;
        pState.dashing = true;
        anim.SetTrigger("Dashing");
        audioSource.PlayOneShot(dashSound);
        rb.gravityScale = 0;
        int _dir = pState.lookingRight ? 1 : -1;
        rb.velocity = new Vector2(_dir * dashSpeed, 0);
        if (Grounded()) Instantiate(dashEffect, transform);
        yield return new WaitForSeconds(dashTime);
        rb.gravityScale = gravity;
        pState.dashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    public IEnumerator WalkIntoNewScene(Vector2 _exitDir, float _delay)
    {
        pState.invincible = true;

        //If exit direction is upwards
        if (_exitDir.y > 0)
        {
            rb.velocity = jumpForce * _exitDir;
        }

        //If exit direction requires horizontal movement
        if (_exitDir.x != 0)
        {
            xAxis = _exitDir.x > 0 ? 1 : -1;
            Move();
        }

        Flip();
        yield return new WaitForSeconds(_delay);
        pState.invincible = false;
        pState.cutscene = false;
    }

    void Attack()
    {
        timeSinceAttack += Time.deltaTime;
        if (attack && timeSinceAttack >= timeBetweenAttack)
        {
            timeSinceAttack = 0;
            anim.SetTrigger("Attacking");
            audioSource.PlayOneShot(attackVoiceSound);

            if (yAxis == 0 || yAxis < 0 && Grounded())
            {
                int _recoilLeftOrRight = pState.lookingRight ? 1 : -1;

                Hit(SideAttackTransform, SideAttackArea, ref pState.recoilingX, Vector2.right * _recoilLeftOrRight, recoilXSpeed);
                Instantiate(slashEffect, SideAttackTransform);
            }
            else if (yAxis > 0)
            {
                Hit(UpAttackTransform, UpAttackArea, ref pState.recoilingY, Vector2.up, recoilYSpeed);
                SlashEffectAtAngle(slashEffect, 90, UpAttackTransform);
            }
            else if (yAxis < 0 && !Grounded())
            {
                Hit(DownAttackTransform, DownAttackArea, ref pState.recoilingY, Vector2.down, recoilYSpeed);
                SlashEffectAtAngle(slashEffect, -90, DownAttackTransform);
            }
        }
    }

    void Hit(Transform _attackTransform, Vector2 _attackArea, ref bool _recoilBool, Vector2 _recoilDir, float _recoilStrength)
    {
        Collider2D[] objectsToHit = Physics2D.OverlapBoxAll(_attackTransform.position, _attackArea, 0, attackableLayer);

        if (objectsToHit.Length > 0)
        {
            _recoilBool = true;
        }

        for (int i = 0; i < objectsToHit.Length; i++)
        {
            if (objectsToHit[i].GetComponent<Enemy>() != null)
            {
                objectsToHit[i].GetComponent<Enemy>().EnemyGetsHit(damage, _recoilDir, _recoilStrength);

                if (objectsToHit[i].CompareTag("Enemy"))
                {
                    if (!halfMana && Mana < 1 || halfMana && Mana < 0.5)
                    {
                        Mana += manaGain;
                    }
                    else
                    {
                        manaOrbsHandler.UpdateMana(manaGain * 3);
                    }
                }
            }
        }
    }

    void SlashEffectAtAngle(GameObject _slashEffect, int _effectAngle, Transform _attackTransform)
    {
        _slashEffect = Instantiate(_slashEffect, _attackTransform);
        _slashEffect.transform.eulerAngles = new Vector3(0, 0, _effectAngle);
        _slashEffect.transform.localScale = new Vector2(transform.localScale.x, transform.localScale.y);
    }

    void Recoil()
    {
        if (pState.recoilingX)
        {
            if (pState.lookingRight)
            {
                rb.velocity = new Vector2(-recoilXSpeed, 0);
            }
            else
            {
                rb.velocity = new Vector2(recoilXSpeed, 0);
            }

        }

        if (pState.recoilingY)
        {
            rb.gravityScale = 0;
            if (yAxis < 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, recoilYSpeed);
            }
            else
            {
                rb.velocity = new Vector2(rb.velocity.x, -recoilYSpeed);
            }
            airJumpCounter = 0;
        }
        else
        {
            rb.gravityScale = gravity;
        }

        // Stop recoil
        if (pState.recoilingX && stepsXRecoiled < recoilXSteps)
        {
            stepsXRecoiled++;
        }
        else
        {
            StopRecoilX();
        }


        if (pState.recoilingY && stepsYRecoiled < recoilYSteps)
        {
            stepsYRecoiled++;
        }
        else
        {
            StopRecoilY();
        }

        if (Grounded())
        {
            StopRecoilY();
        }
    }

    void StopRecoilX()
    {
        stepsXRecoiled = 0;
        pState.recoilingX
         = false;
    }

    public void TakeDamage(float _damage)
    {
        if (pState.alive)
        {
            audioSource.PlayOneShot(hurtSound);

            Health -= Mathf.RoundToInt(_damage);

            if (Health <= 0)
            {
                Health = 0;
                StartCoroutine(Death());
            }
            else
            {
                StartCoroutine(StopTakingDamage());
            }

            Console.Write("takeDamage");
        }
    }

    IEnumerator StopTakingDamage()
    {
        pState.invincible = true;
        GameObject _bloodSpurtParticles = Instantiate(bloodSpurt, transform.position, Quaternion.identity);
        Destroy(_bloodSpurtParticles, 1.5f);
        anim.SetTrigger("TakeDamage");
        ClampHealth();
        yield return new WaitForSeconds(1f);
        pState.invincible = false;
    }

    IEnumerator Flash()
    {
        sr.enabled = !sr.enabled;
        canFlash = false;
        yield return new WaitForSeconds(0.1f);
        canFlash = true;
    }

    void FlashWhileInvincible()
    {
        if (pState.invincible && !pState.cutscene)
        {
            if (Time.timeScale > 0.2 && canFlash)
            {
                StartCoroutine(Flash());
            }
        }
        else
        {
            sr.enabled = true;
        }
    }

    void RestoreTimeScale()
    {
        if (restoreTime == true)
        {
            if (Time.timeScale < 1)
            {
                Time.timeScale += Time.unscaledDeltaTime * restoreTimeSpeed;
            }
            else
            {
                Time.timeScale = 1;
                restoreTime = false;
            }
        }
    }

    public void HitStopTime(float _newTimeScale, int _restoreSpeed, float _delay)
    {
        restoreTimeSpeed = _restoreSpeed;
        Time.timeScale = _newTimeScale;

        if (_delay > 0)
        {
            StopCoroutine(StartTimeAgain(_delay));
            StartCoroutine(StartTimeAgain(_delay));
        }
        else
        {
            restoreTime = true;
        }
    }

    IEnumerator StartTimeAgain(float _delay)
    {
        yield return new WaitForSecondsRealtime(_delay);
        restoreTime = true;
    }

    IEnumerator Death()
    {
        pState.alive = false;
        Time.timeScale = 1f;
        GameObject _bloodSpurtParticles = Instantiate(bloodSpurt, transform.position, Quaternion.identity);
        Destroy(_bloodSpurtParticles, 1.5f);
        anim.SetTrigger("Death");
        rb.constraints = RigidbodyConstraints2D.FreezePosition;
        GetComponent<BoxCollider2D>().enabled = false;

        yield return new WaitForSecondsRealtime(0.9f);
        StartCoroutine(UIManager.Instance.ActivateDeathScreen());

        yield return new WaitForSeconds(0.9f);
        Instantiate(GameManager.Instance.shade, transform.position, Quaternion.identity);
        // Debug.Log("Instantiate!");
    }

    public void Respawned()
    {
        if (!pState.alive)
        {
            rb.constraints = RigidbodyConstraints2D.None;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            GetComponent<BoxCollider2D>().enabled = true;

            pState.alive = true;
            halfMana = true;
            UIManager.Instance.SwitchMana(UIManager.ManaState.HalfMana);
            Mana = 0;
            Health = maxHealth;
            anim.Play("Player_Idle");
        }
    }

    public void RestoreMana()
    {
        halfMana = false;
        UIManager.Instance.SwitchMana(UIManager.ManaState.FullMana);

    }

    public int Health
    {
        get { return health; }
        set
        {
            if (health != value)
            {
                health = Mathf.Clamp(value, 0, maxHealth);

                if (onHealthChangeCallback != null)
                {
                    onHealthChangeCallback.Invoke();
                }
            }
        }
    }

    void Heal()
    {
        if (Input.GetButton("Cast/Heal") && castOrHealTimer > 0.3f && Health < maxHealth && Mana > 0 && Grounded() && !pState.dashing)
        {
            pState.healing = true;
            anim.SetBool("Healing", true);

            // Healing
            healTimer += Time.deltaTime;
            Debug.Log("healTimer: " + healTimer + ", timeToHeal: " + timeToHeal);
            if (healTimer >= timeToHeal)
            {
                Health++;
                healTimer = 0;
            }

            // Drain mana
            manaOrbsHandler.usedMana = true;
            manaOrbsHandler.countDown = 3f;
            Mana -= Time.deltaTime * manaDrainSpeed;
        }
        else
        {
            pState.healing = false;
            anim.SetBool("Healing", false);
            healTimer = 0;
        }
    }

    public float Mana
    {
        get { return mana; }
        set
        {
            // If mana stats change
            if (mana != value)
            {
                if (!halfMana)
                {
                    mana = Mathf.Clamp(value, 0, 1);
                }
                else
                {
                    mana = Mathf.Clamp(value, 0, 0.5f);
                }

                manaStorage.fillAmount = Mana;
            }
        }
    }

    void CastSpell()
    {
        if (Input.GetButtonUp("Cast/Heal") && castOrHealTimer <= 0.3f && timeSinceCast >= timeBetweenCast && Mana >= manaSpellCost)
        {
            Debug.Log("Cast spell!");
            pState.casting = true;
            timeSinceCast = 0;
            StartCoroutine(CastCoroutine());
        }
        else
        {
            // pState.casting = false;
            timeSinceCast += Time.deltaTime;
        }

        if (!Input.GetButton("Cast/Heal"))
        {
            castOrHealTimer = 0;
        }

        if (Grounded())
        {
            // Disable downSpell if on the ground
            downSpellFireball.SetActive(false);
        }

        // If down spell is active, force player down until grounded
        if (downSpellFireball.activeInHierarchy)
        {
            rb.velocity += downSpellForce * Vector2.down;
        }
    }

    IEnumerator CastCoroutine()
    {
        //side cast
        if ((yAxis == 0 || (yAxis < 0 && Grounded())) && unlockedSideCast)
        {
            audioSource.PlayOneShot(spellCastSound);
            anim.SetBool("Casting", true);
            yield return new WaitForSeconds(0.15f);

            GameObject _fireBall = Instantiate(sideSpellFireball, SideAttackTransform.position, Quaternion.identity);

            //flip fireball
            if (pState.lookingRight)
            {
                _fireBall.transform.eulerAngles = Vector3.zero; // if facing right, fireball continues as per normal
            }
            else
            {
                _fireBall.transform.eulerAngles = new Vector2(_fireBall.transform.eulerAngles.x, 180);
                //if not facing right, rotate the fireball 180 deg
            }
            pState.recoilingX = true;

            Mana -= manaSpellCost;
            manaOrbsHandler.usedMana = true;
            manaOrbsHandler.countDown = 3f;
            yield return new WaitForSeconds(0.35f);
        }

        //up cast
        else if (yAxis > 0 && unlockedUpCast)
        {
            audioSource.PlayOneShot(spellCastSound);
            anim.SetBool("Casting", true);
            yield return new WaitForSeconds(0.15f);

            Instantiate(upSpellExplosion, transform);
            rb.velocity = Vector2.zero;

            Mana -= manaSpellCost;
            manaOrbsHandler.usedMana = true;
            manaOrbsHandler.countDown = 3f;
            yield return new WaitForSeconds(0.35f);
        }

        //down cast
        else if (yAxis < 0 && !Grounded() && unlockedDownCast)
        {
            audioSource.PlayOneShot(spellCastSound);
            anim.SetBool("Casting", true);
            yield return new WaitForSeconds(0.15f);

            downSpellFireball.SetActive(true);

            Mana -= manaSpellCost;
            manaOrbsHandler.usedMana = true;
            manaOrbsHandler.countDown = 3f;
            yield return new WaitForSeconds(0.35f);
        }
        anim.SetBool("Casting", false);
        pState.casting = false;
    }

    void StopRecoilY()
    {
        stepsYRecoiled = 0;
        pState.recoilingY = false;
    }

    void ClampHealth()
    {
        health = Mathf.Clamp(health, 0, maxHealth);
    }

    public bool Grounded()
    {
        if (Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckY, whatIsGround)
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround)
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(-groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void Jump()
    {
        if (jumpBufferCounter > 0 && coyoteTimeCounter > 0 && !pState.jumping)
        {
            if (Input.GetButtonDown("Jump"))
            {
                audioSource.PlayOneShot(jumpVoiceSound);
            }

            rb.velocity = new Vector3(rb.velocity.x, jumpForce);

            pState.jumping = true;
        }

        if (!Grounded() && airJumpCounter < maxAirJumps && Input.GetButtonDown("Jump") && unlockedVarJump)
        {
            audioSource.PlayOneShot(jumpVoiceSound);

            pState.jumping = true;

            airJumpCounter++;

            rb.velocity = new Vector3(rb.velocity.x, jumpForce);
        }

        if (Input.GetButtonUp("Jump") && rb.velocity.y > 3)
        {
            pState.jumping = false;

            rb.velocity = new Vector2(rb.velocity.x, 0);
        }

        rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -maxFallingSpeed, rb.velocity.y));

        anim.SetBool("Jumping", !Grounded());
    }

    void UpdateJumpVariables()
    {
        if (Grounded())
        {
            if (!landingSoundPlayed)
            {
                audioSource.PlayOneShot(landingSound);
                landingSoundPlayed = true;
            }

            pState.jumping = false;
            coyoteTimeCounter = coyoteTime;
            airJumpCounter = 0;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
            landingSoundPlayed = false;
        }

        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferFrames;
        }
        else
        {
            jumpBufferCounter--;
        }
    }

    private bool Walled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer);
    }

    void WallSlide()
    {
        if (Walled() && !Grounded() && xAxis != 0)
        {
            isWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
        }
        else
        {
            isWallSliding = false;
        }
    }

    void WallJump()
    {
        if (isWallSliding)
        {
            isWallJumping = false;
            wallJumpingDirection = !pState.lookingRight ? 1 : -1;

            CancelInvoke(nameof(StopWallJumping));
        }

        if (Input.GetButtonDown("Jump") && isWallSliding)
        {
            isWallJumping = true;
            rb.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);

            dashed = false;
            airJumpCounter = 0;

            pState.lookingRight = !pState.lookingRight;
            transform.eulerAngles = new Vector2(transform.eulerAngles.x, 180);

            // if (pState.lookingRight && transform.eulerAngles.y == 0 || !pState.lookingRight && transform.eulerAngles.y != 0)
            // {
            //     pState.lookingRight = !pState.lookingRight;
            //     int _yRotation = pState.lookingRight ? 0 : 100;

            //     transform.eulerAngles = new Vector2(transform.eulerAngles.x, _yRotation);
            // }

            Invoke(nameof(StopWallJumping), wallJumpingDuration);
        }
    }

    void StopWallJumping()
    {
        isWallJumping = false;
        transform.eulerAngles = new Vector2(transform.eulerAngles.x, 0);

        // rb.velocity = new Vector2(rb.velocity.x, 0);
    }
}
