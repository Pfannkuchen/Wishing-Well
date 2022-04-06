using UnityEngine;

[CreateAssetMenu(fileName = "New WebRequestSettings", menuName = "ScriptableObjects/WebRequestSettings", order = 1)]
public class WebRequestSettings : ScriptableObject
{
    [SerializeField] private string _url;
    public string URL => _url;
}