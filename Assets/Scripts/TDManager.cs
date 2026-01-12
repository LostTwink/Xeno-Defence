using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using static EventBus;

public class TDManager : MonoBehaviour
{
    private bool inGame = false;

    public WaveManager WaveManager;
    public float inGameTimer;

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
            inGameTimer += Time.deltaTime;
    }
    public void HandleGameStateChange(object sender, GameStateChangedEventArgs arg)
    {
        inGame = arg.state;
        if (inGame)
        {
            Player.position = playPosition;
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
        Player.position = startPosition;
        WaveManager.StopAllWaves();
    }
}
