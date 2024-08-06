using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.Playables;

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

    [Space, Header("Others")]
    [SerializeField]
    [Tooltip("Fade Background Panel")]
    private PlayableDirector comicTimeline = default;
    #endregion

    #region Private Variables

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

    #endregion

    #region Coroutines

    #endregion

    #region Events

    #endregion
}