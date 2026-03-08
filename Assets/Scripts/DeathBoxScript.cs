using UnityEngine;

/// <summary>
/// Triggers a game loss when the player falls out of bounds.
/// </summary>
public class DeathBoxScript : MonoBehaviour
{
    /// <summary>
    /// Triggers a game loss when the player enters the death zone.
    /// Ignores collisions outside an active session.
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        var gameManager = GameManager.Instance;
        if (gameManager.CurrentGameState != GameManager.GameState.InProgress) return;
        if (other.gameObject.CompareTag("Player")) gameManager.CompleteGame(GameManager.CompletionReason.GameLost);
    }
}