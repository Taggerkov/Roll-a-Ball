using UnityEngine;

namespace SO
{
    /// <summary>
    /// ScriptableObject storing configuration data for a coin.
    /// </summary>
    [CreateAssetMenu(fileName = "CoinSettings", menuName = "SO/CoinSettings")]
    public class CoinSettings : ScriptableObject
    {
        /// <summary>The score value awarded to the player on collection.</summary>
        public int value;
    }
}