using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameManager : MonoBehaviour
{
    #region Serialized Variables
    [SerializeField]
    [Tooltip("After how many demons to kill to goto the next panel?")]
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
    [SerializeField] private List<GameObject> _totalDemonObjs = new List<GameObject>();
    #endregion

    #region Unity Callbacks

    #region Events
    void OnEnable()
    {
        DemonDefault.OnEnemyKillScore += OnEnemyKillScoreEventReceived;

        DemonChase.OnEnemyKillScore += OnEnemyKillScoreEventReceived;
    }

    void OnDisable()
    {
        DemonDefault.OnEnemyKillScore -= OnEnemyKillScoreEventReceived;

        DemonChase.OnEnemyKillScore -= OnEnemyKillScoreEventReceived;
    }

    void OnDestroy()
    {
        DemonDefault.OnEnemyKillScore -= OnEnemyKillScoreEventReceived;

        DemonChase.OnEnemyKillScore -= OnEnemyKillScoreEventReceived;
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
    /// <summary>
    /// Spawns the Chase Demon;
    /// </summary>
    void SpawnChaseDemon()
    {
        int spawnIndex = Random.Range(0, chaseDemonSpawnPos.Length);
        GameObject chaseDemonObj = Instantiate(chaseDemonPrefab, chaseDemonSpawnPos[spawnIndex].position, Quaternion.identity, chaseDemonSpawnPos[spawnIndex]);
        chaseDemonObj.GetComponent<DemonChase>().Player = playerPrefab;

        _totalDemonObjs.Add(chaseDemonObj);

        Debug.Log("Spawning Chase Demon");
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

        if (_totalDemonObjs.Count <= totalDemonsKilled)
        {
            int spawnNumber = Random.Range(1, 3);
            for (int i = 0; i < spawnNumber; i++)
                SpawnChaseDemon();
        }
    }
    #endregion
}