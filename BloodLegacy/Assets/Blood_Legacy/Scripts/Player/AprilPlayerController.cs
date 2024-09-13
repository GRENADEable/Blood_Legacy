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

    #region Player Dashing
    [Space, Header("Player Dashing")]
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

    #region Events

    #region Void Events
    public delegate void SendEvents();
    /// <summary>
    /// Event sent from AprilPlayerController script to Enemy script;
    /// Lets the enemies know that the Player is dead;
    /// </summary>
    public static event SendEvents OnPlayerDead;

    /// <summary>
    /// Event sent from AprilPlayerController script to GameManager Script;
    /// For shooting Enemies, an Event shortcut to update UI;
    /// </summary>
    public static event SendEvents OnPlayerKill;

    /// <summary>
    /// Event sent from AprilPlayerController script to AudioManager Script;
    /// For shooting Enemies, an Event shortcut to update UI;
    /// </summary>
    public static event SendEvents OnSwordSwipe;
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
    [SerializeField] private bool _isFacingRight = true;
    private float _horizontalMoveX = default;
    private Vector2 _moveDirection2D = default;

    [Header("Dash Mechanic")]
    [SerializeField] private bool _canDash = true;
    [SerializeField] private bool _isPlayerDashing;
    private TrailRenderer _playerTrail = default;

    [Header("Player Componenets")]
    private Rigidbody2D _rb2D = default;
    private Animator _playerAnim = default;
    [SerializeField] private bool _isPlayerDead = default;
    [SerializeField] private bool _isPlayerMoving = true;
    private RaycastHit2D _hit2D = default;
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
        _isPlayerMoving = true;
    }

    void OnDestroy()
    {
        DemonEnemy.OnPlayerKill -= OnPlayerKillEventReceived;
    }
    #endregion

    void Start()
    {
        _playerAnim = GetComponent<Animator>();
        _rb2D = GetComponent<Rigidbody2D>();
        _playerTrail = GetComponentInChildren<TrailRenderer>();
        _isPlayerDead = false;
    }

    void Update()
    {
        if (_isPlayerDead || _isPlayerDashing)
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
        _moveDirection2D = new Vector2(_horizontalMoveX, 0f).normalized;

        //if (!_isPlayerMoving)
        //    _rb2D.velocity = Vector2.zero;
        //else
        _rb2D.velocity = new Vector2(_moveDirection2D.x * playerSpeed, _rb2D.velocity.y);
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
        _hit2D = Physics2D.BoxCast(boxCollider.bounds.center + colliderDistance * range * transform.localScale.x * transform.right,
            new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z), 0, Vector2.left, 0, enemyLayer);

        return _hit2D.collider != null;
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
    /// <summary>
    /// Player Dash with Cooldowns;
    /// </summary>
    /// <returns> Float delay; </returns>
    private IEnumerator Dash()
    {
        _canDash = false;
        _isPlayerDashing = true;
        OnPlayerDash?.Invoke(_isPlayerDashing);
        _playerAnim.SetBool("isDashing", true);

        if (_isFacingRight)
            _rb2D.AddForce(Vector2.right * dashPower, ForceMode2D.Impulse);
        else
            _rb2D.AddForce(Vector2.left * dashPower, ForceMode2D.Impulse);

        _playerTrail.emitting = true;
        yield return new WaitForSeconds(dashingTime);
        _playerTrail.emitting = false;
        _isPlayerDashing = false;
        OnPlayerDash?.Invoke(_isPlayerDashing);
        _playerAnim.SetBool("isDashing", false);
        yield return new WaitForSeconds(dashingCooldown);
        _canDash = true;
    }
    #endregion

    #region Events

    #region Input Systems
    /// <summary>
    /// Player Movement Script tied to C_April PlayerInput;
    /// Using new Input System to move;
    /// </summary>
    public void OnMovePlayer(InputAction.CallbackContext context) => _horizontalMoveX = context.ReadValue<Vector2>().x;

    /// <summary>
    /// Player Movement Script tied to C_April PlayerInput;
    /// Using new Input System to jump;
    /// </summary>
    public void OnJumpPlayer(InputAction.CallbackContext context)
    {
        if (context.performed && IsPlayerGrounded() && _isPlayerMoving && !_isPlayerDashing)
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
        if (context.started && !_isPlayerDead && _isPlayerMoving && !_isPlayerDashing)
        {
            _playerAnim.SetTrigger("isAttacking");
            //Debug.Log("Attacking");
        }
    }

    /// <summary>
    /// Player Movement Script tied to C_April PlayerInput;
    /// Using new Input System to dash;
    /// </summary>
    public void OnDashPlayer(InputAction.CallbackContext context)
    {
        if (context.started && _canDash && _isPlayerMoving)
        {
            StartCoroutine(Dash());
            Debug.Log("Dashing");
        }
    }

    /// <summary>
    /// Player Movement Script tied to C_April PlayerInput;
    /// Using new Input System to block;
    /// </summary>
    public void OnBlockPlayer(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _playerAnim.SetBool("isBlocking", true);
            _isPlayerMoving = false;
        }

        if (context.canceled)
        {
            _playerAnim.SetBool("isBlocking", false);
            _isPlayerMoving = true;
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
        Destroy(this.gameObject);
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
    public void OnPlayerSwordSwipe() => OnSwordSwipe?.Invoke();

    /// <summary>
    /// Tied to AnimEvent on C_April_Attack_Anim;
    /// Checks if the Demon is within the attack range and Kills it;
    /// </summary>
    public void OnEnemyHit()
    {
        if (EnemyInSight())
        {
            if (_hit2D.collider.GetComponent<DemonEnemy>() != null)
                _hit2D.collider.GetComponent<DemonEnemy>().EnemyKill();
        }
    }

    public void OnPanelActive() => _isPlayerMoving = true;

    #endregion
}