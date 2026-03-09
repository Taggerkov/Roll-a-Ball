using UnityEngine;

/// <summary>
/// Resets this GameObject's position and rotation to its initial state when a new game session begins.
/// Attach to any object that should return to its starting transform on restart.
/// </summary>
public class ResetOnGameStart : MonoBehaviour
{
    /// <summary>The position recorded at startup, restored on each new session.</summary>
    private Vector3 _initialPosition;

    /// <summary>The rotation recorded at startup, restored on each new session.</summary>
    private Quaternion _initialRotation;

    /// <summary>
    /// Caches the initial transform values.
    /// </summary>
    private void Start()
    {
        _initialPosition = transform.position;
        _initialRotation = transform.rotation;
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
    /// Resets the transform to its initial state when a new session begins.
    /// </summary>
    private void OnGameStateChanged(GameManager.GameState state)
    {
        if (state == GameManager.GameState.Complete) return;
        transform.position = _initialPosition;
        transform.rotation = _initialRotation;
    }
}