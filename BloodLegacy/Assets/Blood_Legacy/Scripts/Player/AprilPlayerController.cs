using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SocialPlatforms;

public class AprilPlayerController : MonoBehaviour
{
    #region Serialized Fields

    #region Movement
    [Space, Header("Movement")]
    [SerializeField]
    [Tooltip("Player movement speed")]
    private float playerSpeed = default;

    [SerializeField]
    [Tooltip("Player jump power")]
    private float jumpPower = default;
    #endregion

    #region Ground Check
    [Space, Header("Ground Check")]
    [SerializeField]
    [Tooltip("What ground layer do you want the player to move on?")]
    private LayerMask groundLayer = default;

    [SerializeField]
    [Tooltip("Circle radius value")]
    private float circleRadius = default;
    #endregion

    #region Player Throwing
    [Space, Header("Player Throwing")]
    [SerializeField]
    [Tooltip("The projectile Prefab itself")]
    private GameObject projectilePrefab = default;

    [SerializeField]
    [Tooltip("From where do you want to spawn the projectile?")]
    private Transform throwPos = default;

    [SerializeField]
    [Tooltip("Throw Power")]
    private float throwPower = default;

    [SerializeField]
    [Tooltip("After how many seconds do you want to destroy the projectile?")]
    private float projectileDestroyTime = default;
    #endregion

    #region Player Attacking
    [Space, Header("Player Attacking")]
    [SerializeField]
    [Tooltip("Trigger Attack Box Collider")]
    private BoxCollider2D boxCollider;

    [SerializeField]
    [Tooltip("Distance of the Collider from the Player")]
    private float colliderDistance;

    [SerializeField]
    [Tooltip("Width of the Collider")]
    private float range = default;

    [SerializeField]
    [Tooltip("The Enemy Layer for attacking")]
    private LayerMask enemyLayer;
    #endregion

    #region Audios
    [Space, Header("Audios")]
    [SerializeField]
    [Tooltip("Audio Source for Player SFX")]
    private AudioSource sfxAud = default;

    [SerializeField]
    [Tooltip("SFX Audio Clips")]
    private AudioClip[] sfxClips = default;
    #endregion

    #region Events

    #region Void Events

    public delegate void SendEvents();
    /// <summary>
    /// Event sent from PlayerMovementV2 script to Enemy script;
    /// Lets the enemies know that the Player is dead;
    /// </summary>
    public static event SendEvents OnPlayerDead;

    /// <summary>
    /// Event sent from PlayerMovementV2 script to GameManager Script;
    /// For shooting Enemies, an Event shortcut to update UI;
    /// </summary>
    public static event SendEvents OnPlayerKill;

    /// <summary>
    /// Event sent from PlayerMovementV2 script to GameManager Script;
    /// For shooting Enemies, an Event shortcut to update UI;
    /// </summary>
    public static event SendEvents OnEnemyKill;
    #endregion

    #endregion

    #endregion

    #region Private Variables
    [Header("Movement")]
    private bool _isFacingRight = true;
    private float _horizontalMoveX = default;

    [Header("Play Componenets")]
    private Rigidbody2D _rb2D = default;
    //private TrailRenderer _playerTrail = default;
    private Animator _playerAnim = default;
    private Collider2D _col2D = default;
    [SerializeField] private bool _isPlayerDead = default;
    [SerializeField] private bool _isPlayerMoving = true;
    #endregion

    #region Unity Callbacks

    #region Events
    void OnEnable()
    {
        DemonEnemy.OnPlayerKill += OnPlayerKillEventReceived;
    }

    void OnDisable()
    {
        DemonEnemy.OnPlayerKill -= OnPlayerKillEventReceived;
    }

    void OnDestroy()
    {
        DemonEnemy.OnPlayerKill -= OnPlayerKillEventReceived;
    }
    #endregion

    void Start()
    {
        _playerAnim = GetComponentInChildren<Animator>();
        _rb2D = GetComponent<Rigidbody2D>();
        //_playerTrail = GetComponentInChildren<TrailRenderer>();
        _isPlayerDead = false;
    }

