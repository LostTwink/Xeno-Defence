using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class WaveManager : MonoBehaviour
{
    [Header("Wave Settings")]
    public Spawner[] spawners = new Spawner[4]; // 4 спаунера
    public GameObject skeletonPrefab; // Для fallback

    [Header("Wave 1 (One Side)")]
    public int wave1Enemies = 10;
    public float wave1SpawnInterval = 1.5f;

    [Header("Wave 2 (Two Sides)")]
    public int wave2EnemiesPerSide = 15;
    public float wave2SpawnInterval = 1.2f;

    [Header("Wave 3 (Infinite)")]
    public float initialInfiniteInterval = 2f;
    public float intervalDecreasePerWave = 0.1f; // Уменьшение каждые N врагов
    public int enemiesBeforeIntervalDecrease = 10;

    [Header("Pre-Wave Countdown")]
    public int preWaveCountdownSeconds = 10; // N раз по секунде
    public UnityEvent<int> onPreWaveCountdownTick; // Invoked each second with remaining seconds
    public UnityEvent onPreWaveCountdownFinished; // Invoked when countdown completes

    private int currentWave = 0;
    private bool waveActive = false;
    private float currentInfiniteInterval;

    void Start()
    {
        if (spawners.Length != 4)
        {
            Debug.LogError("WaveManager: Exactly 4 spawners required!");
        }
        currentInfiniteInterval = initialInfiniteInterval;
    }

    public IEnumerator StartWave1()
    {
        //задержка перед началом N раз по секунде с выводом в UI
        yield return new WaitUntil(() => EnemyManager.Instance.aliveEnemies == 0);
        yield return StartCoroutine(PreWaveCountdown(preWaveCountdownSeconds));
        currentWave = 1;
        Debug.Log("Wave 1 started!");
        StartCoroutine(SpawnWave(0, wave1Enemies, wave1SpawnInterval)); // Одна сторона (index 0)
        yield return null; 
        StartCoroutine(StartWave2());
    }

    public IEnumerator StartWave2()
    {
        //задержка перед началом N раз по секунде с выводом в UI
        yield return new WaitUntil(() => EnemyManager.Instance.aliveEnemies == 0);
        yield return StartCoroutine(PreWaveCountdown(preWaveCountdownSeconds));
        currentWave = 2;
        Debug.Log("Wave 2 started!");
        StartCoroutine(SpawnWave(0, wave2EnemiesPerSide, wave2SpawnInterval));
        yield return null; 
        StartCoroutine(SpawnWave(1, wave2EnemiesPerSide, wave2SpawnInterval)); // Две стороны
        yield return null; 
        StartCoroutine(StartInfiniteWave());
    }

    public IEnumerator StartInfiniteWave()
    {
        //задержка перед началом N раз по секунде с выводом в UI
        yield return new WaitUntil(() => EnemyManager.Instance.aliveEnemies == 0);
        yield return StartCoroutine(PreWaveCountdown(preWaveCountdownSeconds));
        currentWave = 3;
        Debug.Log("Infinite Wave 3 started!");
        StartCoroutine(SpawnInfiniteWave());
    }
    public IEnumerator PreWaveCountdown(int seconds)
    {
        if (seconds <= 0)
        {
            onPreWaveCountdownFinished?.Invoke();
            yield break;
        }

        for (int s = seconds; s > 0; s--)
        {
            onPreWaveCountdownTick?.Invoke(s);
            yield return new WaitForSeconds(1f);
        }

        onPreWaveCountdownFinished?.Invoke();
    }

    IEnumerator SpawnWave(int spawnerIndex, int enemyCount, float spawnInterval)
    {
        waveActive = true;
        for (int i = 0; i < enemyCount; i++)
        {
            if (spawners[spawnerIndex] != null)
            {
                spawners[spawnerIndex].SpawnSkeleton();
            }
            else if (skeletonPrefab != null)
            {
                // Fallback
                Spawner tempSpawner = spawners[spawnerIndex]?.GetComponent<Spawner>();
                tempSpawner?.SpawnSkeleton();
            }
            yield return new WaitForSeconds(spawnInterval);
        }
        waveActive = false;
    }

    public IEnumerator SpawnInfiniteWave()
    {
        int spawnedSinceDecrease = 0;
        while (true) // Бесконечно
        {
            int randomSpawnerIndex = Random.Range(0, 4);
            if (spawners[randomSpawnerIndex] != null)
            {
                spawners[randomSpawnerIndex].SpawnSkeleton();
                spawnedSinceDecrease++;
            }

            if (spawnedSinceDecrease >= enemiesBeforeIntervalDecrease)
            {
                currentInfiniteInterval = Mathf.Max(0.3f, currentInfiniteInterval - intervalDecreasePerWave);
                spawnedSinceDecrease = 0;
                Debug.Log($"Infinite wave interval decreased to {currentInfiniteInterval}");
            }

            yield return new WaitForSeconds(currentInfiniteInterval);
        }
    }
    public void StopAllWaves()
    {
        StopAllCoroutines();
        waveActive = false;
        currentWave = 0;

        // Despawn/kill all spawned skeletons in the scene (modern API)
        SkeletonController[] skeletons = UnityEngine.Object.FindObjectsByType<SkeletonController>(FindObjectsSortMode.None);
        foreach (SkeletonController sk in skeletons)
        {
            if (sk == null) continue;
            Health h = sk.GetComponent<Health>();
            if (h != null && !h.isDead)
            {
                // Apply remaining health as damage so Die() executes and EnemyManager count updates
                h.TakeDamage(h.currentHealth);
            }
            else
            {
                Destroy(sk.gameObject);
            }
        }
    }
}