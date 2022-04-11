using UnityEngine;

public class ToggleCoinAnimation : ToggleBase
{
    [SerializeField] private UserSettings _settings;
    private UserSettings Settings => _settings;


    private void Start()
    {
        UpdateValue(Settings.CoinAnimation);
    }

    public override void ToggleValue()
    {
        Settings.CoinAnimation = !Settings.CoinAnimation;
        
        UpdateValue(Settings.CoinAnimation);
    }

    private void UpdateValue(bool value)
    {
        UpdateCheckSprite(value);
    }
}
