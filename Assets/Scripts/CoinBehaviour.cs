using SO;
using UnityEngine;

/// <summary>
/// Controls coin behaviour, awarding score on player contact and rotating visually.
/// </summary>
public class CoinBehaviour : MonoBehaviour
{
    [SerializeField]
    private CoinSettings settings;
    
    /// <summary>
    /// Awards score to the player and destroys the coin on contact.
    /// Ignores collisions with non-player objects.
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        var playerController = other.GetComponent<PlayerController>();
        playerController.AddScore(settings.value);
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Rotates the coin continuously on the Y axis.
    /// </summary>
    private void Update()
    {
        transform.Rotate (new Vector3 (0, 60, 0) * Time.deltaTime);
    }
}