using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniGameManager : MonoBehaviour
{
    #region Serialized Variables

    #region GameObjects
    [Space, Header("GameObjects")]
    [SerializeField]
    [Tooltip("ChaseDemon Prefab GameObject")]
    private GameObject chaseDemonPrefab = default;

    [SerializeField]
    [Tooltip("Player Prefab GameObject in Scene")]
    private GameObject playerPrefab = default;
    #endregion

    #region Ints and Floats
    [Space, Header("Ints and Floats")]
    [SerializeField]
    [Tooltip("After how many demons to kill to goto the next panel?")]
    private int totalDemonsKilled = default;

    [SerializeField]
    [Tooltip("How long do you want to Fade In the Red Tint?")]
    private float redTintFadeDelay = default;
    #endregion

    #region Others
    [Space, Header("Others")]
    [SerializeField]
    [Tooltip("Chase Demon Transform Positions")]
    private Transform[] chaseDemonSpawnPos = default;

    [SerializeField]
    [Tooltip("Red Tint Post Process Anim Controller")]
    private Animator redTintAnim = default;

    [SerializeField]
    [Tooltip("Red Tint BG Img")]
    private Image redTintBG = default;
    #endregion

    #endregion

    #region Private Variables
    private int _currDemonsKilled = default;
    private List<GameObject> _totalDemonObjs = new List<GameObject>();
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
        _currDemonsKilled++;
        redTintAnim.SetTrigger("isTintTriggered");

        if (_totalDemonObjs.Count <= totalDemonsKilled)
        {
            int spawnNumber = Random.Range(1, 3);
            for (int i = 0; i < spawnNumber; i++)
                SpawnChaseDemon();
        }

        if (_currDemonsKilled >= totalDemonsKilled)
        {
            for (int i = 0; i < _totalDemonObjs.Count; i++)
                _totalDemonObjs[i].SetActive(false);

            redTintBG.DOFade(1, redTintFadeDelay);
        }
    }
    #endregion
}