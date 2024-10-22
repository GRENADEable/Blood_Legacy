using UnityEngine;
using DG.Tweening;

public class AprilWooshFX : MonoBehaviour
{
    #region Serialised Variables
    [SerializeField] private CanvasGroup swooshCanvasGroup = default;
    [SerializeField] private float swooshCanvasAppear = default;
    [SerializeField] private float swooshCanvasDisappear = default;
    //[SerializeField] private float swooshDestoryTime = default;
    [SerializeField] private float _totalSwooshDestroyTime = default;
    #endregion

    #region Private Variables

    #endregion

    #region Unity Callbacks
    void OnEnable()
    {
        _totalSwooshDestroyTime = swooshCanvasAppear + swooshCanvasDisappear;

        swooshCanvasGroup.DOFade(1, swooshCanvasAppear).OnComplete(() => swooshCanvasGroup.DOFade(0, swooshCanvasDisappear));
        Destroy(gameObject, _totalSwooshDestroyTime);
        //DOTween.Sequence().SetDelay(1f).Append(swooshCanvasGroup.DOFade(1, swooshCanvasAppear).OnComplete(() => swooshCanvasGroup.DOFade(0, swooshCanvasAppear)));
    }
    #endregion
}