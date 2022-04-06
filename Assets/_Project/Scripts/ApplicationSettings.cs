using UnityEngine;

[CreateAssetMenu(fileName = "New ApplicationSettings", menuName = "ScriptableObjects/ApplicationSettings", order = 1)]
public class ApplicationSettings : ScriptableObject
{
    [SerializeField] private int _coinSelectionCount;
    public int CoinSelectionCount => _coinSelectionCount = 5;

    [SerializeField] private int _maxFlipRotations;
    private int MaxFlipRotations => _maxFlipRotations = 10;
}