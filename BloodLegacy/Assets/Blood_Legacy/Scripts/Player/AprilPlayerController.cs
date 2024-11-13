using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AprilPlayerController : MonoBehaviour
{
    #region Serialized Fields

    #region Datas
    [Space, Header("Datas")]
    [SerializeField]
    [Tooltip("Current State of the Player")]
    private PlayerState currState = PlayerState.Moving;
    private enum PlayerState { Moving, Jumping, Attacking, Blocking, Dashing, Dead };
    #endregion

    #region Movement
    [Space, Header("Movement")]
    [SerializeField]
    [Tooltip("Player movement speed")]
    private float playerSpeed = default;

    [SerializeField]
    [Tooltip("Player jump power")]
    private float jumpPower = default;
    #endregion

    #region Player Health
    [Space, Header("Player Health")]
    [SerializeField]
    [Tooltip("Max Player Health")]
    private int maxPlayerHealth = default;

    [SerializeField]
    [Tooltip("How long will the Player be invincible when taken Damage?")]
    private float damageInvincibility = 0.5f;
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

    #region FXs
    [Space, Header("FXs")]
    [SerializeField]
    [Tooltip("Block Particle Effects")]
    private GameObject blockFXPrefab = default;

    [SerializeField]
    [Tooltip("Block FX Spawn Position")]
    private Transform blockFXSpawnPos = default;

    [SerializeField]
    [Tooltip("Dash Particle Effects")]
    private GameObject dashFXPrefab = default;

    [SerializeField]
    [Tooltip("Block FX Spawn Position")]
    private Transform dashFXSpawnPos = default;
    #endregion

    #region Events

    #region Void Events
    public delegate void SendEvents();
    /// <summary>
    /// Event sent from AprilPlayerController script to Demon and MiniGamesManager Script;
    /// Lets the Demon know that the Player is dead and restart the game;
    /// </summary>
    public static event SendEvents OnPlayerDead;

    /// <summary>
    /// Event sent from AprilPlayerController script to GameManager Script;
    /// For killing Enemies, an Event shortcut to update UI;
    /// </summary>
    public static event SendEvents OnPlayerKill;

    /// <summary>
    /// Event sent from AprilPlayerController script to GameManager Script;
    /// For damaging the Player, update the health UI;
    /// </summary>
    public static event SendEvents OnPlayerHit;

    /// <summary>
    /// Event sent from AprilPlayerController script to AudioManager Script;
    /// For killing Enemies, an Event shortcut to update UI;
    /// </summary>
    public static event SendEvents OnSwordSwipe;
    #endregion

    #region Bool Events
    public delegate void SendEventsBool(bool flag);
    ///// <summary>
    ///// Event sent from PlayerMovementV2 script to Enemy Script;
    ///// Lets the enemies know that the Player is dashing;
    ///// </summary>
    //public static event SendEventsBool OnPlayerDash;

    ///// <summary>
    ///// Event sent from PlayerMovementV2 script to Enemy Script;
    ///// Lets the enemies know that the Player is Blocking;
    ///// </summary>
    //public static event SendEventsBool OnPlayerBlock;

    /// <summary>
    /// Event sent from PlayerMovementV2 script to Demon Script;
    /// Informs the Demons that the Player is Invincible;
    /// </summary>
    public static event SendEventsBool OnPlayerInvincible;
    #endregion

    #endregion

    #endregion

    #region Private Variables
    [Header("Movement")]
    private bool _isFacingRight = true;
    private float _horizontalMoveX = default;
    private Vector2 _moveDirection2D = default;
    [SerializeField] private bool _isPlayerDamaged = default;

    [Header("Dash Mechanic")]
    [SerializeField] private bool _canDash = true;
    private TrailRenderer _playerTrail = default;

    [Header("Player Componenets")]
    private Rigidbody2D _rb2D = default;
    private Animator _playerAnim = default;
    [SerializeField] private bool _isPlayerMoving = true;
    private RaycastHit2D _hit2D = default;
    private int _currPlayerHealth = 0;
    #endregion

    #region Unity Callbacks

    #region Events
    void OnEnable()
    {
        DemonDefault.OnPlayerDamage += OnPlayerDamageEventReceived;

        DemonChase.OnPlayerDamage += OnPlayerDamageEventReceived;
        DemonChase.OnPlayerBlock += OnPlayerBlockEventReceived;
    }

    void OnDisable()
    {
        DemonDefault.OnPlayerDamage -= OnPlayerDamageEventReceived;

        DemonChase.OnPlayerDamage -= OnPlayerDamageEventReceived;
        DemonChase.OnPlayerBlock -= OnPlayerBlockEventReceived;
    }

    void OnDestroy()
    {
        DemonDefault.OnPlayerDamage -= OnPlayerDamageEventReceived;

        DemonChase.OnPlayerDamage -= OnPlayerDamageEventReceived;
        DemonChase.OnPlayerBlock -= OnPlayerBlockEventReceived;
    }
    #endregion

    void Start()
    {
        _playerAnim = GetComponent<Animator>();
        _rb2D = GetComponent<Rigidbody2D>();
        _playerTrail = GetComponentInChildren<TrailRenderer>();
        _currPlayerHealth = maxPlayerHealth;
    }

    void Update()
    {
        if (currState == PlayerState.Dead || currState == PlayerState.Blocking || (currState == PlayerState.Attacking && IsPlayerGrounded()))
        {
            _rb2D.velocity = Vector2.zero;
            return;
        }

        if (currState == PlayerState.Dashing)
            return;

        if (currState == PlayerState.Moving || currState == PlayerState.Jumping)
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

    #region Gameplay
    /// <summary>
    /// Tied to OnCharEnable Event on MMF_MiniGame_Intro;
    /// Enables the input after the cutscene;
    /// </summary>
    public void OnAprilMove()
    {
        currState = PlayerState.Moving;
        _isPlayerMoving = true;
    }

    /// <summary>
    /// Tied to OnCharDisable Event on MMF_MiniGame_Start;
    /// Pauses the input on the cutscene;
    /// </summary>
    public void OnAprilPause()
    {
        currState = PlayerState.Dead;
        _rb2D.velocity = Vector2.zero;
        _playerAnim.Play("C_April_Idle_V2_Anim");
        _playerAnim.SetBool("isBlocking", false);
        _isPlayerMoving = false;
    }

    /// <summary>
    /// Tied to OnCharDisable Event on MMF_MiniGame_Outro;
    /// Disables the input;
    /// </summary>
    public void OnAprilFreeze()
    {
        currState = PlayerState.Dead;
        _isPlayerMoving = false;
        gameObject.SetActive(false);
    }
    #endregion

    #region Player
    public void OnJumpStarted()
    {
        _rb2D.velocity = new Vector2(_rb2D.velocity.x, jumpPower);
        currState = PlayerState.Jumping;
    }

    public void OnJumpEnded() => currState = PlayerState.Moving;

    public void OnDeathAnimEnded()
    {
        Debug.Log("Anim Reset");
        GetComponent<SpriteRenderer>().enabled = false;
    }

    /// <summary>
    /// Player Movement function;
    /// </summary>
    void PlayerMove()
    {
        _moveDirection2D = new Vector2(_horizontalMoveX, 0f).normalized;
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

    /// <summary>
    /// Damages the Player and adds invincibility buffer for few seconds;
    /// </summary>
    void PlayerDamaged() => StartCoroutine(DamageBuffer());

    /// <summary>
    /// Kills the Player and notifies the demons to become idle;
    /// Also resets the Player state;
    /// </summary>
    void PlayerKilled()
    {
        OnPlayerDead?.Invoke();
        _playerAnim.SetTrigger("isDead");
        _currPlayerHealth = maxPlayerHealth;
        _playerAnim.Play("EmptyDamage");
        _playerAnim.Play("April_Blend");
        _playerAnim.Play("EmptyDeath");
        _canDash = true;
        _isPlayerDamaged = false;
        currState = PlayerState.Dead;
        this.enabled = false;
        _rb2D.velocity = Vector2.zero;
        GetComponent<SpriteRenderer>().enabled = true;
        //gameObject.SetActive(false);
    }
    #endregion

    #region Cheats
    /// <summary>
    /// Subbed to evnet from MMF_MiniGame_Cheats;
    /// Enables the Player Movement;
    /// </summary>
    public void OnMiniGameToggle()
    {
        currState = PlayerState.Moving;
        _isPlayerMoving = true;
    }
    #endregion

    #region FX

    #endregion

    #endregion

    #region Coroutines
    /// <summary>
    /// Player Dash with Cooldowns;
    /// </summary>
    /// <returns> Float delay; </returns>
    private IEnumerator Dash()
    {
        _canDash = false;
        //_isPlayerDashing = true;
        currState = PlayerState.Dashing;
        //OnPlayerDash?.Invoke(true);
        OnPlayerInvincible?.Invoke(true);
        _playerAnim.SetBool("isDashing", true);

        if (_isFacingRight)
            _rb2D.AddForce(Vector2.right * dashPower, ForceMode2D.Impulse);
        else
            _rb2D.AddForce(Vector2.left * dashPower, ForceMode2D.Impulse);

        _playerTrail.emitting = true;
        yield return new WaitForSeconds(dashingTime);
        Instantiate(dashFXPrefab, dashFXSpawnPos.position, Quaternion.identity);
        _playerTrail.emitting = false;
        //_isPlayerDashing = false;
        currState = PlayerState.Moving;
        //OnPlayerDash?.Invoke(false);
        OnPlayerInvincible?.Invoke(false);
        _playerAnim.SetBool("isDashing", false);
        yield return new WaitForSeconds(dashingCooldown);
        _canDash = true;
    }

    /// <summary>
    /// Player Damage with Cooldown;
    /// </summary>
    /// <returns> Float delay; </returns>
    private IEnumerator DamageBuffer()
    {
        _isPlayerDamaged = true;
        OnPlayerInvincible?.Invoke(true);
        OnPlayerHit?.Invoke();
        _playerAnim.SetBool("isDamaged", true);
        yield return new WaitForSeconds(damageInvincibility);

        if (currState != PlayerState.Blocking)
            OnPlayerInvincible?.Invoke(false);

        _isPlayerDamaged = false;
        _playerAnim.SetBool("isDamaged", false);
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
        if (context.performed && IsPlayerGrounded() && _isPlayerMoving && currState != PlayerState.Blocking)
            _playerAnim.SetTrigger("isJumping");

        if (context.canceled && _rb2D.velocity.y > 0f)
            _rb2D.velocity = new Vector2(_rb2D.velocity.x, _rb2D.velocity.y * 0.5f);
    }

    /// <summary>
    /// Player Movement Script tied to C_Apri PlayerInput;
    /// Using new Input System to attack;
    /// </summary>
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started && currState == PlayerState.Moving)
        {
            _playerAnim.SetTrigger("isAttacking");
            currState = PlayerState.Attacking;
            //Debug.Log("Attacking");
        }
    }

    /// <summary>
    /// Player Movement Script tied to C_April PlayerInput;
    /// Using new Input System to dash;
    /// </summary>
    public void OnDashPlayer(InputAction.CallbackContext context)
    {
        if (context.started && _canDash && currState == PlayerState.Moving)
        {
            StartCoroutine(Dash());
            //Debug.Log("Dashing");
        }
    }

    /// <summary>
    /// Player Movement Script tied to C_April PlayerInput;
    /// Using new Input System to block;
    /// </summary>
    public void OnBlockPlayer(InputAction.CallbackContext context)
    {
        if (context.started && currState == PlayerState.Moving && IsPlayerGrounded())
        {
            _playerAnim.SetBool("isBlocking", true);
            currState = PlayerState.Blocking;
            OnPlayerInvincible?.Invoke(true);
        }

        if (context.canceled && currState == PlayerState.Blocking)
        {
            _playerAnim.SetBool("isBlocking", false);
            currState = PlayerState.Moving;
            OnPlayerInvincible?.Invoke(false);
        }

    }
    #endregion

    #region Player
    /// <summary>
    /// Subbed to Event from Demon Script;
    /// Damages the Player;
    /// </summary>
    void OnPlayerDamageEventReceived()
    {
        _currPlayerHealth--;

        if (_currPlayerHealth > 0)
        {
            if (!_isPlayerDamaged)
                PlayerDamaged();
        }
        else if (_currPlayerHealth <= 0)
        {
            if (!_isPlayerDamaged)
                PlayerKilled();
        }
    }

    /// <summary>
    /// Tied to AnimEvent on C_April_Attack_Anim;
    /// Tops the movement when Player when attack starts;
    /// </summary>
    public void OnPlayerAttackStart()
    {
        _isPlayerMoving = false;
        currState = PlayerState.Attacking;
    }

    /// <summary>
    /// Tied to AnimEvent on C_April_Attack_Anim;
    /// Tops the movement when Player when attack ends;
    /// </summary>
    public void OnPlayerAttackEnd()
    {
        _isPlayerMoving = true;
        currState = PlayerState.Moving;
    }

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
            if (_hit2D.collider.GetComponent<DemonDefault>() != null)
                _hit2D.collider.GetComponent<DemonDefault>().EnemyKill();

            if (_hit2D.collider.GetComponent<DemonChase>() != null)
                _hit2D.collider.GetComponent<DemonChase>().EnemyKill();
        }
    }
    #endregion

    #region FXs
    /// <summary>
    /// Subbed to Event from Demon Script;
    /// Plays FX Particle for Blocking;
    /// </summary>
    void OnPlayerBlockEventReceived()
    {
        if (currState == PlayerState.Blocking)
            Instantiate(blockFXPrefab, blockFXSpawnPos.position, Quaternion.identity);
    }
    #endregion

    #endregion
}