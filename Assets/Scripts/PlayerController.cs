using UnityEngine;
using UnityEngine.InputSystem;
using System;

/// <summary>
/// Handles player movement, input, and score tracking.
/// </summary>
public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [Header("Settings")]
    [SerializeField][Range(0, 20)] private float speed = 10;
    
    /// <summary>Fired when the player's score changes.</summary>
    /// <remarks>Subscribers receive the new total score as a parameter.</remarks>
    public event Action<int> OnScoreUpdated;

    /// <summary>The input asset instance used to read player input.</summary>
    private PlayerInput _playerInput;

    /// <summary>The input action responsible for reading movement input.</summary>
    private InputAction _moveAction;

    /// <summary>The current movement input vector read each frame.</summary>
    private Vector2 _movementVector;

    /// <summary>The player's current score.</summary>
    private int _score;

    /// <summary>
    /// Initialises and enables the input system, and binds the move action.
    /// </summary>
    private void Start()
    {
        _playerInput = new PlayerInput();
        _playerInput.Enable();
        _moveAction = _playerInput.FindAction("Move");
    }

    /// <summary>
    /// Reads the current movement input each frame.
    /// </summary>
    private void Update()
    {
        _movementVector = _moveAction.ReadValue<Vector2>();
    }

    /// <summary>
    /// Applies movement force to the rigidbody based on the current input vector.
    /// </summary>
    private void FixedUpdate()
    {
        var force = new Vector3(_movementVector.x, 0, _movementVector.y) * speed;
        rb.AddForce(force, ForceMode.Force);
    }
    
    /// <summary>
    /// Disables the input system when the player is destroyed.
    /// </summary>
    private void OnDestroy()
    {
        _playerInput.Disable();
    }

    /// <summary>
    /// Adds a value to the player's score and notifies subscribers.
    /// </summary>
    /// <param name="value">The amount to add to the current score.</param>
    public void AddScore(int value)
    {
        _score += value;
        OnScoreUpdated?.Invoke(_score);
    }
}