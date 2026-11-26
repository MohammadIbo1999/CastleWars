
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float speed = 12f;
    public float rotationSpeed = 720f;
    public float hitDistance = 0.2f;

    private Soldier targetSoldier;
    private Castle targetCastle;

    private int damage;
    private bool initialized = false;

    // Projektil initialisieren
    public void Initialize(
        Soldier soldier,
        Castle castle,
        int attackDamage)
    {
        targetSoldier = soldier;
        targetCastle = castle;
        damage = attackDamage;

        initialized = true;
    }

    void Update()
    {
        if (!initialized)
            return;

        Transform target = GetTarget();

        // Ziel existiert nicht mehr
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 targetPosition = target.position;

        Collider targetCollider =
            target.GetComponent<Collider>();

        if (targetCollider != null)
        {
            targetPosition = targetCollider.ClosestPoint(
                transform.position);
        }

        // Projektil bewegen
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            speed * Time.deltaTime);

        // Flasche während des Fluges drehen
        transform.Rotate(
            Vector3.right,
            rotationSpeed * Time.deltaTime);

        // Treffer überprüfen
        float distance = Vector3.Distance(
            transform.position,
            targetPosition);

        if (distance <= hitDistance)
        {
            HitTarget();
        }
    }

    // Aktuelles Ziel ermitteln
    private Transform GetTarget()
    {
        if (targetSoldier != null &&
            targetSoldier.currentHealth > 0 &&
            targetSoldier.gameObject.activeInHierarchy)
        {
            return targetSoldier.transform;
        }

        if (targetCastle != null &&
            targetCastle.currentHealth > 0 &&
            targetCastle.gameObject.activeInHierarchy)
        {
            return targetCastle.transform;
        }

        return null;
    }

    // Schaden verursachen
    private void HitTarget()
    {
        if (targetSoldier != null &&
            targetSoldier.currentHealth > 0)
        {
            targetSoldier.TakeDamage(damage);
        }
        else if (targetCastle != null &&
                 targetCastle.currentHealth > 0)
        {
            targetCastle.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}
