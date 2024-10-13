using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

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
    [Range(1f, 7f)]
    [Tooltip("Timescale for debuggging")]
    private int timescaleModifier = 1;
    #endregion

    #region Floats

    #endregion

    #region Transforms

    #endregion

    #region GameObjects

    #endregion

    #region UIs
    [Space, Header("UIs")]
    [SerializeField]
    [Tooltip("All the render texture GameObjects")]
    private RenderTexture[] rendTexs = default;
    #endregion

    #region Virtual Cams
    [SerializeField]
    [Tooltip("Array of Virtual Cams for the Comic Book")]
    private CinemachineVirtualCamera[] vCams = default;
    #endregion

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
        gmData.ChangeGameState("Book");
        ComicBookTexRelease();
    }

    void Update()
    {
        Time.timeScale = timescaleModifier;
    }
    #endregion

    #region My Functions
    /// <summary>
    /// Releases and letft over Camera output render texture data;
    /// </summary>
    void ComicBookTexRelease()
    {
        for (int i = 0; i < rendTexs.Length; i++)
            rendTexs[i].Release();
    }

    #region Cheats

    #endregion

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
    #endregion
}