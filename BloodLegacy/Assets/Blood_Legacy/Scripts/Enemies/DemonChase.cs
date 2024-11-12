using UnityEngine;

public class DemonChase : MonoBehaviour
{
    #region Serialized Variables

    #region Floats and Ints
    [Space, Header("Floats and Ints")]
    [SerializeField]
    [Tooltip("Demon Speed")]
    private float demonSpeed = default;

    [SerializeField]
    [Tooltip("Demon Speed in cutscene")]
    private float demonSpeedCutscene = default;

    [SerializeField]
    [Tooltip("At what Distance does the Demon Attack the Player?")]
    private float attackDistance = default;

    //[SerializeField]
    //[Tooltip("Attack Cooldown of the Demon")]
    //private float atkCooldown = default;

    [SerializeField]
    [Tooltip("The attack range of the Demon")]
    private float range = default;

    [SerializeField]
    [Tooltip("The Distance of the Collider from the Demon")]
    private float colliderDistance;

    [SerializeField]
    [Tooltip("How much score to increment when the Enemy is dead?")]
    private int enemyScoreIncrement = default;
    #endregion

    #region Others
    [Space, Header("Others")]
    [SerializeField]
    [Tooltip("The Box Collder Trigger for Attacking")]
    private BoxCollider2D boxCollider;

    [SerializeField]
    [Tooltip("Player LayerMask to attach the Player")]
    private LayerMask playerLayer;

    [SerializeField]
    [Tooltip("Demon Blood Particle Effects")]
    private ParticleSystem[] bloodFX = default;
    #endregion

    #region Events

    #region Events Void
    public delegate void SendEvents();
    /// <summary>
    /// Event sent from DemonEnemy script to AprilPlayerController and AudioManager Scripts;
    /// Damages the player;
    /// </summary>
    public static event SendEvents OnPlayerDamage;

    /// <summary>
    /// Event sent from DemonEnemy script to AprilPlayerController and AudioManager Scripts;
    /// Plays SFX and FX for blocknig;;
    /// </summary>
    public static event SendEvents OnPlayerBlock;

    /// <summary>
    /// Event sent from DemonEnemy to AudioManager Script;
    /// Kills the enemy and plays the death SFX;
    /// </summary>
    public static event SendEvents OnEnemyDead;
    #endregion

    #endregion

    #endregion

    #region Private Variables
    private Animator _demonAnim = default;
    private Rigidbody2D _demonrb2D = default;
    private CapsuleCollider2D _demonCapsuleCol2D = default;
    //[SerializeField] private float _cooldownTimer = default;
    [SerializeField] private float _currDemonSpeed = default;
    [SerializeField] private EnemyState _currState = EnemyState.Chasing;
    private enum EnemyState { Chasing, Attacking, Dead, PlayerDead };
    private float _distance = default;
    private bool _isPlayerDead = false;
    private bool _isMovingLeft = false;
    public GameObject Player { get => _player; set => _player = value; }
    [SerializeField] private GameObject _player = default;
    private Vector3 _initScale = default;
    private Vector3 _direction = default;
    [SerializeField] private bool _canAttackPlayer = true;
    [SerializeField] private int _randDeathIndex = default;
    #endregion

    #region Unity Callbacks

    #region Events
    void OnEnable()
    {
        AprilPlayerController.OnPlayerDead += OnPlayerDeadEventReceived;
        AprilPlayerController.OnPlayerInvincible += OnPlayerInvincibleEventReceived;

        MiniGameManager.OnDemonChase += OnDemonChaseEventReceived;
    }

    void OnDisable()
    {
        AprilPlayerController.OnPlayerDead -= OnPlayerDeadEventReceived;
        AprilPlayerController.OnPlayerInvincible -= OnPlayerInvincibleEventReceived;

        MiniGameManager.OnDemonChase -= OnDemonChaseEventReceived;
    }

