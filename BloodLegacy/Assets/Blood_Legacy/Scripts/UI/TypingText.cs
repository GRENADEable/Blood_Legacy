using UnityEngine;
using TMPro;

public class TypingText : MonoBehaviour
{
    #region Public Variables
    public float revealTime = 0.02f;
    #endregion

    #region Private Variables
    private TextMeshProUGUI _typingText;
    private string _tempText;
    private string[] _textChar;
    private bool _isRevealing;
    private float _currTime = 0f;
    private int _charCount = 0;
    #endregion

    #region Unity Callbacks
    void OnEnable()
    {
        _typingText = GetComponent<TextMeshProUGUI>();
        _tempText = _typingText.text;
        _textChar = new string[_typingText.text.Length];

        for (int i = 0; i < _typingText.text.Length; i++)
            _textChar[i] = _typingText.text.Substring(i, 1);

        _typingText.text = "";
        _isRevealing = true;
    }

    void OnDisable()
    {
        _currTime = 0f;
        _charCount = 0;
        _isRevealing = false;
    }

    void Update()
    {
        if (_isRevealing)
        {
            if (_charCount < _textChar.Length)
            {
                _currTime += Time.deltaTime;
                if (_currTime >= revealTime)
                {
                    _typingText.text += _textChar[_charCount];
                    _charCount++;
                    _currTime = 0f;
                }
            }

            if (_charCount == _textChar.Length)
                _isRevealing = false;
        }
    }
    #endregion
}