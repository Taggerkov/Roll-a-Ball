using UnityEngine;

/// <summary>
/// Follows and looks at the player, maintaining a fixed offset.
/// </summary>
[ExecuteAlways]
public class CameraController : MonoBehaviour
{
    [SerializeField] private Vector3 offset = new(0, 8, -7);

    /// <summary>The cached reference to the player GameObject.</summary>
    private GameObject _player;

    /// <summary>
    /// Retrieves and caches the player GameObject when the component is enabled.
    /// </summary>
    private void OnEnable()
    {
        _player = GameManager.Instance.GetPlayerObject();
    }

    /// <summary>
    /// Updates the camera position and rotation to follow the player.
    /// </summary>
    private void LateUpdate()
    {
        transform.position = _player.transform.position + offset;
        transform.LookAt(_player.transform.position);
    }
}