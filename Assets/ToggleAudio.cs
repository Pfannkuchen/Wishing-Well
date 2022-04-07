using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class ToggleAudio : ToggleBase
{
    [SerializeField] private UserSettings _settings;
    private UserSettings Settings => _settings;
    
    [SerializeField] private AudioMixer _mixer;
    private AudioMixer Mixer => _mixer;
    
    [SerializeField] private string _parameter;
    private string Parameter => _parameter;
    

    private void Start()
    {
        UpdateValue(Settings.AudioEverything);
    }

    public override void ToggleValue()
    {
        Settings.AudioEverything = !Settings.AudioEverything;
        
        UpdateValue(Settings.AudioEverything);
    }

    private void UpdateValue(bool value)
    {
        Mixer.SetFloat(Parameter, Settings.AudioEverything ? 0f : -80f);
        UpdateCheckSprite(value);
    }
}
