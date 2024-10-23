using UnityEngine;
using DG.Tweening;

public class DemonClimbDangle : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField] private Vector3 dangleEndVector = default;
    [SerializeField] private float dangleSpeed = default;
    #endregion

    #region Unity Callbacks
    void Start() => gameObject.transform.DORotate(dangleEndVector, dangleSpeed).SetLoops(-1, LoopType.Yoyo);
    #endregion
}