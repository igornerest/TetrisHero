using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    [Range(0f, 1f)]
    public float volume;
    [HideInInspector]
    public AudioSource source;
    public int totalCycles;
    public float momentHighPointCycle;

    private int howManyCycles;
    private bool stop = true;
    public void AddCycle()
    {
        howManyCycles++;
    } 

    public bool IsStop()
    {
        if( stop) { return true; }
        else if (EndCycle())
        { 
            stop = true; 
        }
        return stop;
    }
    public bool EndCycle()
    {
        return howManyCycles == totalCycles;
    }

    public void Stop()
    {
        source.Stop();
        Reset();
    }

    public void Reset()
    {
        howManyCycles = 0;
        stop = true;
    }

    public void Play()
    {
        source.Play();
        stop = false;
    }
}
