using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DemonEnemy : MonoBehaviour
{
    #region Serialized Variables
    [Space, Header("Attack Fields")]
    [SerializeField] private float atkCooldown = default;
    [SerializeField] private float range = default;

    [Space, Header("Range Fields")]
    [SerializeField] private float colliderDistance;
    [SerializeField] private BoxCollider2D boxCollider;

    [Space, Header("Player Fields")]
    [SerializeField] private LayerMask playerLayer;

    #region Scoring System
    [Space, Header("Scoring System")]
    [SerializeField]
    [Tooltip("How much score to increment when the Enemy is dead?")]
    private int enemyScoreIncrement = default;
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
    private Animator _enemyAnim;
    private Rigidbody2D _rb2D;
    private DemonPatrol _demonPatrol;
    private Collider2D _col2D = default;
    [SerializeField] private float _cooldownTimer = default;
    [SerializeField] private bool _isPlayerDead = default;
    #endregion

    #region Unity Callbacks

    #region Events
    void OnEnable()
    {
        AprilPlayerController.OnPlayerDead += OnPlayerDeadEventReceived;
        AprilPlayerController.OnEnemyKill += OnEnemyKillEventReceived;
    }

    void OnDisable()
    {
        AprilPlayerController.OnPlayerDead -= OnPlayerDeadEventReceived;
        AprilPlayerController.OnEnemyKill -= OnEnemyKillEventReceived;
    }

    void OnDestroy()
    {
        AprilPlayerController.OnPlayerDead -= OnPlayerDeadEventReceived;
        AprilPlayerController.OnEnemyKill -= OnEnemyKillEventReceived;
    }
    #endregion

    void Start()
    {
        _enemyAnim = GetComponent<Animator>();
        _demonPatrol = GetComponentInParent<DemonPatrol>();
        _rb2D = GetComponent<Rigidbody2D>();
        _col2D = GetComponent<Collider2D>();
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
                Debug.Log("Attacking");
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
    void OnPlayerDeadEventReceived() => _isPlayerDead = true;

    /// <summary>
    /// Subbed to AprilPlayerController Script;
    /// Kills the enemy;
    /// </summary>
    void OnEnemyKillEventReceived()
    {
        OnEnemyDead?.Invoke();
        Destroy(gameObject);
        _demonPatrol.IsDemonAlive = false;
    }
    #endregion
}