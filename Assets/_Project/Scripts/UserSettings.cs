using UnityEngine;

[CreateAssetMenu(fileName = "New UserSettings", menuName = "ScriptableObjects/UserSettings", order = 1)]
public class UserSettings : ScriptableObject
{
    public bool AudioEverything = true;
    public bool AudioMusic = true;
    public bool TextDistortion = true;
    public bool CoinAnimation = true;
}