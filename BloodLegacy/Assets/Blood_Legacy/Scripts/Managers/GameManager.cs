using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using Cinemachine;
using DG.Tweening;
using UnityEngine.Video;

public class GameManager : MonoBehaviour
{
    #region Serialized Variables

    #region Datas
    [Space, Header("Datas")]
    [SerializeField]
    [Tooltip("GameManager Data")]
    private GameMangerData gmData = default;
    #endregion

    #region Ints
    [Space, Header("Ints")]
    [SerializeField]
    [Tooltip("Play Video Index for 1st panel using current Virtual Cam")]
    private int videoPanel1Idex = default;
    #endregion

    #region Floats

    #endregion

    #region UIs
    [Space, Header("UIs")]
    [SerializeField]
    [Tooltip("Fade Background Panel")]
    private Image fadeBG = default;

    [SerializeField]
    [Tooltip("Canvas Group for the Comic Button Panels")]
    private CanvasGroup comicButtonCanvasGroup = default;

    [SerializeField]
    [Tooltip("First Video Player Reference")]
    private VideoPlayer firstVidPlayer = default;

    [SerializeField]
    [Tooltip("First Comic Video Render Texture")]
    private RenderTexture firstAnimaticTex = default;
    #endregion

    #region Virtual Cams
    [Space, Header("Virtual Cams")]
    [SerializeField]
    [Tooltip("The First Virtual Camera Reference")]
    private CinemachineVirtualCamera vCam1 = default;

    [SerializeField]
    [Tooltip("Array of Virtual Cams for the Comic Book")]
    private CinemachineVirtualCamera[] vCams = default;
    #endregion

    #region Others
    [Space, Header("Others")]
    [SerializeField]
    [Tooltip("Fade Background Panel")]
    private PlayableDirector comicTimeline = default;
    #endregion

    #endregion

    #region Private Variables
    [SerializeField] private int _currVCamIndex = default;
    #endregion

    #region Unity Callbacks

    #region Events
    void OnEnable()
    {

    }

    void OnDisable()
    {

    }

    void OnDestroy()
    {

    }
    #endregion

    void Start()
    {
        firstAnimaticTex.Release();
    }

    void Update()
    {

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

        //CheckVideoPanels();
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

        //CheckVideoPanels();
    }

    /// <summary>
    /// Tied to Timeline;
    /// When book opened, Fade in the Panel Buttons;
    /// </summary>
    public void OnComicBookOpened()
    {
        comicButtonCanvasGroup.DOFade(1, 0.5f);
    }

    /// <summary>
    /// Keeps a check if any Video Player is coming up;
    /// </summary>
    void CheckVideoPanels()
    {
        if (_currVCamIndex == videoPanel1Idex && !firstVidPlayer.isPlaying)
        {
            firstVidPlayer.Play();
            Debug.Log("Playing Video 1");
        }
    }
    #endregion

    #region Coroutines

    #endregion

    #region Events

    #endregion
}