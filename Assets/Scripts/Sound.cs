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

    private int howManyCycles;
    
    public void AddCycle()
    {
        howManyCycles++;
    } 

    public bool EndCycle()
    {
        return howManyCycles == totalCycles-1;
    }

    public void Reset()
    {
        howManyCycles = 0;
    }

    public float TimeCourse()
    {
        return source.clip.length / totalCycles;
    }
}
