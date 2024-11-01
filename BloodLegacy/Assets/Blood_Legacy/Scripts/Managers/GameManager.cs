using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using UnityEngine.UI;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    #region Serialized Variables

    #region Datas
    [Space, Header("Datas")]
    [SerializeField]
    [Tooltip("GameManager Data")]
    private GameMangerData gmData = default;

    [SerializeField]
    [Tooltip("Do you wanna use the timescale modifier?")]
    private bool isUsingTimeModifier = default;
    #endregion

    #region Ints and Floats
    [Space, Header("Ints and Floats")]
    [SerializeField]
    [Range(1f, 7f)]
    [Tooltip("Timescale for debuggging")]
    private int timescaleModifier = 1;

    [SerializeField]
    [Tooltip("April Hands BG Alpha ramp down value")]
    private float aprilhandsFadeOut = default;

    [SerializeField]
    [Tooltip("How much time do you want the alpha to ramp down?")]
    private float aprilhandsFadeOutDelay = default;
    #endregion

    #region Transforms
    [Space, Header("Transforms")]
    [SerializeField]
    [Tooltip("The End Position of the Book to show ending")]
    private Transform bookEndPos = default;
    #endregion

    #region GameObjects
    [Space, Header("GameObjects")]
    [SerializeField]
    [Tooltip("Comic Book GameObject")]
    private GameObject bookObj = default;

    [SerializeField]
    [Tooltip("Comic Book Page GameObject")]
    private GameObject[] comicPagesObjs = default;
    #endregion

    #region UIs
    [Space, Header("UIs")]
    [SerializeField]
    [Tooltip("All the render texture GameObjects")]
    private RenderTexture[] rendTexs = default;

    [SerializeField]
    [Tooltip("Red Background for the April Hands Panel")]
    private Image aprilHandsRedBG = default;
    #endregion

    #region Virtual Cams
    [SerializeField]
    [Tooltip("Array of Virtual Cams for the Comic Book")]
    private CinemachineVirtualCamera[] vCams = default;
    #endregion

    #endregion

    #region Private Variables
    [SerializeField] private int _currVCamIndex = default;
    #endregion

    #region Unity Callbacks

    #region Events
    void OnEnable()
    {
        MiniGameManager.OnMiniGameEnable += OnMiniGameToggleEventReceived;
    }

    void OnDisable()
    {
        MiniGameManager.OnMiniGameEnable -= OnMiniGameToggleEventReceived;
    }

    void OnDestroy()
    {
        MiniGameManager.OnMiniGameEnable -= OnMiniGameToggleEventReceived;
    }
    #endregion

    void Start()
    {
        gmData.ChangeGameState("Book");
        ComicBookTexRelease();
    }

    void Update()
    {
        if (isUsingTimeModifier)
            Time.timeScale = timescaleModifier;
    }
    #endregion

    #region My Functions
    /// <summary>
    /// Tied to Next_Comic_Button;
    /// Pans the camera to the next panel;
    /// Also keeps check if there is any Video Panel coming up;
    /// </summary>
    public void OnClick_ComicNext()
    {
        vCams[_currVCamIndex].gameObject.SetActive(false);
        _currVCamIndex++;

        if (_currVCamIndex >= vCams.Length)
            _currVCamIndex = 0;

        vCams[_currVCamIndex].gameObject.SetActive(true);
    }

    /// <summary>
    /// Tied to Prev_Comic_Button;
    /// Pans the camera to the previous panel;
    /// Also keeps check if there is any Video Panel coming up;
    /// </summary>
    public void OnClick_ComicPrev()
    {
        vCams[_currVCamIndex].gameObject.SetActive(false);

        if (_currVCamIndex <= 0)
            _currVCamIndex = vCams.Length;

        _currVCamIndex--;
        vCams[_currVCamIndex].gameObject.SetActive(true);
    }

    /// <summary>
    /// Tied to Restart_Button;
    /// Restarts the Game;
    /// </summary>
    public void OnRestartGame() => gmData.ChangeLevel(Application.loadedLevel);

    /// <summary>
    /// Tied to Exit_Button;
    /// Exits the Game;
    /// </summary>
    public void OnExitGame() => gmData.QuitGame();

    /// <summary>
    /// Tied to MMF_April_Hands;
    /// Pulses the Red Background when triggered;
    /// </summary>
    public void OnRedBGPulse()
    {
        aprilHandsRedBG.DOFade(aprilhandsFadeOut, aprilhandsFadeOutDelay).SetLoops(-1, LoopType.Yoyo);
    }

    /// <summary>
    /// Tied to MMF_MiniGame_April_Hit Feedback;
    /// Freezes the game for a split second;
    /// </summary>
    /// <param name="isFreezing"> If true, freeze, else don't freeze; </param>
    public void OnFreezeTimeToggle(bool isFreezing)
    {
        if (isFreezing)
            Time.timeScale = 0.001f;
        else
            Time.timeScale = 1f;

    }

    public void OnBookEnd()
    {
        bookObj.transform.SetPositionAndRotation(bookEndPos.position, bookEndPos.rotation);

        for (int i = 0; i < comicPagesObjs.Length; i++)
            comicPagesObjs[i].SetActive(false);

    }

    /// <summary>
    /// Releases and letft over Camera output render texture data;
    /// </summary>
    void ComicBookTexRelease()
    {
        for (int i = 0; i < rendTexs.Length; i++)
            rendTexs[i].Release();
    }
    #endregion

    #region Events

    #region Input Systems
    public void OnCamMoveNext(InputAction.CallbackContext context)
    {
        if (context.started && gmData.currState == GameMangerData.GameState.Book)
            OnClick_ComicNext();
    }

    public void OnCamMovePrev(InputAction.CallbackContext context)
    {
        if (context.started && gmData.currState == GameMangerData.GameState.Book)
            OnClick_ComicPrev();
    }
    #endregion

    /// <summary>
    /// Subbed to event from MiniGameManager;
    /// Enables the VCam for the Mini Game only;
    /// </summary>
    void OnMiniGameToggleEventReceived()
    {
        for (int i = 0; i < vCams.Length; i++)
            vCams[i].gameObject.SetActive(false);

        _currVCamIndex = 6;
        vCams[_currVCamIndex].gameObject.SetActive(true);
    }

    #endregion
}