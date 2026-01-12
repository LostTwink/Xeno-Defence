using TMPro;
using UnityEngine;

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
        countdown.text = $"Волна началсь";
    }
    private void OnCountdownTick(int time)
    {
        countdown.text = $"До волны осталось: {time}";
    }
}
