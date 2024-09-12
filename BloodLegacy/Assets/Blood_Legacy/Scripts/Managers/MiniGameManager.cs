using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameManager : MonoBehaviour
{
    #region Serialized Variables
    [SerializeField]
    [Tooltip("After how many patrol demons to kill to spawn chase demons?")]
    private int totalDemonsKilled = default;

    [SerializeField]
    [Tooltip("ChaseDemon Prefab GameObject")]
    private GameObject chaseDemonPrefab = default;

    [SerializeField]
    [Tooltip("Player Prefab GameObject in Scene")]
    private GameObject playerPrefab = default;

    [SerializeField]
    [Tooltip("Chase Demon Transform Positions")]
    private Transform[] chaseDemonSpawnPos = default;
    #endregion

    #region Private Variables
    [SerializeField] private int _currEnemyKilled = default;
    private int _chaseDemonSpawnIndex = default;
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
    void SpawnChaseDemon()
    {
        int spawnIndex = Random.Range(0, chaseDemonSpawnPos.Length);
        GameObject chaseDemonObj = Instantiate(chaseDemonPrefab, chaseDemonSpawnPos[spawnIndex].position, Quaternion.identity, chaseDemonSpawnPos[spawnIndex]);
        chaseDemonObj.GetComponent<DemonChase>().Player = playerPrefab;
    }
    #endregion

    #region Coroutines

    #endregion

    #region Events
    /// <summary>
    /// Subbed to event from DemonEnemy and DemonChase;
    /// Spawns the next Demon after certain amount killed;
    /// </summary>
    void OnEnemyKillScoreEventReceived(int score)
    {
        _currEnemyKilled++;

        if (_currEnemyKilled >= totalDemonsKilled)
            SpawnChaseDemon();
    }
    #endregion
}