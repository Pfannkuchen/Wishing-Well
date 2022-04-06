using UnityEngine;

[CreateAssetMenu(fileName = "New DatabaseSettings", menuName = "ScriptableObjects/DatabaseSettings", order = 1)]
public class DatabaseSettings : ScriptableObject
{
    [SerializeField] private WebRequestSettings _requestSettings;
    public WebRequestSettings RequestSettings => _requestSettings;

    [SerializeField, Min(16)] private int _maxImageResolution = 800;
    public int MaxImageResolution => _maxImageResolution;
}
