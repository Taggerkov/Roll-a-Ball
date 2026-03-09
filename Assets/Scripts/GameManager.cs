using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic;

/// <summary>
/// Central manager responsible for game flow, UI state, and entity spawning.
/// Persists across scene loads as a singleton.
/// </summary>
public class GameManager : PersistentSingleton<GameManager>
{
    [SerializeField] private int scoreWin = 35;
    [SerializeField] private Vector3 jumpForce;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private GameObject canvasUIStart;
    [SerializeField] private GameObject canvasUIInGame;
    [SerializeField] private GameObject canvasUIWin;
    [SerializeField] private GameObject canvasUILose;
    [SerializeField] private TextMeshProUGUI inGameScoreValueText;
    [SerializeField] private TextMeshProUGUI winScoreValueText;
    [SerializeField] private TextMeshProUGUI loseScoreValueText;
    [SerializeField] private EntitySpawner playerSpawner;
    [SerializeField] private EntitySpawner wardenSpawner;
    [SerializeField] private EntitySpawner[] obstacleSpawners;
    private List<GameObject> _spawnedObstacles;

    /// <summary>Fired when the game state changes.</summary>
    /// <remarks>Subscribers receive the new <see cref="GameState"/> as a parameter.</remarks>
    public event Action<GameState> OnGameStateChanged;

    /// <summary>The player controller for the current session. Null outside a session.</summary>
    private PlayerController _playerController;

    /// <summary>The warden instance for the current session. Null outside a session.</summary>
    private GameObject _warden;

    /// <summary>
    /// Represents the current state of the game.
    /// </summary>
    public enum GameState
    {
        /// <summary>The game is on the main menu, no session is active.</summary>
        InMenu,

        /// <summary>A game session is actively running.</summary>
        InProgress,

        /// <summary>The game session has ended, either by winning or losing.</summary>
        Complete,
    }

    /// <summary>
    /// Describes the reason a game session was completed.
    /// </summary>
    public enum CompletionReason
    {
        /// <summary>The player met the win condition.</summary>
        GameWon,

        /// <summary>The player met the loss condition.</summary>
        GameLost
    }

    /// <summary>
    /// The current state of the game session.
    /// </summary>
    public GameState CurrentGameState { get; private set; }

    /// <summary>
    /// Initialises the game to the main menu state on scene load.
    /// </summary>
    private void Start()
    {
        CurrentGameState = GameState.InMenu;
        canvasUIStart.SetActive(true);
        _spawnedObstacles = new List<GameObject>();
    }

    /// <summary>
    /// Unsubscribes from player score events to prevent memory leaks.
    /// </summary>
    private void OnDisable()
    {
        if (!_playerController) return;
        _playerController.OnScoreUpdated -= UpdateUIScores;
        _playerController.OnScoreUpdated -= CheckGameCompletion;
    }

    /// <summary>
    /// Starts a new game session, spawning the player and warden, and switching to the in-game UI.
    /// </summary>
    /// <remarks>
    /// If a previous session's player or warden still exists, they are destroyed before spawning new ones.
    /// </remarks>
    public void BeginGame()
    {
        CurrentGameState = GameState.InProgress;

        // Clean up any leftover entities from a previous session
        if (_playerController) Destroy(_playerController.gameObject);
        if (_warden)
        {
            Destroy(_warden);
        }
        foreach (var spawnedObstacle in _spawnedObstacles) Destroy(spawnedObstacle);
        _spawnedObstacles.Clear();
        
        // Spawn and initialise the player
        var playerRef = playerSpawner.SpawnEntity();
        _playerController = playerRef.GetComponent<PlayerController>();
        var playerRb = playerRef.GetComponent<Rigidbody>();
        playerRb.AddForce(jumpForce, ForceMode.Force);

        // Spawn the warden
        _warden = wardenSpawner.SpawnEntity();

        // Spawn the coins
        foreach (var coin in FindObjectsByType<CoinBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            coin.gameObject.SetActive(true);

        // Spawn objects
        foreach (var spawner in obstacleSpawners) _spawnedObstacles.Add(spawner.SpawnEntity());

        // Transition UI to in-game state
        cameraController.enabled = true;
        canvasUIStart.SetActive(false);
        canvasUIInGame.SetActive(true);
        canvasUIWin.SetActive(false);
        canvasUILose.SetActive(false);

        // Subscribe to score events and reset the score display
        _playerController.OnScoreUpdated += UpdateUIScores;
        _playerController.OnScoreUpdated += CheckGameCompletion;
        UpdateUIScores(0);
    }

    /// <summary>
    /// Ends the current game session and transitions the UI based on the outcome.
    /// </summary>
    /// <param name="reason">Whether the session ended in a win or loss.</param>
    public void CompleteGame(CompletionReason reason)
    {
        CurrentGameState = GameState.Complete;
        OnGameStateChanged?.Invoke(CurrentGameState);
        cameraController.enabled = false;
        switch (reason)
        {
            case CompletionReason.GameWon:
            {
                canvasUIInGame.SetActive(false);
                canvasUIWin.SetActive(true);
                Destroy(_warden);
                break;
            }
            case CompletionReason.GameLost:
            {
                canvasUIInGame.SetActive(false);
                canvasUILose.SetActive(true);
                Destroy(_playerController.gameObject);
                break;
            }
            default:
                throw new ArgumentOutOfRangeException(nameof(reason), reason, "Unexpected completion reason.");
        }
    }

    /// <summary>
    /// Returns the player's <see cref="GameObject"/> for the current session.
    /// </summary>
    /// <returns>The active player GameObject, or <c>null</c> if no session is running.</returns>
    public GameObject GetPlayerObject()
    {
        return _playerController.gameObject;
    }

    /// <summary>
    /// Updates all score UI elements to reflect the current score.
    /// </summary>
    /// <param name="newScore">The score value to display.</param>
    private void UpdateUIScores(int newScore)
    {
        inGameScoreValueText.text = newScore.ToString();
        winScoreValueText.text = newScore.ToString();
        loseScoreValueText.text = newScore.ToString();
    }

    /// <summary>
    /// Checks whether the player has reached the winning score and completes the game if so.
    /// </summary>
    /// <param name="newScore">The latest score to evaluate against <see cref="scoreWin"/>.</param>
    private void CheckGameCompletion(int newScore)
    {
        if (newScore >= scoreWin) CompleteGame(CompletionReason.GameWon);
    }
}