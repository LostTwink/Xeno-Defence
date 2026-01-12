using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }

    public int aliveEnemies { get; private set; } = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OnEnemySpawned()
    {
        aliveEnemies++;
    }

    public void OnEnemyDied()
    {
        aliveEnemies--;
        if (aliveEnemies < 0) aliveEnemies = 0;
    }
}