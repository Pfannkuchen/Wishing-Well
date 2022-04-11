using UnityEngine;
using UnityEngine.Audio;

public class ToggleAudioMusic : ToggleBase
{
    [SerializeField] private UserSettings _settings;
    private UserSettings Settings => _settings;
    
    [SerializeField] private AudioMixer _mixer;
    private AudioMixer Mixer => _mixer;
    
    [SerializeField] private string _parameter;
    private string Parameter => _parameter;
    

    private void Start()
    {
        UpdateValue(Settings.AudioMusic);
    }

    public override void ToggleValue()
    {
        Settings.AudioMusic = !Settings.AudioMusic;
        
        UpdateValue(Settings.AudioMusic);
    }

    private void UpdateValue(bool value)
    {
        Mixer.SetFloat(Parameter, value ? 0f : -80f);
        UpdateCheckSprite(value);
    }
}