using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameManager : MonoBehaviour
{
    #region Serialized Variables
    //[SerializeField]
    //[Tooltip("")]
    #endregion

    #region Private Variables

    #endregion

    #region Unity Callbacks

    #region Events
    void OnEnable()
    {
        DemonEnemy.OnEnemyKillScore += OnEnemyKillScoreEventReceived;
    }

    void OnDisable()
    {
        DemonEnemy.OnEnemyKillScore -= OnEnemyKillScoreEventReceived;
    }

    void OnDestroy()
    {
        DemonEnemy.OnEnemyKillScore -= OnEnemyKillScoreEventReceived;
    }
    #endregion

    void Start()
    {

    }

    void Update()
    {

    }
    #endregion

    #region My Functions

    #endregion

    #region Coroutines

    #endregion

    #region Events
    void OnEnemyKillScoreEventReceived(int scroe)
    {
        Debug.Log("Enemy Score ++");
    }
    #endregion
}