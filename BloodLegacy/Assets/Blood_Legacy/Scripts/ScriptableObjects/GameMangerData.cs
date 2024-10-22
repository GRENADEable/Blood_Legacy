using UnityEngine;

[CreateAssetMenu(fileName = "GameManager_Data", menuName = "Managers/GameManagerData")]
public class GameMangerData : ScriptableObject
{
    #region Public Variables
    [Space, Header("Enums")]
    public GameState currState = GameState.Book;
    public enum GameState { Intro, Book, MiniGame, Exit };
    #endregion

    #region My Functions

    #region Cursor
    /// <summary>
    /// Locks the user's cusor;
    /// </summary>
    /// <param name="isLocked"> If true, lock the cursor in place, if false, free the cursor; </param>
    public void LockCursor(bool isLocked)
    {
        if (isLocked)
            Cursor.lockState = CursorLockMode.Locked;
        else
            Cursor.lockState = CursorLockMode.None;
    }

    /// <summary>
    /// User's cursor visibility;
    /// </summary>
    /// <param name="isVisible"> If true, lock show cursor, if false, hide cursor; </param>
    public void VisibleCursor(bool isVisible)
    {
        if (isVisible)
            Cursor.visible = true;
        else
            Cursor.visible = false;
    }
    #endregion

    /// <summary>
    /// Pauses game using Time.timeScale;
    /// </summary>
    /// <param name="isPaused"> If true, pause game, if false, un-pause game; </param>
    public void TogglePause(bool isPaused)
    {
        if (isPaused)
            Time.timeScale = 0f;
        else
            Time.timeScale = 1f;
    }

    #region Game States
    /// <summary>
    /// Changes the state the game is running on using enums;
    /// </summary>
    /// <param name="state"> Uses string to check which state to change to. The string has to be exact or else it won't work; </param>
    public void ChangeGameState(string state)
    {
        if (state.Contains("Intro"))
            currState = GameState.Intro;

        if (state.Contains("Book"))
            currState = GameState.Book;

        if (state.Contains("MiniGame"))
            currState = GameState.MiniGame;

        if (state.Contains("Exit"))
            currState = GameState.Exit;
    }

    /// <summary>
    /// Changes Level using the int Values;
    /// </summary>
    /// <param name="level"> Int variable for the Scenes added in Build Windows; </param>
    public void ChangeLevel(int level) => Application.LoadLevel(level);

    /// <summary>
    /// Closes the Game;
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game Closed");
    }
    #endregion

    #endregion
}