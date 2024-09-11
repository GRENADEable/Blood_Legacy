using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Playables;
using Cinemachine;
using DG.Tweening;
using UnityEngine.Video;
using MoreMountains.Feedbacks;

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

    #endregion

    #region Floats
    [Space, Header("Floats")]
    [SerializeField]
    [Tooltip("Blood splatter intial Scale")]
    private float bloodEndScale = default;
    #endregion

    #region Transforms
    [Space, Header("Transforms")]
    [SerializeField]
    [Tooltip("")]
    private Transform comic1Bullet1EndPos = default;
    #endregion

    #region GameObjects
    //[Space, Header("GameObjects")]
    //[SerializeField]
    //[Tooltip("Second Page Comic Book")]
    //private GameObject comicPage2 = default;
    #endregion

    #region UIs
    [Space, Header("UIs")]
    [SerializeField]
    [Tooltip("Canvas Group for the Comic Button Panels")]
    private CanvasGroup comicButtonCanvasGroup = default;

    [SerializeField]
    [Tooltip("First Video Player Reference")]
    private VideoPlayer firstVidPlayer = default;

    [SerializeField]
    [Tooltip("First Comic Video Render Texture")]
    private RenderTexture firstAnimaticTex = default;

    #region Comic Layer 1
    [Space, Header("Comic Layer 1")]
    [SerializeField]
    [Tooltip("Comic Panel 1 Speech Bubble 1 Image")]
    private SpriteRenderer comic1Speech1 = default;

    [SerializeField]
    [Tooltip("Comic Panel 1 Bullet Image")]
    private SpriteRenderer comic1Bullet = default;

    [SerializeField]
    [Tooltip("Comic Panel 1 Blood Splatter Image")]
    private SpriteRenderer comic1BloodSplatter = default;

    [SerializeField]
    [Tooltip("Comic Panel 1 Speech Bubble 2 Image")]
    private SpriteRenderer comic1Speech2 = default;

    [SerializeField]
    [Tooltip("Comic Panel 1 Speech Bubble 3 Image")]
    private SpriteRenderer comic1Speech3 = default;
    #endregion

    #endregion

    #region Virtual Cams
    [Space, Header("Virtual Cams")]
    [SerializeField]
    [Tooltip("The Tenth Virtual Camera Reference")]
    private CinemachineVirtualCamera vCam10 = default;

    [SerializeField]
    [Tooltip("Array of Virtual Cams for the Comic Book")]
    private CinemachineVirtualCamera[] vCams = default;
    #endregion

    #region Others
    [Space, Header("Others")]
    [SerializeField]
    [Tooltip("Fade Background Panel")]
    private PlayableDirector comicTimeline = default;

    [SerializeField]
    [Tooltip("MMF Player Script after the first Animatic Vid Ends")]
    private MMF_Player mMFFirstVid = default;
    #endregion

    #endregion

    #region Private Variables
    private int _currVCamIndex = default;
    [SerializeField] private MiniGameManager _miniGameManager = default;
    #endregion

    #region Unity Callbacks

    #region Events
    void OnEnable()
    {
        firstVidPlayer.loopPointReached += OnVideoEnded;
    }

    void OnDisable()
    {
        firstVidPlayer.loopPointReached -= OnVideoEnded;
    }

    void OnDestroy()
    {
        firstVidPlayer.loopPointReached -= OnVideoEnded;
    }
    #endregion

    void Start()
    {
        gmData.ChangeGameState("Book");
        OnComicVidTexRelease();

        _miniGameManager = GetComponent<MiniGameManager>();
    }

    void Update()
    {

    }
    #endregion

    #region My Functions

    #region Comic Book Triggers
    /// <summary>
    /// Tield to T_Text_Bubble_1;
    /// Makes the Speech Bubble Appear;
    /// </summary>
    /// <param name="appearTime"> How long will it take for the Image to FadeIn; </param>
    public void OnComic1SpeechBubble1Trigger(float appearTime)
    {
        comic1Speech1.DOFade(1, appearTime);
    }

    /// <summary>
    /// Tied to T_Bullet_1;
    /// Fades in the sniper Bullet and Moves it in also to a specific Position;
    /// </summary>
    /// <param name="moveAppearTime">How long will it take for the Image to FadeIn; </param>
    public void OnComic1BulletTrigger(float moveAppearTime)
    {
        comic1Bullet.transform.DOMove(comic1Bullet1EndPos.position, moveAppearTime);
        comic1Bullet.DOFade(1, moveAppearTime);
    }

    /// <summary>
    /// Tied to T_Aud_Groan;
    /// Fades in the Blood Spaltter and Scales it up
    /// </summary>
    /// <param name="scaleAppearTime">How long will it take for the Image to FadeIn and Scale; </param>
    public void OnComic1BloodSplatter(float scaleAppearTime)
    {
        comic1BloodSplatter.DOFade(1, scaleAppearTime);
        comic1BloodSplatter.transform.DOScale(bloodEndScale, scaleAppearTime);

    }

    /// <summary>
    /// Tied to T_Text_2;
    /// Fades in the Speech Bubble;
    /// </summary>
    /// <param name="moveAppearTime"> How long will it take for the Image to FadeIn; </param>
    public void OnComic1SpeechBubble2Trigger(float moveAppearTime)
    {
        comic1Speech2.DOFade(1, moveAppearTime);
    }

    /// <summary>
    /// Tied to T_Text_3;
    /// Fades in the Speech Bubble;
    /// </summary>
    /// <param name="moveAppearTime"> How long will it take for the Image to FadeIn; </param>
    public void OnComic1SpeechBubble3Trigger(float moveAppearTime)
    {
        comic1Speech3.DOFade(1, moveAppearTime);
    }
    #endregion

    #endregion

    #region Coroutines

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
    /// Tied to Timeline;
    /// When book opened, Fade in the Panel Buttons;
    /// </summary>
    public void OnComicBookOpened()
    {
        comicButtonCanvasGroup.DOFade(1, 0.5f);
        //comicPage2.SetActive(true);
    }

    /// <summary>
    /// Releases the texture of the video on the comic;
    /// </summary>
    public void OnComicVidTexRelease() => firstAnimaticTex.Release();

    #region Mini Games
    /// <summary>
    /// Tied to Video Player;
    /// Switches to Mini Game when the Video ends;
    /// </summary>
    /// <param name="vid"> Video Player of the First Animatic; </param>
    void OnVideoEnded(VideoPlayer vid)
    {
        mMFFirstVid.PlayFeedbacks();
        vCams[_currVCamIndex].gameObject.SetActive(false);
        vCam10.gameObject.SetActive(true);
        gmData.ChangeGameState("MiniGame");
    }
    #endregion

    #endregion
}