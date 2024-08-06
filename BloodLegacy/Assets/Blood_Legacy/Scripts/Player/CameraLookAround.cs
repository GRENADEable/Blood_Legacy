using UnityEngine;

public class CameraLookAround : MonoBehaviour
{
    #region Serialized Variables
    [Space, Header("Data")]
    [SerializeField]
    [Tooltip("GameManager Scriptable Object")]
    private GameMangerData gmData = default;

    [SerializeField]
    [Tooltip("Can it depend on the GameManager Scriptable Object? Set this to true if you want to use the GmData Scriptable Object")]
    private bool isUsingScriptableObject = default;

    [Space, Header("Mouse Settings")]
    [SerializeField]
    [Tooltip("Minimum clamp on X Axis")]
    private float minXClamp = -90f;

    [SerializeField]
    [Tooltip("Maximum clamp on X Axis")]
    private float maxXClamp = 90f;

    [SerializeField]
    [Tooltip("Mouse sensitivity")]
    private float mouseSens = 300f;

    [SerializeField]
    [Tooltip("Transform Component of the root object")]
    private Transform playerRoot = default;
    #endregion

    #region Private Variables
    private float _xRotate = default;
    #endregion

    #region Unity Callbacks
    void Update()
    {
        if (isUsingScriptableObject)
        {
            if (gmData.currState == GameMangerData.GameState.Game)
            {
                float mouseX = Input.GetAxis("Mouse X") * mouseSens * Time.deltaTime;
                float mouseY = Input.GetAxis("Mouse Y") * mouseSens * Time.deltaTime;

                _xRotate -= mouseY;
                _xRotate = Mathf.Clamp(_xRotate, minXClamp, maxXClamp);

                transform.localRotation = Quaternion.Euler(_xRotate, 0f, 0f);

                playerRoot.Rotate(Vector3.up * mouseX);
            }
        }
        else
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSens * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSens * Time.deltaTime;

            _xRotate -= mouseY;
            _xRotate = Mathf.Clamp(_xRotate, minXClamp, maxXClamp);

            transform.localRotation = Quaternion.Euler(_xRotate, 0f, 0f);

            playerRoot.Rotate(Vector3.up * mouseX);
        }
    }
    #endregion

    #region Functions

    #endregion
}