    void OnDestroy()
    {
        AprilPlayerController.OnPlayerDead -= OnPlayerDeadEventReceived;
        AprilPlayerController.OnPlayerInvincible -= OnPlayerInvincibleEventReceived;

        MiniGameManager.OnDemonChase -= OnDemonChaseEventReceived;
    }
    #endregion

    void Start()
    {
        _demonAnim = GetComponent<Animator>();
        _demonrb2D = GetComponent<Rigidbody2D>();
        _demonCapsuleCol2D = GetComponent<CapsuleCollider2D>();
        _initScale = transform.localScale;
        _currDemonSpeed = demonSpeed;
        _randDeathIndex = Random.Range(0, 2);
        //_cooldownTimer = atkCooldown;
    }

    void Update()
    {
        if (_currState != EnemyState.PlayerDead && Player != null)
        {
            _distance = Vector3.Distance(transform.position, Player.transform.position);
            MovementDirCheck();
        }

        DemonStates();
    }

    void OnDrawGizmos()
    {
        if (boxCollider != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(boxCollider.bounds.center + colliderDistance * range * transform.localScale.x * transform.right,
                new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z));
        }
    }
    #endregion

    #region My Functions
    /// <summary>
    /// Tied to AprilPlayerController;
    /// Kills the Enemy when it reaches within the Player Attack Trigger;
    /// </summary>
    public void EnemyKill()
    {
        _currState = EnemyState.Dead;
        _demonAnim.SetTrigger("isDead");
        _demonAnim.SetInteger("deadParameter", _randDeathIndex);
        _demonAnim.SetBool("isMoving", false);
        _demonAnim.SetBool("isAttacking", false);
        _demonAnim.Play("C_Demon_2_Idle_Anim");
        _demonrb2D.isKinematic = true;
        _demonrb2D.velocity = Vector2.zero;
        _demonCapsuleCol2D.enabled = false;
        gameObject.layer = LayerMask.NameToLayer("Default");
        this.enabled = false;
    }

    /// <summary>
    /// Tied to Enable Event on MMF_MiniGame_Intro;
    /// Changes the Demon speed after the cutscene;
    /// </summary>
    public void OnDemonChaseDefault() => _currState = EnemyState.Chasing;

    /// <summary>
    /// Tied to OnCharDisable Event on MMF_MiniGame_Intro;
    /// Changes the Demon speed after the cutscene;
    /// </summary>
    public void OnDemonIdleDefault()
    {
        _currState = EnemyState.Dead;
        _demonAnim.Play("C_Demon_2_Idle_Anim");
    }

    /// <summary>
    /// Tied to C_Demon_2_Dead_Anim;
    /// Plays the blood effect and sends events;
    /// </summary>
    public void OnDemonDead()
    {
        bloodFX[_randDeathIndex].Play();
        OnEnemyDead?.Invoke();
    }

    /// <summary>
    /// 
    /// </summary>
    void DemonStates()
    {
        switch (_currState)
        {
            case EnemyState.Chasing:
                ChasePlayer();

                if (_distance <= attackDistance)
                {
                    _currState = EnemyState.Attacking;
                    //Debug.Log("Switching to Attack");
                }
                break;
            case EnemyState.Attacking:
                AttackPlayer();

                if (_distance >= attackDistance && !PlayerInSight())
                {
                    _currState = EnemyState.Chasing;
                    //_cooldownTimer = atkCooldown;
                    //Debug.Log("Switching to Chase");
                }
                break;
            case EnemyState.Dead:

                break;

            case EnemyState.PlayerDead:

                break;
            default:

                break;
        }
    }

    /// <summary>
    /// Chases the Player is 
    /// </summary>
    void ChasePlayer()
    {
        _demonAnim.SetBool("isMoving", true);
        _demonAnim.SetBool("isAttacking", false);

        Vector3 target = Player.transform.position;
        target.y = transform.position.y;

        _direction = target - transform.position;

        _demonrb2D.MovePosition(transform.position + _currDemonSpeed * Time.fixedDeltaTime * _direction.normalized);

        //_demonrb2D.velocity = Vector2.MoveTowards(_demonrb2D.velocity, target, demonSpeed * Time.deltaTime);
        //_demonrb2D.velocity = Vector2.ClampMagnitude(_demonrb2D.velocity, demonMaxSpeed);
        //Debug.Log("Chasing");
    }

    /// <summary>
    /// Player Attack Function, Attack the Player with a cooldown when the Player is within the Trigger;
    /// </summary>
    void AttackPlayer()
    {
        //if (_cooldownTimer <= atkCooldown)
        //    _cooldownTimer += Time.deltaTime;

        //if (PlayerInSight() && !_isPlayerDead)
        //{
        //    if (_cooldownTimer >= atkCooldown)
        //    {
        //        _cooldownTimer = 0;
        //        _demonAnim.SetBool("isAttacking", true);
        //        //_demonAnim.SetTrigger("isAttackingTrigger");
        //        Debug.Log("Attacking");
        //    }
        //}

        if (PlayerInSight() && !_isPlayerDead)
        {
            _demonAnim.SetBool("isAttacking", true);
            //Debug.Log("Attacking");
        }
    }

    /// <summary>
    /// 
    /// </summary>
    void MovementDirCheck()
    {
        if (Player.transform.position.x > transform.position.x && _isMovingLeft)
        {
            MoveInDirection(1);
            _isMovingLeft = false;
        }
        else if (Player.transform.position.x < transform.position.x && !_isMovingLeft)
        {
            MoveInDirection(-1);
            _isMovingLeft = true;
        }
    }

    void MoveInDirection(int direction)
    {
        transform.localScale = new Vector3(Mathf.Abs(_initScale.x) * direction, _initScale.y, _initScale.z);
    }

    /// <summary>
    /// Bool check to see if the Player is within the Box Trigger Collider;
    /// </summary>
    /// <returns> If true, Player in sight, if false, Player out of sight; </returns>
    bool PlayerInSight()
    {
        RaycastHit2D hit = Physics2D.BoxCast(boxCollider.bounds.center + colliderDistance * range * transform.localScale.x * transform.right,
            new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z), 0, Vector2.left, 0, playerLayer);

        return hit.collider != null;
    }

    #endregion

    #region Events
    /// <summary>
    /// Tied to Enemy C_Demon_1_Attack Anim Event;
    /// Kills the Player and updates the game that the Player is dead;
    /// </summary>
    public void OnKillPlayer()
    {
        if (PlayerInSight() && _canAttackPlayer)
        {
            OnPlayerDamage?.Invoke();
            //Debug.Log("Killing Player");
        }

        if (PlayerInSight() && !_canAttackPlayer)
        {
            OnPlayerBlock?.Invoke();
            //Debug.Log("Player Blocking");
        }

    }

    /// <summary>
    /// Subbed to event from AprilPlayerController Script;
    /// Lets the Demon know that the player is Dead;
    /// </summary>
    void OnPlayerDeadEventReceived()
    {
        _currState = EnemyState.PlayerDead;
        _demonAnim.Play("C_Demon_2_Idle_Anim");
        _demonAnim.SetBool("isMoving", false);
        _demonAnim.SetBool("isAttacking", false);
        //_isPlayerDead = true;
    }

    /// <summary>
    /// Subbed to event from AprilPlayerController Script;
    /// Lets the Demon know that the player can't be damaged;
    /// </summary>
    /// <param name="isInvincible"> If true, can't be damaged. False, can damage; </param>
    void OnPlayerInvincibleEventReceived(bool isInvincible)
    {
        if (isInvincible)
            _canAttackPlayer = false;
        else
            _canAttackPlayer = true;
    }

    void OnDemonChaseEventReceived() => _currState = EnemyState.Chasing;
    #endregion
}