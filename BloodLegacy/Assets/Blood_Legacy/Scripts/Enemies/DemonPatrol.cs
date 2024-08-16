using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonPatrol : MonoBehaviour
{
    #region Serialized Variables

    #region Transforms
    [Space, Header("Transforms")]
    [SerializeField]
    [Tooltip("The right side of the Enemy Move Position")]
    private Transform rightEdge = default;

    [SerializeField]
    [Tooltip("The left side of the Enemy Move Position")]
    private Transform leftEdge = default;

    [SerializeField]
    [Tooltip("The Enemy Transform Position")]
    private Transform enemy = default;
    #endregion

    #region Floats
    [Space, Header("Floats")]
    [SerializeField]
    [Tooltip("Enemy movement Speed")]
    private float speed = default;

    [SerializeField]
    [Tooltip("How long to idle when reaching the end points")]
    private float idleDuration = default;
    #endregion

    #region Others
    private Animator demonAnim = default;

    public bool IsMovingLeft { get => _isMovingLeft; set => _isMovingLeft = value; }
    private bool _isMovingLeft;

    public bool IsDemonAlive { get => _isDemonAlive; set => _isDemonAlive = value; }
    [SerializeField] private bool _isDemonAlive = true;
    #endregion

    #endregion

    #region Private Variables
    private float idleTimer = default;
    private Vector3 initScale; //initial scale
    #endregion

    #region Unity Callbacks

    #region Events
    //void OnEnable()
    //{

    //}

    //void OnDisable() => demonAnim.SetBool("isMoving", false);

    //void OnDestroy()
    //{

    //}
    #endregion

    void Start()
    {
        initScale = enemy.localScale;
        IsDemonAlive = true;
        demonAnim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (IsDemonAlive)
            DemonMoveCheck();
    }
    #endregion

    #region My Functions
    /// <summary>
    /// Movement Function for the Demon Moving;
    /// </summary>
    void DemonMoveCheck()
    {
        if (_isMovingLeft)
        {
            if (enemy.position.x >= leftEdge.position.x)
                MoveInDirection(-1);
            else
                DirectionChange();
        }
        else
        {
            if (enemy.position.x <= rightEdge.position.x)
                MoveInDirection(1);
            else
                DirectionChange();
        }
    }

    /// <summary>
    /// Changes Direction when reaches one end of the waypoint;
    /// </summary>
    void DirectionChange()
    {
        demonAnim.SetBool("isMoving", false);

        idleTimer += Time.deltaTime;

        if (idleTimer > idleDuration)
            _isMovingLeft = !_isMovingLeft;
    }

    /// <summary>
    /// Changes the direction of the Demon Enemy when reaching end of the waypoint;
    /// </summary>
    /// <param name="_direction"> If direction -1, move left. If direction 1, move right; </param>
    void MoveInDirection(int _direction)
    {
        idleTimer = 0;
        demonAnim.SetBool("isMoving", true);

        //Make enemy face direction
        enemy.localScale = new Vector3(Mathf.Abs(initScale.x) * _direction, initScale.y, initScale.z);
        //Move in that direction
        enemy.position = new Vector3(enemy.position.x + Time.deltaTime * _direction * speed, enemy.position.y, enemy.position.z);
    }
    #endregion

    #region Coroutines

    #endregion

    #region Events

    #endregion
}