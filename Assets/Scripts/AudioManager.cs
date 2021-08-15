using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private Sound[] sounds;

    private static AudioManager instance;

    public static AudioManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameObject("AudioManager").AddComponent<AudioManager>();
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
        //DontDestroyOnLoad(gameObject);

        foreach (Sound sound in sounds)
        {
            gameObject.AddComponent<AudioSource>();
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
        }
    }

    public bool Play()
    {
        // TODO - Abstract Sound transitions automaticaly
        string name = "Init";

        Sound sound = GetSound(name);
        if (sound == null)
            return false;

        if (sound.IsStop())
        {
            Debug.Log("New Cycle");
            sound.AddCycle();
            sound.Play();
            return true;
        }
        else if (sound.HasCompletedCyles())
        {
            Debug.Log("Restarting Cycles");
            sound.AddCycle();
            sound.Reset();
            return true;
        }

        return false;       
    }

    public void Stop(string name)
    {
        Sound sound = GetSound(name);
        if (sound == null)
            return;

        sound.Stop();
    }

    public Sound GetSound(string name)
    {
        foreach (Sound sound in sounds)
        {
            if (sound.name == name)
            {
                return sound;
            }
        }
        return null;
    }

    public float GetMomentHighPointCycle()
    {
        return GetSound("Init").momentHighPointCycle;
    }
}
