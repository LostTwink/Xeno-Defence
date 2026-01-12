using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;

    public bool isDead => currentHealth <= 0;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        // Для скелета: EnemyManager.OnEnemyDied()
        // Для башни: Destroy(gameObject);
        // Для кристалла: Game Over UI
        if (CompareTag("Skeleton"))
        {
            EnemyManager.Instance.OnEnemyDied();
        }
        Destroy(gameObject);
    }
}