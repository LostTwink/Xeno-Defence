using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class UI : MonoBehaviour
{
    public WaveManager WaveManager;
    public TextMeshPro countdown;
    private void Awake()
    {
        WaveManager.onPreWaveCountdownTick.AddListener(OnCountdownTick);
        WaveManager.onPreWaveCountdownFinished.AddListener(OnCountdownFinished);
    }
    private void OnCountdownFinished()
    {

    }
    private void OnCountdownTick(int time)
    {
        countdown.text = $"До волны осталось: {time}";
    }
}
