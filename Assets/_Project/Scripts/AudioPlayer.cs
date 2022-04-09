using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public class AudioPlayer : SingletonMonoBehaviour<AudioPlayer>
{
    private List<AudioSource> _sourcePool;
    private List<AudioSource> SourcePool => _sourcePool ??= new List<AudioSource>();

    public void PlayRandomAudioClip(AudioClip[] clips, Vector2 volumeRange, Vector2 pitchRange)
    {
        if (clips == null || clips.Length <= 0) return;
        
        AudioClip randomClip = clips[UnityEngine.Random.Range(0, clips.Length)];
        if (randomClip == null) return;

        AudioSource availableSource = SourcePool.First(x => !x.isPlaying);
        if (availableSource == null)
        {
            availableSource = gameObject.AddComponent<AudioSource>();
            SourcePool.Add(availableSource);
        }

        availableSource.clip = randomClip;
        availableSource.volume = UnityEngine.Random.Range(volumeRange.x, volumeRange.y);
        availableSource.pitch = UnityEngine.Random.Range(pitchRange.x, pitchRange.y);
        availableSource.Play();
    }
}