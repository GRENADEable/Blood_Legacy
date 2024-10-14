using MoreMountains.Feedbacks;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;

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

    [SerializeField]
    [Tooltip("How long do you want to Fade Out the Mini Game Tutorial World Canvas?")]
    private float miniGameCanvasFadeDelay = default;
    #endregion

    #region UIs
    [Space, Header("UIs")]
    [SerializeField]
    [Tooltip("The Mini Game Move Control UI World Canvas")]
    private CanvasGroup miniGameMoveCanvas = default;

    [SerializeField]
    [Tooltip("The Mini Game Dash Control UI World Canvas")]
    private CanvasGroup miniGameDashCanvas = default;

    [SerializeField]
    [Tooltip("The Mini Game Attack/Block Control UI World Canvas")]
    private CanvasGroup miniGameAtkBlockCanvas = default;
    #endregion

    #region Feel
    [Space, Header("Feel")]
    [SerializeField]
    [Tooltip("MMF_MiniGame_Restart Component to Restart the MiniGame")]
    private MMF_Player mmfMiniGameRestart = default;

    [SerializeField]
    [Tooltip("MMF_MiniGame_Restart Component to End the MiniGame")]
    private MMF_Player mmfMiniGameOutro = default;

    [SerializeField]
    [Tooltip("MMF_MiniGame_Cheats Component to for Mini Game Cheats")]
    private MMF_Player mmfMiniGameCheats = default;
    #endregion

    #region Others
    [Space, Header("Others")]
    [SerializeField]
    [Tooltip("Chase Demon Transform Positions")]
    private Transform[] chaseDemonSpawnPos = default;

    [SerializeField]
    [Tooltip("Red Tint Post Process Anim Controller")]
    private Animator redTintAnim = default;
    #endregion

    #region Cheats
    [Space, Header("Cheats")]
    [SerializeField]
    [Tooltip("Mini Games Panel GameObject")]
    private GameObject miniGamePanel = default;
    #endregion

    #region Events
    public delegate void SendEvents();
    /// <summary>
    /// Event sent from MiniGameManager to Demons;
    /// Changes the current state of the Demons to Chasing;
    /// </summary>
    public static event SendEvents OnDemonChase;

    /// <summary>
    /// Event sent from MiniGameManager and GameManager;
    /// Cheats for Enabling MiniGame;
    /// </summary>
    public static event SendEvents OnMiniGameEnable;
    #endregion

    #endregion

    #region Private Variables
    private int _currDemonsKilled = default;
    private List<GameObject> _totalDemonObjs = new List<GameObject>();
    private Vector3 _playerStartPos = default;
    #endregion

    #region Unity Callbacks

    #region Events
    void OnEnable()
    {
        DemonDefault.OnEnemyDead += OnEnemyDeadEventReceived;

        DemonChase.OnEnemyDead += OnEnemyDeadEventReceived;

        AprilPlayerController.OnPlayerDead += OnPlayerDeadEventReceived;
    }

    void OnDisable()
    {
        DemonDefault.OnEnemyDead -= OnEnemyDeadEventReceived;

        DemonChase.OnEnemyDead -= OnEnemyDeadEventReceived;

        AprilPlayerController.OnPlayerDead -= OnPlayerDeadEventReceived;
    }

    void OnDestroy()
    {
        DemonDefault.OnEnemyDead -= OnEnemyDeadEventReceived;

        DemonChase.OnEnemyDead -= OnEnemyDeadEventReceived;

        AprilPlayerController.OnPlayerDead -= OnPlayerDeadEventReceived;
    }
    #endregion

    void Start() => _playerStartPos = playerPrefab.transform.position;

    void Update()
    {

    }
    #endregion

    #region My Functions

    #region Gameplay
    /// <summary>
    /// Tied to MMF_MiniGame_Restart;
    /// Resets the Player Position;
    /// </summary>
    public void OnPlayerReset() => playerPrefab.transform.position = _playerStartPos;

    /// <summary>
    /// Tied to MMF_MiniGame_Restart;
    /// Resets the Game;
    /// </summary>
    public void OnGameReset()
    {
        for (int i = 0; i < _totalDemonObjs.Count; i++)
            Destroy(_totalDemonObjs[i]);

        _totalDemonObjs.Clear();
        _currDemonsKilled = 0;
        redTintAnim.Play("Empty");
    }

    /// <summary>
    /// Tied to MMF_MiniGame_Restart;
    /// Starts the game after the Reset;
    /// </summary>
    public void OnGameStart()
    {
        playerPrefab.SetActive(true);
        SpawnChaseDemon();
        OnDemonChase?.Invoke();
    }

    /// <summary>
    /// Tied to MMF_MiniGame_Outro;
    /// Ends the game after Winning;
    /// </summary>
    public void OnGameEnd()
    {
        for (int i = 0; i < _totalDemonObjs.Count; i++)
            Destroy(_totalDemonObjs[i]);

        _totalDemonObjs.Clear();
    }

    /// <summary>
    /// Spawns the Chase Demon;
    /// </summary>
    void SpawnChaseDemon()
    {
        int spawnIndex = Random.Range(0, chaseDemonSpawnPos.Length);
        GameObject chaseDemonObj = Instantiate(chaseDemonPrefab, chaseDemonSpawnPos[spawnIndex].position, Quaternion.identity, chaseDemonSpawnPos[spawnIndex]);
        chaseDemonObj.GetComponent<DemonChase>().Player = playerPrefab;

        _totalDemonObjs.Add(chaseDemonObj);

        //Debug.Log("Spawning Chase Demon");
    }
    #endregion

    #region UI
    public void OnMoveUIEnable() => miniGameMoveCanvas.DOFade(1, 0.5f);

    public void OnDashUIEnable() => miniGameDashCanvas.DOFade(1, 0.5f);

    public void OnAttkBlockUIEnable() => miniGameAtkBlockCanvas.DOFade(1, 0.5f);
    #endregion

    #region Cheats
    #endregion

    #endregion

    #region Coroutines

    #endregion

    #region Events
    public void OnMiniGameToggle(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            mmfMiniGameCheats.PlayFeedbacks();
            OnMiniGameEnable?.Invoke();
            miniGamePanel.SetActive(true);
            OnMoveUIEnable();
        }
    }

    /// <summary>
    /// Subbed to event from DemonEnemy and DemonChase;
    /// Spawns the next Demon after certain amount killed;
    /// </summary>
    void OnEnemyDeadEventReceived()
    {
        _currDemonsKilled++;
        redTintAnim.SetTrigger("isTintTriggered");

        if (_totalDemonObjs.Count <= totalDemonsKilled)
        {
            int spawnNumber = Random.Range(1, 3);
            for (int i = 0; i < spawnNumber; i++)
                SpawnChaseDemon();
        }

        if (_currDemonsKilled == 1)
        {
            miniGameMoveCanvas.DOFade(0, miniGameCanvasFadeDelay);
            miniGameDashCanvas.DOFade(0, miniGameCanvasFadeDelay);
            miniGameAtkBlockCanvas.DOFade(0, miniGameCanvasFadeDelay);
        }

        if (_currDemonsKilled >= totalDemonsKilled)
            mmfMiniGameOutro.PlayFeedbacks();
    }

    /// <summary>
    /// Subbed to event from AprilPlayerController;
    /// Restarts the game by playing the MiniGameRestartFeedback;
    /// </summary>
    void OnPlayerDeadEventReceived() => mmfMiniGameRestart.PlayFeedbacks();
    #endregion
}