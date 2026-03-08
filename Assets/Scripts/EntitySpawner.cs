using UnityEngine;

/// <summary>
/// Spawns a configured prefab at this transform's position.
/// </summary>
public class EntitySpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject prefab;
    
    /// <summary>
    /// Instantiates the prefab at this spawner's position with no rotation.
    /// </summary>
    /// <returns>The spawned <see cref="GameObject"/>.</returns>
    public GameObject SpawnEntity() {
        return Instantiate(prefab, transform.position, Quaternion.identity);
    }
}