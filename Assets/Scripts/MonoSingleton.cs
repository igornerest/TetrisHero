using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

public abstract class NetworkSingleton<T> : NetworkBehaviour where T : NetworkSingleton<T>
{
    private static T instance = null;

    public static T Instance
    {
        get
        {
            instance = instance ?? (FindObjectOfType(typeof(T)) as T);
            instance = instance ?? new GameObject(typeof(T).ToString(), typeof(T)).GetComponent<T>();
            return instance;
        }
    }

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    private void OnApplicationQuit()
    {
        instance = null;
    }
}
