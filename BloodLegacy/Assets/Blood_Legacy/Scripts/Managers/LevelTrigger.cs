using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class LevelTrigger : MonoBehaviour
{
    public enum TriggerType { General, Gameplay, Audio, Camera, None };

    public static bool showBoundries = false;

    [Tooltip("Shows differnt types of icons for the trigger")]
    public TriggerType type = TriggerType.General;

    [Tooltip("Call event after x amount of seconds")]
    public float triggerStayTime = 0f;

    [Tooltip("Call event with specific tag")]
    public string activatorTag = "Player";

    [Space, Header("Trigger Counter Clamp")]

    [Tooltip("How many times can it enter the trigger")]
    public int triggerCounterEnter = 0;

    [Tooltip("How many times can it stay in the trigger")]
    public int triggerCounterStay = 0;

    [Tooltip("How many times can it exit the trigger")]
    public int triggerCounterExit = 0;

    [Space, Header("Trigger Counter")]

    [Tooltip("When to trigger after x amount of enteries")]
    public int triggerAfterEnter = 0;

    [Tooltip("When to trigger after x amount of stays")]
    public int triggerAfterStay = 0;

    [Tooltip("When to trigger after x amount of exits")]
    public int triggerAfterExit = 0;

    [Space, Header("Trigger Delay")]
    [Tooltip("Call event after x amount of seconds")]
    public float triggerDelay = 0f;

    [Header("Event Trigger Enter / Stay / Exit")]
    public UnityEvent EventTriggerEnter;
    public UnityEvent EventTriggerStay;
    public UnityEvent EventTriggerExit;

    private float _timeStayed = 0f;
    private bool _stayActive = false;
    private int _triggerCountedEnter = 0;
    private int _triggerCountedStay = 0;
    private int _triggerCountedExit = 0;

    #region Unity Callbacks
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(activatorTag))
        {
            int triggerRange = _triggerCountedEnter - triggerAfterEnter;

            if (triggerCounterEnter == 0 || (triggerRange >= 0 && triggerRange < triggerCounterEnter))
            {
                _timeStayed = 0f;
                _stayActive = true;

                Debug.Log($"Trigger Entered {gameObject.name}");

                if (triggerDelay > 0)
                    StartCoroutine("EnterDelayed");
                else
                    EventTriggerEnter.Invoke();
            }

            _triggerCountedEnter++;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(activatorTag))
        {
            int triggerRange = _triggerCountedStay - triggerAfterStay;
            _timeStayed += Time.deltaTime;

            if (_timeStayed > triggerStayTime && _stayActive)
            {
                if (triggerCounterStay == 0 || (triggerRange >= 0 && triggerRange < triggerCounterStay))
                {
                    Debug.Log("Trigger Stay");

                    if (triggerDelay > 0)
                        StartCoroutine("StayDelayed");
                    else
                        EventTriggerStay.Invoke();
                }

                _stayActive = false;

                _triggerCountedStay++;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(activatorTag))
        {
            int triggerRange = _triggerCountedExit - triggerAfterExit;

            if (triggerCounterExit == 0 || (triggerRange >= 0 && triggerRange < triggerCounterExit))
            {
                _timeStayed = 0f;

                Debug.Log("Trigger Exit");

                if (triggerDelay > 0)
                    StartCoroutine("ExitDelayed");
                else
                    EventTriggerExit.Invoke();
            }

            _triggerCountedExit++;
        }
    }

    void OnDrawGizmos()
    {
        if (Vector3.Distance(transform.position, Camera.current.transform.position) > 0.1f)
        {
            switch (type)
            {
                case TriggerType.General:
                    Gizmos.DrawIcon(transform.position, "GeneralIcon.png");
                    break;

                case TriggerType.Gameplay:
                    Gizmos.DrawIcon(transform.position, "GameplayIcon.png");
                    break;

                case TriggerType.Audio:
                    Gizmos.DrawIcon(transform.position, "AudioIcon.png");
                    break;

                case TriggerType.Camera:
                    Gizmos.DrawIcon(transform.position, "CameraIcon.png");
                    break;

                case TriggerType.None:
                    break;

                default:
                    break;

            }
        }

        if (TryGetComponent(out BoxCollider bc) && showBoundries)
        {
            Gizmos.color = Color.red;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(bc.center, bc.size);
        }
        else if (TryGetComponent(out SphereCollider sc) && showBoundries)
        {
            Gizmos.color = Color.red;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireSphere(sc.center, sc.radius);
        }
    }
    #endregion

    #region Coroutines
    IEnumerator EnterDelayed()
    {
        yield return new WaitForSeconds(triggerDelay);
        EventTriggerEnter.Invoke();
    }

    IEnumerator StayDelayed()
    {
        yield return new WaitForSeconds(triggerDelay);
        EventTriggerStay.Invoke();
    }

    IEnumerator ExitDelayed()
    {
        yield return new WaitForSeconds(triggerDelay);
        EventTriggerExit.Invoke();
    }
    #endregion

#if UNITY_EDITOR
    [MenuItem("Tools/Trigger/ToggleBoundries #t")]
    static void ToggleBoundries()
    {
        showBoundries = !showBoundries;
    }
#endif
}