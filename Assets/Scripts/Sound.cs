using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;
    
    [Range(0f, 1f)] 
    public float volume;
    
    [HideInInspector] 
    public AudioSource source;
    public AudioClip clip;
    
    public int totalCycles;
    public float momentHighPointCycle;

    private int howManyCycles = 0;
    private float lastPlaybackPosition = 0;

    public void AddCycle()
    {
        howManyCycles++;
    } 

    public bool IsStop()
    {
        return !source.isPlaying;
    }

    public bool HasCompletedCyles()
    {
        return howManyCycles == totalCycles;
    }

    public void Stop()
    {
        source.Stop();
    }

    public void Reset()
    {
        howManyCycles = 0;
        lastPlaybackPosition = 0;
        Play();
    }

    public void Play()
    {
        float playbackDuration = clip.length / totalCycles;

        source.time = lastPlaybackPosition;
        source.volume = volume;
        source.Play();
        source.SetScheduledEndTime(AudioSettings.dspTime + playbackDuration);

        lastPlaybackPosition += playbackDuration;
    }
}
