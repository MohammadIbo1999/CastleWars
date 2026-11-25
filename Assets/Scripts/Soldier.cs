
using UnityEngine;

public class Soldier : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    public int attackDamage = 20;
    public float moveSpeed = 3f;
    public float attackRange = 1.5f;
    public float attackInterval = 1f;
    public float detectionRange = 5f;

    // Soldatenabstand
    public float separationRadius = 0.8f;
    public float separationStrength = 3f;
    public LayerMask soldierLayer;

    public Castle ownCastle;
    public Castle targetCastle;
    public Soldier targetSoldier;

    private float nextAttackTime = 0f;
    private float nextSearchTime = 0f;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (ownCastle == null || currentHealth <= 0)
            return;

        if (Time.time >= nextSearchTime)
        {
            FindEnemySoldier();
            nextSearchTime = Time.time + 0.5f;
        }

        if (targetSoldier != null &&
            targetSoldier.currentHealth > 0)
        {
            MoveAndAttack(targetSoldier.transform);
        }
        else
        {
            if (targetCastle == null ||
                targetCastle == ownCastle ||
                !targetCastle.gameObject.activeInHierarchy)
            {
                FindTargetCastle();
            }

            if (targetCastle != null)
            {
                MoveAndAttack(targetCastle.transform);
            }
        }

        ApplySeparation();
    }

    // Gegnerische Soldaten suchen
    private void FindEnemySoldier()
    {
        Soldier[] soldiers = FindObjectsOfType<Soldier>();

        float shortestDistance = detectionRange;
        targetSoldier = null;

        foreach (Soldier soldier in soldiers)
        {
            if (soldier == this ||
                soldier.ownCastle == null ||
                soldier.ownCastle == ownCastle ||
                soldier.currentHealth <= 0)
                continue;

            float distance = Vector3.Distance(
                transform.position,
                soldier.transform.position);

            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                targetSoldier = soldier;
            }
        }
    }

    // Gegnerische Burg suchen
    private void FindTargetCastle()
    {
        Castle[] castles = FindObjectsOfType<Castle>();

        float shortestDistance = Mathf.Infinity;
        targetCastle = null;

        foreach (Castle castle in castles)
        {
            if (castle == ownCastle ||
                !castle.gameObject.activeInHierarchy)
                continue;

            float distance = Vector3.Distance(
                transform.position,
                castle.transform.position);

            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                targetCastle = castle;
            }
        }
    }

    // Bewegung und Angriff
    private void MoveAndAttack(Transform target)
    {
        Vector3 targetPosition = target.position;

        Collider targetCollider =
            target.GetComponent<Collider>();

        if (targetCollider != null)
        {
            targetPosition = targetCollider.ClosestPoint(
                transform.position);
        }

        targetPosition.y = transform.position.y;

        float distance = Vector3.Distance(
            transform.position, targetPosition);

        if (distance > attackRange)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                moveSpeed * Time.deltaTime);
        }
        else
        {
            Attack();
        }
    }

    // Abstand zu anderen Soldaten halten
    private void ApplySeparation()
    {
        Collider[] nearbySoldiers = Physics.OverlapSphere(
            transform.position,
            separationRadius,
            soldierLayer);

        Vector3 separation = Vector3.zero;

        foreach (Collider nearby in nearbySoldiers)
        {
            if (nearby.gameObject == gameObject)
                continue;

            Vector3 direction =
                transform.position - nearby.transform.position;

            direction.y = 0f;

            float distance = direction.magnitude;

            if (distance < 0.001f)
            {
                direction = Random.insideUnitSphere;
                direction.y = 0f;
                distance = 0f;
            }

            if (distance < separationRadius)
            {
                separation += direction.normalized *
                    (1f - distance / separationRadius);
            }
        }

        separation = Vector3.ClampMagnitude(separation, 1f);

        transform.position += separation *
            separationStrength * Time.deltaTime;
    }

    // Gegner angreifen
    private void Attack()
    {
        if (Time.time < nextAttackTime)
            return;

        if (targetSoldier != null &&
            targetSoldier.currentHealth > 0)
        {
            targetSoldier.TakeDamage(attackDamage);
        }
        else if (targetCastle != null)
        {
            targetCastle.TakeDamage(attackDamage);
        }

        nextAttackTime = Time.time + attackInterval;
    }

    // Schaden erhalten
    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0)
            return;

        currentHealth = Mathf.Max(
            0, currentHealth - damage);

        if (currentHealth == 0)
        {
            Destroy(gameObject);
        }
    }
}
