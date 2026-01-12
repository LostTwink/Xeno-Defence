using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using static EventBus;

public class TDManager : MonoBehaviour
{
    private bool inGame;
    private bool gameReseted;
    private float waveTimer;

    public WaveManager WaveManager;
    public float inGameTimer;
    public int waveNumber;

    public Vector3 startPosition;
    public Vector3 playPosition;
    public Transform Player;
    public void Awake()
    {
        HandleGameStateChange(this, new(false));
        GameStateChanged += HandleGameStateChange;
        Player.position = startPosition;
    }
    public void Update()
    {
        if (inGame)
        {
            HandleWave();
            inGameTimer += Time.deltaTime;
        }
        else if (!gameReseted)
        {
            //??
        }
    }
    public void HandleGameStateChange(object sender, GameStateChangedEventArgs arg)
    {
        inGame = arg.state;
        if (inGame)
        {
            Player.position = playPosition;
            gameReseted = false;//??
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
        waveNumber = 0;
        Player.position = startPosition;
        gameReseted = true;
        WaveManager.StopAllWaves();
    }
    public void HandleWave()
    {
        if (waveNumber == 0)
        {
            waveNumber++;
            WaveManager.StartCoroutine(WaveManager.StartWave1());
        }
        else if (waveNumber == 1)
        {
            waveNumber++;
            WaveManager.StartCoroutine(WaveManager.StartWave2());
        }
        else
        {
            WaveManager.StartCoroutine(WaveManager.SpawnInfiniteWave());
        }
    }
}
