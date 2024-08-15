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
    [Space, Header("Others")]
    private bool _isMovingLeft;
    public bool IsMovingLeft { get => _isMovingLeft; set => _isMovingLeft = value; }
    [SerializeField] private Animator demonAnim = default;
    #endregion

    #endregion

    #region Private Variables
    private float idleTimer = default;
    private Vector3 initScale; //initial scale
    #endregion

    #region Unity Callbacks

    #region Events
    void OnEnable()
    {

    }

    void OnDisable() => demonAnim.SetBool("isMoving", false);

    void OnDestroy()
    {

    }
    #endregion

    void Start()
    {
        initScale = enemy.localScale;
        demonAnim = GetComponentInChildren<Animator>();
    }

    void Update()
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
    #endregion

    #region My Functions
    void DirectionChange()
    {
        demonAnim.SetBool("isMoving", false);

        idleTimer += Time.deltaTime;

        if (idleTimer > idleDuration)
            _isMovingLeft = !_isMovingLeft;
    }

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