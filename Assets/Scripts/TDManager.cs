using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using static EventBus;

public class TDManager : MonoBehaviour
{
    private bool inGame = false;

    public WaveManager WaveManager;
    public float inGameTimer;
    public void Awake()
    {
        HandleGameStateChange(this, new(false));
        GameStateChanged += HandleGameStateChange;
    }
    public void Update()
    {
        if (inGame)
            inGameTimer += Time.deltaTime;
    }
    public void HandleGameStateChange(object sender, GameStateChangedEventArgs arg)
    {
        inGame = arg.state;
        if (inGame)
        {
            StartCoroutine(WaveManager.StartWave1());
        }
        else
        {
            Leaderboard.Instance.SendNewValue(inGameTimer);
            ResetGame();
        }
    }
    public void ResetGame()
    {
        inGameTimer = 0f;
        WaveManager.StopAllWaves();
    }
    public void StartGame()
    {
        EventBus.Invoke(this, new GameStateChangedEventArgs(true));
        Debug.Log("Game Started");
    }
}
