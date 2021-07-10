using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    private static AudioManager instance;

    // Start is called before the first frame update

    public static AudioManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameObject("GM").AddComponent<AudioManager>();
            }
            return instance;
        }
    }

    void Awake()
    {
        if (instance != null)
        {
            DestroyImmediate(this);
            return;
        }
        instance = this;

        DontDestroyOnLoad(gameObject);
        foreach( Sound s in sounds ){
            gameObject.AddComponent<AudioSource>();
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
        }
    }

    public void Play(string name)
    {
        Sound sound = GetSound(name);
        if (sound == null) { return; }
        if (sound.EndCycle())
        {
            sound.Reset();
            sound.source.Play();
        }
        else
        {
            sound.AddCycle();
        }
       
    }

    public void Stop(string name)
    {
        Sound sound = GetSound(name);
        if (sound == null) { return; }
        sound.source.Stop();
        sound.Reset();
    }

    public Sound GetSound( string name)
    {
        foreach (Sound s in sounds)
        {
            if (s.name == name)
            {
                return s;
            }
        }
        return null;
    }
}
