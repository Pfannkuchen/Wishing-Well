using StylizedWater;
using UnityEngine;

public class ToggleTextDistortion : ToggleBase
{
    [SerializeField] private UserSettings _settings;
    private UserSettings Settings => _settings;

    /*
    [SerializeField] private StylizedWaterURP _water;
    private StylizedWaterURP Water => _water;
    */
    
    [SerializeField] private Material _water;
    private Material Water => _water;
    

    private void Start()
    {
        UpdateValue(Settings.TextDistortion);
    }

    public override void ToggleValue()
    {
        Settings.TextDistortion = !Settings.TextDistortion;
        
        UpdateValue(Settings.TextDistortion);
    }

    private void UpdateValue(bool value)
    {
        /*
        Water.refractionStrength = value ? 0.06f : 0f;
        Water.WriteMaterialProperties();
        */
        
        Water.SetFloat("_SettingsRefraction", value ? 1f : 0f);
        
        UpdateCheckSprite(value);
    }
}