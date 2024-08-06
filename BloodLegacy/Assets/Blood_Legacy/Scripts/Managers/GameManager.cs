using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using Cinemachine;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    #region Serialized Variables
    [Space, Header("Datas")]
    [SerializeField]
    [Tooltip("GameManager Data")]
    private GameMangerData gmData = default;

    [Space, Header("UI")]
    [SerializeField]
    [Tooltip("Fade Background Panel")]
    private Image fadeBG = default;

    [SerializeField]
    [Tooltip("Canvas Group for the Comic Button Panels")]
    private CanvasGroup comicButtonCanvasGroup = default;

    [Space, Header("Virtual Cams")]
    [SerializeField]
    [Tooltip("The First Virtual Camera Reference")]
    private CinemachineVirtualCamera vCam1 = default;

    [SerializeField]
    [Tooltip("Array of Virtual Cams for the Comic Book")]
    private CinemachineVirtualCamera[] vCams = default;

    [Space, Header("Others")]
    [SerializeField]
    [Tooltip("Fade Background Panel")]
    private PlayableDirector comicTimeline = default;
    #endregion

    #region Private Variables
    private int _currVCamIndex = default;
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

    }

    void Update()
    {

    }
    #endregion

    #region My Functions
    public void OnClick_ComicNext()
    {
        if (_currVCamIndex < vCams.Length)
        {
            vCams[_currVCamIndex].gameObject.SetActive(false);
            _currVCamIndex++;
            vCams[_currVCamIndex].gameObject.SetActive(true);
        }
    }

    public void OnClick_ComicPrev()
    {
        if (_currVCamIndex > vCams.Length)
        {
            vCams[_currVCamIndex].gameObject.SetActive(false);
            _currVCamIndex--;
            vCams[_currVCamIndex].gameObject.SetActive(true);
        }
    }

    public void OnComicBookOpened()
    {
        comicButtonCanvasGroup.DOFade(1, 0.5f);
    }
    #endregion

    #region Coroutines

    #endregion

    #region Events

    #endregion
}