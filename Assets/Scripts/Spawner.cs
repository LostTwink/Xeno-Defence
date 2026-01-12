using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject skeletonPrefab;
    public float spawnHeightOffset = 1f; // Над землёй

    public void SpawnSkeleton()
    {
        if (skeletonPrefab == null) return;

        Vector3 spawnPos = transform.position + Vector3.up * spawnHeightOffset;
        GameObject skeleton = Instantiate(skeletonPrefab, spawnPos, transform.rotation);
        
        SkeletonController controller = skeleton.GetComponent<SkeletonController>();
        if (controller != null)
        {
            controller.Initialize();
        }

        EnemyManager.Instance.OnEnemySpawned();
    }
}