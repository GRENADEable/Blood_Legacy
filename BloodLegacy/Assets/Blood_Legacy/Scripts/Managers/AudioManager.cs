using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    #region Serialized Variables

    #region Audios
    [Space, Header("Audios")]
    [SerializeField]
    [Tooltip("Audio Source for Player SFX")]
    private AudioSource sfxAud = default;

    [SerializeField]
    [Tooltip("SFX Audio Clips")]
    private AudioClip[] sfxClips = default;
    #endregion

    #endregion

    #region Private Variables

    #endregion

    #region Unity Callbacks

    #region Events
    void OnEnable()
    {
        DemonDefault.OnEnemyDead += OnEnemyDeadEventReceived;
        DemonChase.OnEnemyDead += OnEnemyDeadEventReceived;

        AprilPlayerController.OnPlayerDead += OnPlayerDeadEventReceived;
        AprilPlayerController.OnSwordSwipe += OnSwordSwipeEventReceived;
        AprilPlayerController.OnPlayerBlock += OnPlayerBlockEventReceived;
    }

    void OnDisable()
    {
        DemonDefault.OnEnemyDead -= OnEnemyDeadEventReceived;
        DemonChase.OnEnemyDead -= OnEnemyDeadEventReceived;

        AprilPlayerController.OnPlayerDead -= OnPlayerDeadEventReceived;
        AprilPlayerController.OnSwordSwipe -= OnSwordSwipeEventReceived;
        AprilPlayerController.OnPlayerBlock -= OnPlayerBlockEventReceived;
    }

    void OnDestroy()
    {
        DemonDefault.OnEnemyDead -= OnEnemyDeadEventReceived;
        DemonChase.OnEnemyDead -= OnEnemyDeadEventReceived;

        AprilPlayerController.OnPlayerDead -= OnPlayerDeadEventReceived;
        AprilPlayerController.OnSwordSwipe -= OnSwordSwipeEventReceived;
        AprilPlayerController.OnPlayerBlock -= OnPlayerBlockEventReceived;
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

    /// <summary>
    /// Subbed to Event from Enemy Script;
    /// Kills the Player and Plays the Death SFX;
    /// </summary>
    void OnPlayerDeadEventReceived() => sfxAud.PlayOneShot(sfxClips[2]);

    /// <summary>
    /// Subbed to Event from AprilPlayerController Script;
    /// Plays the sword swipe SFX;
    /// </summary>
    void OnSwordSwipeEventReceived() => sfxAud.PlayOneShot(sfxClips[3]);

    /// <summary>
    /// Subbed teh Event from DemonEnemy Script;
    /// Plays the enemy death SFX;
    /// </summary>
    void OnEnemyDeadEventReceived()
    {
        sfxAud.PlayOneShot(sfxClips[0]);
        sfxAud.PlayOneShot(sfxClips[1]);
    }

    void OnPlayerBlockEventReceived() => sfxAud.PlayOneShot(sfxClips[4]);
    #endregion
}