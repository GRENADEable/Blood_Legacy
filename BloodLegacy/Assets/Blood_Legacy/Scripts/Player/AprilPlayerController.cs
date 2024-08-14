using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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

    #region Dash Mechanic
    [Space, Header("Dash Mechanic")]
    [SerializeField]
    [Tooltip("How much power to dash?")]
    private float dashPower = 12f;

    [SerializeField]
    [Tooltip("Dash duration")]
    private float dashingTime = 0.2f;

    [SerializeField]
    [Tooltip("Dash cooldown")]
    private float dashingCooldown = 1f;
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

    #region Events

    #region Void Events

    public delegate void SendEvents();
    /// <summary>
    /// Event sent from PlayerMovementV2 script to GameManager Level 1, 2, 3, 4, 5, 6 and 7 Scripts;
    /// Updates the UI when item is picked;
    /// </summary>
    public static event SendEvents OnItemPicked;

    /// <summary>
    /// Event sent from PlayerMovementV2 script to GameManager Level 1, 2, 3, 4, 5, 6 and 7 Scripts;
    /// Updates the UI when item is used;
    /// </summary>
    public static event SendEvents OnItemUsed;

    /// <summary>
    /// Event sent from PlayerMovementV2 script to Enemy script;
    /// Lets the enemies know that the Player is dead;
    /// </summary>
    public static event SendEvents OnPlayerDead;

    /// <summary>
    /// Event sent from PlayerMovementV2 script to GameManager Level 1, 2, 3, 4, 5, 6 and 7 Scripts;
    /// For shooting Enemies, an Event shortcut to update UI;
    /// </summary>
    public static event SendEvents OnPlayerKill;
    #endregion

    #region Bool Events
    public delegate void SendEventsBool(bool flag);
    /// <summary>
    /// Event sent from PlayerMovementV2 script to Enemy Script;
    /// Lets the enemies know that the Player is dashing;
    /// </summary>
    public static event SendEventsBool OnPlayerDash;
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
        _playerAnim = GetComponentInChildren<Animator>();
        _rb2D = GetComponent<Rigidbody2D>();
        //_playerTrail = GetComponentInChildren<TrailRenderer>();
        _isPlayerDead = false;
    }

    void Update()
    {
        if (!_isPlayerDead)
            PlayerMove();
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

    void Attack()
    {
        _playerAnim.SetTrigger("isAttacking");
    }

    /// <summary>
    /// Bool ground check to see if the player is hitting the ground according to the layer;
    /// </summary>
    /// <returns> True, grounded. False, in the air and let gravity act upon it; </returns>
    bool IsPlayerGrounded() => Physics2D.OverlapCircle(transform.position, circleRadius, groundLayer);

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
        if (context.performed && IsPlayerGrounded())
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
        if (context.started && !_isPlayerDead)
        {
            Attack();
            Debug.Log("Attacking");
        }
    }

    /// <summary>
    /// Player Movement Script tied to C_Apri PlayerInput;
    /// Using new Input System to pickup;
    /// </summary>
    public void OnPickItem(InputAction.CallbackContext context)
    {
        if (context.started && _col2D != null)
        {
            Destroy(_col2D.gameObject);
            _col2D = null;
            OnItemPicked?.Invoke();
        }
    }
    #endregion

    /// <summary>
    /// Subbed to Event from Enemy Script;
    /// Kills the Player;
    /// </summary>
    void OnPlayerKillEventReceived()
    {
        _rb2D.bodyType = RigidbodyType2D.Static;
        _playerAnim.SetTrigger("isDead");
        _isPlayerDead = true;
        OnPlayerDead?.Invoke();
    }

    public void OnPlayerAttacking() => _isPlayerMoving = !_isPlayerMoving;

    #endregion
}