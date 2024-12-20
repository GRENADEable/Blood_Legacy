using MoreMountains.Feedbacks;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;
using TMPro;

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

    [SerializeField]
    [Tooltip("Mini Game HUD Panel")]
    private GameObject miniGameHUDPanel = default;

    [SerializeField]
    [Tooltip("Skip MiniGame Panel")]
    private GameObject miniGameSkipPanel = default;

    [SerializeField]
    [Tooltip("HP GameObjects")]
    private GameObject[] hpObjs = default;
    #endregion

    #region Ints and Floats
    [Space, Header("Ints and Floats")]
    [SerializeField]
    [Tooltip("After how many demons to kill to goto the next panel?")]
    private int totalDemonsKilled = default;

    [SerializeField]
    [Tooltip("After how many deaths do you want to show the skip paenl?")]
    private int totaldeathToSkip = 3;

    [SerializeField]
    [Tooltip("How long do you want to Fade In the Red Tint?")]
    private float redTintFadeDelay = default;

    [SerializeField]
    [Tooltip("How long do you want to Fade Out the Mini Game Tutorial World Canvas?")]
    private float miniGameCanvasFadeDelay = default;

    [SerializeField]
    [Tooltip("How long is the mini game duration?")]
    private float miniGameTotalTime = default;
    #endregion

    //#region Bools
    //[Space, Header("Bools")]

    //#endregion

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

    [SerializeField]
    [Tooltip("Game Timer Text")]
    private TextMeshProUGUI timerText = default;

    [SerializeField]
    [Tooltip("Demon Kill Counter Text")]
    private TextMeshProUGUI demonKillText = default;
    #endregion

    #region Feels
    [Space, Header("Feels")]
    [SerializeField]
    [Tooltip("MMF_MiniGame_Long_Restart Component to Restart the MiniGame")]
    private MMF_Player mmfMiniGameLongRestart = default;

    [SerializeField]
    [Tooltip("MMF_MiniGame_Short_Restart Component to Restart the MiniGame")]
    private MMF_Player mmfMiniGameShortRestart = default;

    [SerializeField]
    [Tooltip("MMF_MiniGame_Restart Component to End the MiniGame")]
    private MMF_Player mmfMiniGameOutro = default;

    [SerializeField]
    [Tooltip("MMF_MiniGame_Enemy_Hit Component to react with Enemy Hit")]
    private MMF_Player mmfMiniGameEnemyHit = default;

    [SerializeField]
    [Tooltip("MMF_MiniGame_Enemy_Hit Component to react with Enemy Hit")]
    private MMF_Player mmfMiniGameAprilHit = default;

    [SerializeField]
    [Tooltip("MMF_MiniGame_Skip Component to for Mini Game Skipping")]
    private MMF_Player mmfMiniGameSkip = default;

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
    [Tooltip("For testing purposes")]
    private bool isUsingFeedbacks = default;

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
    private float _currTime = default;
    private int _currRespawnCount = default;
    private List<GameObject> _totalDemonObjs = new List<GameObject>();
    private Vector3 _playerStartPos = default;
    private bool _isMiniGameStarted = default;
    private int _currHP = 0;
    #endregion

    #region Unity Callbacks

    #region Events
    void OnEnable()
    {
        DemonDefault.OnEnemyDead += OnEnemyDeadEventReceived;

        DemonChase.OnEnemyDead += OnEnemyDeadEventReceived;
        DemonChase.OnPlayerDamage += OnPlayerDamageEventReceived;

        AprilPlayerController.OnPlayerDead += OnPlayerDeadEventReceived;
        AprilPlayerController.OnPlayerHit += OnPlayerHitEventReceived;
    }

    void OnDisable()
    {
        DemonDefault.OnEnemyDead -= OnEnemyDeadEventReceived;

        DemonChase.OnEnemyDead -= OnEnemyDeadEventReceived;
        DemonChase.OnPlayerDamage -= OnPlayerDamageEventReceived;

        AprilPlayerController.OnPlayerDead -= OnPlayerDeadEventReceived;
        AprilPlayerController.OnPlayerHit -= OnPlayerHitEventReceived;
    }

    void OnDestroy()
    {
        DemonDefault.OnEnemyDead -= OnEnemyDeadEventReceived;

        DemonChase.OnEnemyDead -= OnEnemyDeadEventReceived;
        DemonChase.OnPlayerDamage -= OnPlayerDamageEventReceived;

        AprilPlayerController.OnPlayerDead -= OnPlayerDeadEventReceived;
        AprilPlayerController.OnPlayerHit -= OnPlayerHitEventReceived;
    }
    #endregion

    void Start()
    {
        _playerStartPos = playerPrefab.transform.position;
        _currTime = miniGameTotalTime;
        UpdateTimerUI(_currTime);
        UpdateKillUI(_currDemonsKilled);
        _currHP = hpObjs.Length;
    }

    void Update()
    {
        if (_isMiniGameStarted)
            CountdownTimer();
    }
    #endregion

    #region My Functions

    #region Gameplay
    /// <summary>
    /// Tied to MMF_MiniGame_Restart;
    /// Resets the Player Position;
    /// </summary>
    public void OnPlayerReset()
    {
        miniGameHUDPanel.SetActive(false);
        playerPrefab.SetActive(false);
        playerPrefab.transform.position = _playerStartPos;
    }

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
        _currRespawnCount++;
        _currTime = miniGameTotalTime;
        redTintAnim.Play("Empty");
        UpdateKillUI(_currDemonsKilled);
        UpdateTimerUI(_currTime);
        _isMiniGameStarted = false;
    }

    /// <summary>
    /// Enables the Mini Game Timer;
    /// </summary>
    public void OnGameTimerStart() => _isMiniGameStarted = true;

    /// <summary>
    /// Tied to MMF_MiniGame_Long_Restart;
    /// Starts the game after the Reset;
    /// </summary>
    public void OnGameStart()
    {
        if (_currRespawnCount >= totaldeathToSkip)
            miniGameSkipPanel.SetActive(true);
        else
        {
            playerPrefab.SetActive(true);
            SpawnChaseDemon();
            OnDemonChase?.Invoke();
            _isMiniGameStarted = true;
            miniGameHUDPanel.SetActive(true);
        }
    }

    /// <summary>
    /// Tied to MMF_MiniGame_Quick_Restart;
    /// Starts the game quickly after the Reset;
    /// </summary>
    public void OnGameStartQuick()
    {
        playerPrefab.SetActive(true);
        miniGameSkipPanel.SetActive(false);
        miniGameHUDPanel.SetActive(true);
        SpawnChaseDemon();
        OnDemonChase?.Invoke();
        _isMiniGameStarted = true;
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
    /// <summary>
    /// Enables the Move UI Control Layout;
    /// </summary>
    public void OnMoveUIEnable() => miniGameMoveCanvas.DOFade(1, 0.5f);

    /// <summary>
    /// Tied to T_Dash_UI;
    /// Enables the Dash UI World Canvas;
    /// </summary>
    public void OnDashUIEnable() => miniGameDashCanvas.DOFade(1, 0.5f);

    /// <summary>
    /// Tied to T_Atk_Block_UI;
    /// Enables the Attack UI World Canvas;
    /// </summary>
    public void OnAttkBlockUIEnable() => miniGameAtkBlockCanvas.DOFade(1, 0.5f);

    /// <summary>
    /// Tied to MMF_MiniGame_Start Feedback Event;
    /// Fades out the UI canvas after the cutscene ends;
    /// </summary>
    public void OnMiniGameUIFade()
    {
        miniGameMoveCanvas.DOFade(0, miniGameCanvasFadeDelay);
        miniGameDashCanvas.DOFade(0, miniGameCanvasFadeDelay);
        miniGameAtkBlockCanvas.DOFade(0, miniGameCanvasFadeDelay);
    }

    /// <summary>
    /// Tied to Mini_Game_Yes_Button;
    /// Skips the Mini Game;
    /// </summary>
    public void OnClick_SkipMiniGame() => mmfMiniGameSkip.PlayFeedbacks();

    /// <summary>
    /// Tied to Mini_Game_No_Button;
    /// Restarts the Mini Game;
    /// </summary>
    public void OnClick_RetryMiniGame() => mmfMiniGameShortRestart.PlayFeedbacks();

    /// <summary>
    /// Updates the timer Text UI;
    /// </summary>
    /// <param name="timer"> Timer float; </param>
    void UpdateTimerUI(float timer) => timerText.text = $"Time: {_currTime:f0}";

    /// <summary>
    /// Updates the kill counter UI;
    /// </summary>
    /// <param name="kills"> Kill int; </param>
    void UpdateKillUI(int kills) => demonKillText.text = $"Kills: {kills}";

    /// <summary>
    /// Countdown timer of the MiniGame;
    /// </summary>
    void CountdownTimer()
    {
        _currTime -= Time.deltaTime;
        UpdateTimerUI(_currTime);

        if (_currTime <= 0)
        {
            _currTime = miniGameTotalTime;
            mmfMiniGameLongRestart.PlayFeedbacks();
        }
    }
    #endregion

    #endregion

    #region Events
    /// <summary>
    /// Cheats for enabling MiniGame;
    /// </summary>
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
        UpdateKillUI(_currDemonsKilled);
        redTintAnim.SetTrigger("isTintTriggered");

        mmfMiniGameEnemyHit.PlayFeedbacks();

        if (_totalDemonObjs.Count <= totalDemonsKilled)
        {
            int spawnNumber = Random.Range(1, 3);
            for (int i = 0; i < spawnNumber; i++)
                SpawnChaseDemon();
        }

        //if (_currDemonsKilled == 1)
        //{
        //    miniGameMoveCanvas.DOFade(0, miniGameCanvasFadeDelay);
        //    miniGameDashCanvas.DOFade(0, miniGameCanvasFadeDelay);
        //    miniGameAtkBlockCanvas.DOFade(0, miniGameCanvasFadeDelay);
        //}

        if (_currDemonsKilled >= totalDemonsKilled)
        {
            if (isUsingFeedbacks)
                mmfMiniGameOutro.PlayFeedbacks();

            _isMiniGameStarted = false;
        }
    }

    /// <summary>
    /// Subbed to event from AprilPlayerController;
    /// Restarts the game by playing the MiniGameRestartFeedback;
    /// </summary>
    void OnPlayerDeadEventReceived()
    {
        if (isUsingFeedbacks)
            mmfMiniGameLongRestart.PlayFeedbacks();

        _currHP = hpObjs.Length;

        for (int i = 0; i < hpObjs.Length; i++)
            hpObjs[i].SetActive(true);

    }

    /// <summary>
    /// Subbed to DemonChase Event;
    /// Plays Feedback for April Hit;
    /// </summary>
    void OnPlayerDamageEventReceived() => mmfMiniGameAprilHit.PlayFeedbacks();

    /// <summary>
    /// Subbed to event from AprilPlayerController;
    /// Updates the HP UI;
    /// </summary>
    void OnPlayerHitEventReceived()
    {
        _currHP--;
        hpObjs[_currHP].SetActive(false);
    }
    #endregion
}