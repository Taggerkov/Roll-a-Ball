using UnityEngine;

/// <summary>
/// Abstract base class for persistent MonoBehaviour singletons.
/// Provides shared shutdown detection logic for <see cref="PersistentSingleton{T}"/>.
/// </summary>
public abstract class PersistentSingletonBase : MonoBehaviour
{
    /// <summary>
    /// Set to <c>true</c> when the application begins shutting down.
    /// Prevents an Instance from creating or looking up instances during teardown.
    /// </summary>
    protected static bool IsQuitting { get; private set; }

    /// <summary>
    /// Called when the instance is loaded. Override to implement singleton enforcement.
    /// </summary>
    protected virtual void Awake() { }

    /// <summary>
    /// Sets a flag to prevent instance creation or lookup during application shutdown.
    /// </summary>
    protected virtual void OnApplicationQuit() => IsQuitting = true;

    /// <summary>
    /// Called when the instance is destroyed. Override to clean up the static instance reference.
    /// </summary>
    protected virtual void OnDestroy() { }
}

/// <summary>
/// A generic MonoBehaviour singleton that persists across scene loads.
/// Ensures only one instance of <typeparamref name="T"/> exists at any time.
/// </summary>
/// <typeparam name="T">The MonoBehaviour type to be managed as a singleton. Must derive from <see cref="MonoBehaviour"/>.</typeparam>
/// <remarks>
/// Shutdown safety is handled by <see cref="PersistentSingletonBase.IsQuitting"/>.
/// </remarks>
public class PersistentSingleton<T> : PersistentSingletonBase where T : MonoBehaviour
{
    /// <summary>
    /// The backing field for <see cref="Instance"/>.
    /// </summary>
    private static T _instance;

    /// <summary>
    /// The global instance of <typeparamref name="T"/>.
    /// If no instance exists in the scene, one will be created automatically.
    /// Returns <c>null</c> during application shutdown.
    /// </summary>
    public static T Instance
    {
        get
        {
            // Don’t create/lookup during shutdown.
            if (IsQuitting) return null;
            if (_instance != null) return _instance;

            // Try find existing instance in loaded scenes.
            _instance = FindFirstObjectByType<T>();
            if (_instance != null) return _instance;

            // Create a new GameObject instance if none exists.
            var go = new GameObject(typeof(T).Name);
            _instance = go.AddComponent<T>();
            DontDestroyOnLoad(go);
            return _instance;
        }
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Destroys this GameObject if a duplicate instance is detected.
    /// Otherwise, registers itself as the singleton and marks it persistent across scene loads.
    /// </remarks>
    protected override void Awake()
    {
        if (_instance != null && _instance != this) { Destroy(gameObject); return; }
        _instance = (T)(MonoBehaviour)this;
        DontDestroyOnLoad(gameObject);
    }

    /// <inheritdoc/>
    /// <remarks>Clears <see cref="_instance"/> if this is the current singleton.</remarks>
    protected override void OnDestroy()
    {
        if (_instance == this) _instance = null;
    }
}