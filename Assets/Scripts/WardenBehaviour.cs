using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Controls the warden enemy, chasing the player using NavMesh pathfinding.
/// </summary>
public class WardenBehaviour : MonoBehaviour
{
    /// <summary>The player's transform, used as the navigation destination.</summary>
    private Transform _player;

    /// <summary>The NavMesh agent used to navigate towards the player.</summary>
    private NavMeshAgent _navMeshAgent;

    /// <summary>Whether the game has ended, used to halt navigation.</summary>
    private bool _isGameComplete;

    /// <summary>
    /// Caches the player transform and NavMesh agent on initialisation.
    /// </summary>
    private void Awake()
    {
        _player = GameManager.Instance.GetPlayerObject().transform;
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    /// <summary>
    /// Continuously updates the warden's navigation destination to the player's position.
    /// </summary>
    private void Update()
    {
        if (_isGameComplete) return;
        _navMeshAgent.SetDestination(_player.position);
    }

    /// <summary>
    /// Subscribes to game state change events.
    /// </summary>
    private void OnEnable()
    {
        GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
    }

    /// <summary>
    /// Unsubscribes from game state change events.
    /// Guards against the GameManager being null during shutdown.
    /// </summary>
    private void OnDisable()
    {
        if (!GameManager.Instance) return;
        GameManager.Instance.OnGameStateChanged -= OnGameStateChanged;
    }

    /// <summary>
    /// Triggers a game loss when the warden collides with the player.
    /// </summary>
    private void OnCollisionEnter(Collision collision)
    {
        if (_isGameComplete) return;
        if (collision.gameObject.CompareTag("Player"))
            GameManager.Instance.CompleteGame(GameManager.CompletionReason.GameLost);
    }

    /// <summary>
    /// Halts navigation when the game is complete.
    /// </summary>
    private void OnGameStateChanged(GameManager.GameState gameState)
    {
        _isGameComplete = gameState == GameManager.GameState.Complete;
    }
}