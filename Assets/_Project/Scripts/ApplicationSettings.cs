using UnityEngine;

[CreateAssetMenu(fileName = "New ApplicationSettings", menuName = "ScriptableObjects/ApplicationSettings", order = 1)]
public class ApplicationSettings : ScriptableObject
{
    [SerializeField, Min(1)] private int _coinSelectionCount = 5;
    public int CoinSelectionCount => _coinSelectionCount;
    
    [SerializeField, Min(0.1f)] private float _coinScale = 1f;
    public float CoinScale => _coinScale;
    
    [SerializeField, Min(0.1f)] private Vector2 _metadataFadeInRange = new Vector2(1.5f, 3f);
    public Vector2 MetadataFadeInRange => _metadataFadeInRange;
    
    [SerializeField, Min(0.1f)] private Vector2 _metadataFadeOutRange = new Vector2(0.3f, 0.7f);
    public Vector2 MetadataFadeOutRange => _metadataFadeOutRange;
    
    [SerializeField, Min(1)] private int _minMetadataDisplay = 3;
    public int MinMetadataDisplay => _minMetadataDisplay;
    
    [SerializeField, Min(1)] private int _maxMetadataDisplay = 5;
    public int MaxMetadataDisplay => _maxMetadataDisplay;

    [SerializeField, Min(1)] private int _maxFlipRotations = 6;
    public int MaxFlipRotations => _maxFlipRotations;
    
    [SerializeField, Min(0.1f)] private float _underwaterGravity = 0.4f;
    public float UnderwaterGravity => _underwaterGravity;
}