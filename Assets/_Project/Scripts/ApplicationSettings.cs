using UnityEngine;

[CreateAssetMenu(fileName = "New ApplicationSettings", menuName = "ScriptableObjects/ApplicationSettings", order = 1)]
public class ApplicationSettings : ScriptableObject
{
    [SerializeField] private Coin _coinPrefab;
    public Coin CoinPrefab => _coinPrefab;
    
    [SerializeField, Min(1)] private int _coinSelectionCount = 5;
    public int CoinSelectionCount => _coinSelectionCount;
    
    [SerializeField, Min(0.1f)] private float _coinScale = 1f;
    public float CoinScale => _coinScale;

    [SerializeField, Min(0)] private int _maxFlipRotations = 6;
    public int MaxFlipRotations => _maxFlipRotations;
    
    [SerializeField, Min(0.1f)] private float _underwaterGravity = 0.4f;
    public float UnderwaterGravity => _underwaterGravity;
}