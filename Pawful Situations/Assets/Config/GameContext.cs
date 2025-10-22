using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameContext : MonoBehaviour
{
    public static GameContext Instance { get; private set; }
    public GameSettings Settings = new GameSettings();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(this.gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        // LoadFromPrefs();
    }

    //public void SaveToPrefs()
    //{
    //    PlayerPrefs.SetInt
    //}
}
