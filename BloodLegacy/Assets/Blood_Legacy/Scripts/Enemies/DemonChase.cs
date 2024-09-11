using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonChase : MonoBehaviour
{
    #region Serialized Variables

    #region Floats and Ints
    [Space, Header("Floats and Ints")]
    [SerializeField]
    [Tooltip("Enemy Speed")]
    private float demonSpeed = default;

    [SerializeField]
    [Tooltip("At what Distance does the Demon Attack the Player?")]
    private float attackDistance = default;

    [SerializeField]
    [Tooltip("Attack Cooldown of the Demon")]
    private float atkCooldown = default;

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
    [Tooltip("Player Reference GameObject")]
    private GameObject player = default;
    #endregion

    #region Events

    #region Events Void
    public delegate void SendEvents();
    /// <summary>
    /// Event sent from DemonEnemy script to PlayerMovementV2 and AudioManager Scripts;
    /// Kills the player and plays the death SFX;
    /// </summary>
    public static event SendEvents OnPlayerKill;

    /// <summary>
    /// Event sent from DemonEnemy to AudioManager Script;
    /// Kills the enemy and plays the death SFX;
    /// </summary>
    public static event SendEvents OnEnemyDead;
    #endregion

    #region Events Int
    public delegate void SendEventsInt(int score);
    public static event SendEventsInt OnEnemyKillScore;
    #endregion

    #endregion

    #endregion

    #region Private Variables
    private Animator _demonAnim;
    [SerializeField] private float _cooldownTimer = default;
    [SerializeField] private EnemyState _curState = EnemyState.Chasing;
    private enum EnemyState { Chasing, Attacking, Dead, PlayerDead };
    [SerializeField] private float _distance = default;
    [SerializeField] private bool _isPlayerDead = default;
    #endregion

    #region Unity Callbacks

    #region Events
    void OnEnable()
    {
        AprilPlayerController.OnPlayerDead += OnPlayerDeadEventReceived;
    }

    void OnDisable()
    {
        AprilPlayerController.OnPlayerDead -= OnPlayerDeadEventReceived;
    }

    void OnDestroy()
    {
        AprilPlayerController.OnPlayerDead -= OnPlayerDeadEventReceived;
    }
    #endregion

    void Start()
    {
        _demonAnim = GetComponent<Animator>();
        _cooldownTimer = atkCooldown;
    }

    void Update()
    {
        if (_curState != EnemyState.PlayerDead)
            _distance = Vector3.Distance(transform.position, player.transform.position);

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
        OnEnemyDead?.Invoke();
        OnEnemyKillScore?.Invoke(enemyScoreIncrement);
        Destroy(gameObject);
        Debug.Log("Killing Enemy");
    }

    /// <summary>
    /// 
    /// </summary>
    void DemonStates()
    {
        switch (_curState)
        {
            case EnemyState.Chasing:
                ChasePlayer();

                if (_distance <= attackDistance)
                {
                    _curState = EnemyState.Attacking;
                    //Debug.Log("Switching to Attack");
                }
                break;
            case EnemyState.Attacking:
                AttackPlayer();

                if (_distance >= attackDistance && !PlayerInSight())
                {
                    _curState = EnemyState.Chasing;
                    _cooldownTimer = atkCooldown;
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
        _demonAnim.SetBool("isAttackingBool", false);

        Vector3 target = player.transform.position;
        target.y = transform.position.y;

        transform.position = Vector3.MoveTowards(transform.position, target, demonSpeed * Time.deltaTime);
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
        //        _enemyAnim.SetTrigger("isAttacking");
        //    }
        //}

        if (PlayerInSight() && !_isPlayerDead)
        {
            _demonAnim.SetBool("isAttackingBool", true);
        }

        //Debug.Log("Attacking");
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
        if (PlayerInSight())
        {
            OnPlayerKill?.Invoke();
            Debug.Log("Killing Player");
        }
    }

    /// <summary>
    /// Subbed to AprilPlayerController Script;
    /// Lets the Enemy Script know that the player is Dead;
    /// </summary>
    void OnPlayerDeadEventReceived()
    {
        _curState = EnemyState.PlayerDead;
        //_isPlayerDead = true;
    }
    #endregion
}