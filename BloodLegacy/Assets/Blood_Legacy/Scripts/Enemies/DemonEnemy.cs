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
    /// Event sent from Enemy script to PlayerMovementV2 and GameManager Level 1, 2, 3, 4, 5, 6 and 7 Scripts;
    /// Kills the player and starts the coroutine for fading out the UI;
    /// </summary>
    public static event SendEvents OnPlayerKill;
    #endregion

    #region Events Int
    public delegate void SendEventsInt(int score);
    public static event SendEventsInt OnEnemyKillScore;
    #endregion

    [SerializeField]
    [Tooltip("What to trigger when enemy is dead?")]
    private UnityEvent OnEnemyDead = default;
    #endregion

    #endregion

    #region Private Variables
    private Animator _enemyAnim;
    private Rigidbody2D _rb2D;
    private DemonPatrol _demonPatrol;
    private Collider2D _col2D = default;
    [SerializeField] private float _cooldownTimer = default;
    [SerializeField] private bool _isPlayerDead = default;
    [SerializeField] private bool _isPlayerDashing = default;
    #endregion

    #region Unity Callbacks

    #region Events
    void OnEnable()
    {

    }

    void OnDisable()
    {

    }

    void OnDestroy()
    {

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

    /// <summary>
    /// Tied to Enemy E2_Atk_Anim Anim Event;
    /// Kills the Player and updates the game that the Player is dead;
    /// </summary>
    public void KillPlayer()
    {
        if (PlayerInSight() && !_isPlayerDashing)
        {
            OnPlayerKill?.Invoke();
            Debug.Log("Killing Player");
        }
    }
    #endregion

    #region Events
    /// <summary>
    /// Subbed to Event from PlayerMovementV2 Script;
    /// Disables Enemy to kill the Player;
    /// </summary>
    /// <param name="isDashing"> If True, Player immune else Player not immune; </param>
    void OnPlayerDashEventReceived(bool isDashing)
    {
        if (isDashing)
            _isPlayerDashing = true;
        else
            _isPlayerDashing = false;
    }

    /// <summary>
    /// Subbed to PlayerMovementV2 script;
    /// Lets the Enemy Script know that the player is Dead;
    /// </summary>
    void OnPlayerDeadEventReceived() => _isPlayerDead = true;
    #endregion
}