
using UnityEngine;
using System.Collections.Generic;

public class CarAttack : MonoBehaviour
{
    [Header("Car Settings")]
    public float moveSpeed = 15f;
    public float travelDistance = 25f;
    public float attackWidth = 3f;
    public int attackDamage = 150;

    private Castle ownCastle;
    private Vector3 direction;
    private float traveledDistance = 0f;

    private bool initialized = false;

    // Bereits getroffene Gegner speichern
    private HashSet<Soldier> hitSoldiers =
        new HashSet<Soldier>();

    private HashSet<Castle> hitCastles =
        new HashSet<Castle>();

    // Auto initialisieren
    public void Initialize(Castle owner, Vector3 moveDirection)
    {
        ownCastle = owner;

        moveDirection.y = 0f;

        if (moveDirection.sqrMagnitude < 0.001f)
        {
            Destroy(gameObject);
            return;
        }

        direction = moveDirection.normalized;

        transform.rotation =
            Quaternion.LookRotation(direction);

        traveledDistance = 0f;

        hitSoldiers.Clear();
        hitCastles.Clear();

        initialized = true;
    }

    void Update()
    {
        if (!initialized)
            return;

        float distance = Mathf.Min(
            moveSpeed * Time.deltaTime,
            travelDistance - traveledDistance);

        Vector3 previousPosition = transform.position;

        // Auto bewegen
        transform.position += direction * distance;

        traveledDistance += distance;

        // Gegner im Fahrweg überprüfen
        CheckHits(previousPosition, distance);

        // Auto nach maximaler Fahrstrecke entfernen
        if (traveledDistance >= travelDistance)
        {
            Destroy(gameObject);
        }
    }

    // Treffer überprüfen
    private void CheckHits(
        Vector3 previousPosition, float distance)
    {
        Vector3 center =
            (previousPosition + transform.position) * 0.5f;

        Vector3 halfExtents = new Vector3(
            attackWidth / 2f,
            2f,
            2f + distance / 2f);

        Collider[] hits = Physics.OverlapBox(
            center,
            halfExtents,
            transform.rotation);

        foreach (Collider hit in hits)
        {
            Soldier enemy =
                hit.GetComponentInParent<Soldier>();

            if (enemy != null &&
                enemy.ownCastle != null &&
                enemy.ownCastle != ownCastle &&
                enemy.currentHealth > 0 &&
                hitSoldiers.Add(enemy))
            {
                enemy.TakeDamage(attackDamage);
            }

            Castle castle =
                hit.GetComponentInParent<Castle>();

            if (castle != null &&
                castle != ownCastle &&
                castle.currentHealth > 0 &&
                hitCastles.Add(castle))
            {
                castle.TakeDamage(attackDamage);
            }
        }
    }
}
