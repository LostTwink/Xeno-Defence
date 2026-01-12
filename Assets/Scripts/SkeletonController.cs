using UnityEngine;
using UnityEngine.AI;

public class SkeletonController : MonoBehaviour
{
    [Header("AI Settings")]
    public float attackRange = 2f;
    public float attackDamage = 20f;
    public float attackRate = 1f;
    public LayerMask targetLayerMask = -1; // Все слои, или настройте для Tower/Crystal

    private NavMeshAgent agent;
    private Health health;
    private Transform crystalTarget;
    private GameObject currentAttackTarget;
    private float lastAttackTime;

    public void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        health = GetComponent<Health>();
        if (agent == null) agent = gameObject.AddComponent<NavMeshAgent>();
    }

    public void Initialize()
    {
        // Найти кристалл автоматически
        GameObject crystal = GameObject.FindWithTag("Crystal");
        if (crystal != null)
        {
            crystalTarget = crystal.transform;
            MoveToCrystal();
        }
    }

    void Update()
    {
        if (health.isDead || crystalTarget == null) return;

        // Найти ближайшую цель для атаки
        GameObject nearestTarget = FindNearestTarget();
        if (nearestTarget != null)
        {
            agent.ResetPath(); // Остановить движение
            Attack(nearestTarget);
        }
        else
        {
            // Если нет целей — бежать к кристаллу
            MoveToCrystal();
        }
    }

    GameObject FindNearestTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, attackRange, targetLayerMask);
        GameObject nearest = null;
        float nearestDist = float.MaxValue;

        foreach (Collider hit in hits)
        {
            if ((hit.CompareTag("Tower") || hit.CompareTag("Crystal")) && hit.GetComponent<Health>() != null)
            {
                float dist = Vector3.Distance(transform.position, hit.transform.position);
                if (dist < nearestDist)
                {
                    nearestDist = dist;
                    nearest = hit.gameObject;
                }
            }
        }
        return nearest;
    }

    void MoveToCrystal()
    {
        if (crystalTarget != null && !agent.pathPending)
        {
            agent.SetDestination(crystalTarget.position);
        }
    }

    void Attack(GameObject target)
    {
        currentAttackTarget = target;
        transform.LookAt(target.transform);

        if (Time.time >= lastAttackTime + attackRate)
        {
            Health targetHealth = target.GetComponent<Health>();
            if (targetHealth != null)
            {
                targetHealth.TakeDamage(attackDamage);
            }
            lastAttackTime = Time.time;

            // Здесь можно добавить анимацию атаки: animator.SetTrigger("Attack");
        }
    }

    // Визуализация range в Scene view
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}