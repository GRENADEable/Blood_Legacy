using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComicPanelController : MonoBehaviour
{
    #region Serialized Variables
    [SerializeField]
    [Tooltip("Comic Panel UI Images")]
    private GameObject[] comicPanels = default;

    [SerializeField]
    [Tooltip("After how many clicks do we close the Overlay?")]
    private int totalComicPanelClicks = default;

    [SerializeField]
    [Tooltip("Close Comic Panel Button")]
    private Button closeComicPanelButton = default;

    [SerializeField]
    [Tooltip("Next Comic Panel Reveal Button")]
    private Button nextComicRevealButton = default;
    #endregion

    #region Private Variables
    [SerializeField] private int _currPanelIndex = default;
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
    #endregion

    #region My Functions
    public void OnComicPanelNext()
    {
        comicPanels[_currPanelIndex].SetActive(true);
        _currPanelIndex++;

        if (_currPanelIndex > totalComicPanelClicks)
        {
            closeComicPanelButton.gameObject.SetActive(true);
            nextComicRevealButton.interactable = false;
        }

    }
    #endregion

    #region Coroutines

    #endregion

    #region Events

    #endregion
}