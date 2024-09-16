using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonDefault : MonoBehaviour
{
    #region Serialized Variables
    [Space, Header("Attack Fields")]
    [SerializeField]
    [Tooltip("Attack Cooldown of the Demon")]
    private float atkCooldown = default;

    [SerializeField]
    [Tooltip("The attack range of the Demon")]
    private float range = default;

    [Space, Header("Range Fields")]
    [SerializeField]
    [Tooltip("The Distance of the Collider from the Demon")]
    private float colliderDistance;

    [SerializeField]
    [Tooltip("The Box Collder Trigger for Attacking")]
    private BoxCollider2D boxCollider;

    [Space, Header("Player Fields")]
    [SerializeField]
    [Tooltip("Player LayerMask to attach the Player")]
    private LayerMask playerLayer;

    [Space, Header("Scoring System")]
    [SerializeField]
    [Tooltip("How much score to increment when the Enemy is dead?")]
    private int enemyScoreIncrement = default;

    #region Events

    #region Events Void
    public delegate void SendEvents();
    /// <summary>
    /// Event sent from DemonEnemy script to AprilPlayerController and AudioManager Scripts;
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
    private Animator _enemyAnim;
    private DemonPatrol _demonPatrol;
    private float _cooldownTimer = default;
    private bool _isPlayerDead = default;
    [SerializeField] private bool _canAttackPlayer = true;
    #endregion

    #region Unity Callbacks

    #region Events
    void OnEnable()
    {
        AprilPlayerController.OnPlayerDead += OnPlayerDeadEventReceived;
        AprilPlayerController.OnPlayerInvincible += OnPlayerInvincibleEventReceived;
    }

    void OnDisable()
    {
        AprilPlayerController.OnPlayerDead -= OnPlayerDeadEventReceived;
        AprilPlayerController.OnPlayerInvincible -= OnPlayerInvincibleEventReceived;
    }

    void OnDestroy()
    {
        AprilPlayerController.OnPlayerDead -= OnPlayerDeadEventReceived;
        AprilPlayerController.OnPlayerInvincible -= OnPlayerInvincibleEventReceived;
    }
    #endregion

    void Start()
    {
        _enemyAnim = GetComponent<Animator>();
        _demonPatrol = GetComponentInParent<DemonPatrol>();
    }

    void Update()
    {
        if (_cooldownTimer <= atkCooldown)
            _cooldownTimer += Time.deltaTime;

        if (PlayerInSight() && !_isPlayerDead)
        {
            if (_cooldownTimer >= atkCooldown)
            {
                _cooldownTimer = 0;
                _enemyAnim.SetTrigger("isAttacking");
                //Debug.Log("Attacking");
            }
        }

        if (_demonPatrol != null)
            _demonPatrol.enabled = !PlayerInSight();
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
    /// Bool check to see if the Player is within the Box Trigger Collider;
    /// </summary>
    /// <returns> If true, Player in sight, if false, Player out of sight; </returns>
    bool PlayerInSight()
    {
        RaycastHit2D hit = Physics2D.BoxCast(boxCollider.bounds.center + colliderDistance * range * transform.localScale.x * transform.right,
            new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z), 0, Vector2.left, 0, playerLayer);

        return hit.collider != null;
    }

    public void EnemyKill()
    {
        OnEnemyDead?.Invoke();
        Destroy(gameObject);
        OnEnemyKillScore?.Invoke(enemyScoreIncrement);
        _demonPatrol.IsDemonAlive = false;
        //Debug.Log("Killing Enemy");
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
            OnPlayerKill?.Invoke();
            Debug.Log("Killing Player");
        }
    }

    /// <summary>
    /// Subbed to AprilPlayerController Script;
    /// Lets the Enemy Script know that the player is Dead;
    /// </summary>
    void OnPlayerDeadEventReceived() => _isPlayerDead = true;

    void OnPlayerInvincibleEventReceived(bool isInvincible)
    {
        if (isInvincible)
            _canAttackPlayer = false;
        else
            _canAttackPlayer = true;
    }
    #endregion
}