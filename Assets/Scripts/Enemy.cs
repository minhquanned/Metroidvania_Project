using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected float health;
    [SerializeField] protected float recoilLength;
    [SerializeField] protected float recoilFactor;
    [SerializeField] protected bool isRecoiling = false;

    // [SerializeField] protected PlayerController player;
    [SerializeField] public float speed;

    [SerializeField] public float damage;
    [SerializeField] protected GameObject monsterBlood;

    [SerializeField] protected AudioClip hurtSound;

    protected float recoilTimer;
    [HideInInspector] public Rigidbody2D rb;
    protected SpriteRenderer sr;
    public Animator anim;
    protected AudioSource audioSource;

    protected enum EnemyStates
    {
        // Crawler
        Crawler_Idle,
        Crawler_Flip,

        // Bat
        Bat_Idle,
        Bat_Chase,
        Bat_Stunned,
        Bat_Death,

        // Charger
        Charger_Idle,
        Charger_Surprised,
        Charger_Charge,

        // Shade
        Shade_Idle,
        Shade_Chase,
        Shade_Stunned,
        Shade_Death,

        //THK
        THK_Stage1,
        THK_Stage2,
        THK_Stage3,
        THK_Stage4,
    }

    protected EnemyStates currentEnemyState;

    protected virtual EnemyStates GetCurrentEnemyState
    {
        get { return currentEnemyState; }
        set
        {
            if (currentEnemyState != value)
            {
                currentEnemyState = value;

                ChangeCurrentAnimation();
            }
        }
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    // protected virtual void Awake()
    // {
    //     player = PlayerController.Instance;
    // }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (GameManager.Instance.gameIsPaused) return;

        if (isRecoiling)
        {
            if (recoilTimer < recoilLength)
            {
                recoilTimer += Time.deltaTime;
            }
            else
            {
                isRecoiling = false;
                recoilTimer = 0;
            }
        }
        else
        {
            UpdateEnemyStates();
        }
    }

    public virtual void EnemyGetsHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
    {
        health -= _damageDone;
        if (!isRecoiling)
        {
            audioSource.PlayOneShot(hurtSound);
            GameObject _monsterBlood = Instantiate(monsterBlood, transform.position, Quaternion.identity);
            Destroy(_monsterBlood, 5.5f);
            rb.velocity = _hitForce * recoilFactor * _hitDirection;
        }
        Debug.Log("Hit!");
    }

    protected virtual void OnCollisionStay2D(Collision2D _other)
    {
        if (_other.gameObject.CompareTag("Player") && !PlayerController.Instance.pState.invincible && health > 0)
        {
            Attack();
            if (PlayerController.Instance.pState.alive)
            {
                PlayerController.Instance.HitStopTime(0, 5, 0.5f);
            }
        }
    }

    protected virtual void Death(float _destroyTime)
    {
        Destroy(gameObject, _destroyTime);
    }

    protected virtual void UpdateEnemyStates() { }

    protected virtual void ChangeCurrentAnimation() { }

    protected void ChangeState(EnemyStates _newState)
    {
        // Debug.Log(name + " changed state to " + _newState.ToString());
        GetCurrentEnemyState = _newState;
    }

    protected virtual void Attack()
    {
        PlayerController.Instance.TakeDamage(damage);
    }
}
