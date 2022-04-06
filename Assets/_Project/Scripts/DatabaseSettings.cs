using UnityEngine;

[CreateAssetMenu(fileName = "New DatabaseSettings", menuName = "ScriptableObjects/DatabaseSettings", order = 1)]
public class DatabaseSettings : ScriptableObject
{
    [SerializeField] private string _url;
    public string URL => _url;

    [SerializeField, Min(16)] private int _maxImageResolution;
    private int MaxImageResolution => _maxImageResolution = 800;
}
