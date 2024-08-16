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

    #region Transforms
    [Space, Header("Transforms")]
    [SerializeField]
    [Tooltip("")]
    private Transform comic1Bullet1EndPos = default;
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


    #region Comic Layers
    [Space, Header("Comic Layers")]
    [SerializeField]
    [Tooltip("Comic Panel 1 Speech Bubble 1 Image")]
    private SpriteRenderer comic1Speech1 = default;

    [SerializeField]
    [Tooltip("Comic Panel 1 Bullet Image")]
    private SpriteRenderer comic1Bullet = default;

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
        OnComicVidTexRelease();
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
    }

    /// <summary>
    /// Releases the texture of the video on the comic;
    /// </summary>
    public void OnComicVidTexRelease() => firstAnimaticTex.Release();

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

    #endregion
}