    void Update()
    {
        if (_isPlayerDead)
            return;

        if (_isPlayerMoving)
        {
            PlayerMove();
            PlayerAnims();
        }
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
    /// Player Movement function;
    /// </summary>
    void PlayerMove()
    {
        Vector2 horizonalXVel = new Vector2(_horizontalMoveX, 0f).normalized;

        if (!_isPlayerMoving)
            _rb2D.velocity = Vector2.zero;
        else
            _rb2D.velocity = new Vector2(horizonalXVel.x * playerSpeed, _rb2D.velocity.y);
    }

    /// <summary>
    /// The Player Animations;
    /// </summary>
    void PlayerAnims()
    {
        _playerAnim.SetFloat("playerSpeed", _horizontalMoveX);

        if (!_isFacingRight && _horizontalMoveX > 0f)
            FlipPlayer();
        else if (_isFacingRight && _horizontalMoveX < 0f)
            FlipPlayer();
    }

    ///// <summary>
    ///// Spawns a projectile at a specific location when key is pressed;
    ///// </summary>
    //void Shoot()
    //{
    //    _canThrow = false;
    //    _canPickup = true;
    //    OnItemUsed?.Invoke();
    //    _playerAnim.SetTrigger("isThrowing");

    //    if (_isFacingRight)
    //    {
    //        GameObject projectileObj = Instantiate(projectilePrefab, throwPos.position, Quaternion.identity);
    //        projectileObj.GetComponent<Rigidbody2D>().AddForce(throwPos.right * throwPower, ForceMode2D.Impulse);
    //        Destroy(projectileObj, projectileDestroyTime);
    //    }
    //    else
    //    {
    //        GameObject projectileObj = Instantiate(projectilePrefab, throwPos.position, Quaternion.identity);
    //        projectileObj.GetComponent<Rigidbody2D>().AddForce(-throwPos.right * throwPower, ForceMode2D.Impulse);
    //        projectileObj.GetComponent<SpriteRenderer>().flipX = true;
    //        Destroy(projectileObj, projectileDestroyTime);
    //    }
    //}

    /// <summary>
    /// Bool ground check to see if the player is hitting the ground according to the layer;
    /// </summary>
    /// <returns> True, grounded. False, in the air and let gravity act upon it; </returns>
    bool IsPlayerGrounded() => Physics2D.OverlapCircle(transform.position, circleRadius, groundLayer);

    /// <summary>
    /// Bool check to see if the Enemy is within the Box Trigger Collider;
    /// </summary>
    /// <returns> If true, Enemy in sight, if false, Enemy out of sight; </returns>
    bool EnemyInSight()
    {
        RaycastHit2D hit = Physics2D.BoxCast(boxCollider.bounds.center + colliderDistance * range * transform.localScale.x * transform.right,
            new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z), 0, Vector2.left, 0, enemyLayer);

        return hit.collider != null;
    }

    /// <summary>
    /// Flips the X axis scale of the Player so it looks like its rotating.
    /// </summary>
    void FlipPlayer()
    {
        _isFacingRight = !_isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }
    #endregion

    #region Coroutines

    #endregion

    #region Events

    #region Input Systems
    /// <summary>
    /// Player Movement Script tied to C_Apri PlayerInput;
    /// Using new Input System to move;
    /// </summary>
    public void OnMovePlayer(InputAction.CallbackContext context) => _horizontalMoveX = context.ReadValue<Vector2>().x;

    /// <summary>
    /// Player Movement Script tied to C_Apri PlayerInput;
    /// Using new Input System to jump;
    /// </summary>
    public void OnJumpPlayer(InputAction.CallbackContext context)
    {
        if (context.performed && IsPlayerGrounded() && _isPlayerMoving)
            _rb2D.velocity = new Vector2(_rb2D.velocity.x, jumpPower);

        if (context.canceled && _rb2D.velocity.y > 0f)
            _rb2D.velocity = new Vector2(_rb2D.velocity.x, _rb2D.velocity.y * 0.5f);
    }

    /// <summary>
    /// Player Movement Script tied to C_Apri PlayerInput;
    /// Using new Input System to attack;
    /// </summary>
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started && !_isPlayerDead && _isPlayerMoving)
        {
            _playerAnim.SetTrigger("isAttacking");
            Debug.Log("Attacking");
        }
    }
    #endregion

    /// <summary>
    /// Subbed to Event from Enemy Script;
    /// Kills the Player;
    /// </summary>
    void OnPlayerKillEventReceived()
    {
        OnPlayerDead?.Invoke();
        _isPlayerDead = true;
        _rb2D.bodyType = RigidbodyType2D.Static;
        sfxAud.PlayOneShot(sfxClips[1]);
        //Destroy(this.gameObject);
    }

    /// <summary>
    /// Tied to AnimEvent on C_April_Attack_Anim;
    /// Toggles the movement of the Player when Attacking;
    /// </summary>
    public void OnPlayerAttacking() => _isPlayerMoving = !_isPlayerMoving;

    /// <summary>
    /// Tied to the AnimEvent on C_April_Attack_Anim;
    /// Plays the sword swipping SFX;
    /// </summary>
    public void OnPlayerSwordSwipe()
    {
        sfxAud.PlayOneShot(sfxClips[2]);
    }

    public void OnEnemyHit()
    {
        if (EnemyInSight())
        {
            OnEnemyKill?.Invoke();
            Debug.Log("Killing Enemy");
        }
    }

    public void OnPanelActive()
    {
        _isPlayerMoving = true;
    }

    #endregion
}