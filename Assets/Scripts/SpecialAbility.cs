
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Soldier))]
public class SpecialAbility : MonoBehaviour
{
    private Soldier soldier;
    private float nextSpecialTime;
    private bool isUsingSpecial = false;

    public bool IsUsingSpecial => isUsingSpecial;

    [Header("Talahona Special")]
    public int handbagDamage = 50;
    public float handbagRadius = 3f;
    public float handbagDuration = 0.8f;

    [Header("Alman Special")]
    public int bottleCount = 3;

    [Header("Gymbro Special")]
    public float proteinDuration = 5f;
    public int damageMultiplier = 2;

    private bool proteinActive = false;
    private int originalDamage;

    void Awake()
    {
        soldier = GetComponent<Soldier>();
    }

    void Start()
    {
        if (soldier.characterData != null)
        {
            StartCooldown();
        }
    }

    void Update()
    {
        if (!soldier.enabled ||
            soldier.currentHealth <= 0 ||
            soldier.ownCastle == null ||
            soldier.characterData == null)
            return;

        if (isUsingSpecial ||
            proteinActive ||
            Time.time < nextSpecialTime)
            return;

        switch (soldier.characterData.characterType)
        {
            case CharacterType.Talahon:
                TryTalahonSpecial();
                break;

            case CharacterType.Talahona:
                TryTalahonaSpecial();
                break;

            case CharacterType.Alman:
                TryAlmanSpecial();
                break;

            case CharacterType.Gymbro:
                TryGymbroSpecial();
                break;

            case CharacterType.DubaiInfluencerin:
                TryDubaiSpecial();
                break;
        }
    }

    // ==============================
    // TALAHON
    // ==============================

    private void TryTalahonSpecial()
    {
        Soldier target = soldier.targetSoldier;
        Castle castle = soldier.targetCastle;

        if (IsEnemySoldier(target))
        {
            if (!IsValidSoldier(target))
                return;

            castle = null;
        }
        else
        {
            target = null;

            if (!IsValidCastle(castle))
                return;
        }

        StartCooldown();

        StartCoroutine(TalahonCombo(target, castle));
    }

    private IEnumerator TalahonCombo(
        Soldier target, Castle castle)
    {
        isUsingSpecial = true;

        Debug.Log("Talahon: Schattenbox-Kombo!");

        for (int i = 0; i < 5; i++)
        {
            if (!soldier.enabled ||
                soldier.currentHealth <= 0)
                break;

            if (IsValidSoldier(target))
            {
                target.TakeDamage(20);
            }
            else if (IsValidCastle(castle))
            {
                castle.TakeDamage(20);
            }
            else
            {
                break;
            }

            if (i < 4)
            {
                yield return new WaitForSeconds(0.2f);
            }
        }

        isUsingSpecial = false;
    }

    // ==============================
    // TALAHONA
    // ==============================

    private void TryTalahonaSpecial()
    {
        if (!HasEnemiesInRadius())
            return;

        StartCooldown();

        StartCoroutine(TalahonaSpin());
    }

    private bool HasEnemiesInRadius()
    {
        Soldier[] soldiers = FindObjectsOfType<Soldier>();

        foreach (Soldier enemy in soldiers)
        {
            if (IsEnemySoldier(enemy) &&
                GetDistance(enemy.transform) <= handbagRadius)
            {
                return true;
            }
        }

        Castle[] castles = FindObjectsOfType<Castle>();

        foreach (Castle castle in castles)
        {
            if (IsEnemyCastle(castle) &&
                GetDistance(castle.transform) <= handbagRadius)
            {
                return true;
            }
        }

        return false;
    }

    private IEnumerator TalahonaSpin()
    {
        isUsingSpecial = true;

        Debug.Log("Talahona: Handtaschen-Wirbel!");

        Soldier[] soldiers = FindObjectsOfType<Soldier>();
        Castle[] castles = FindObjectsOfType<Castle>();

        foreach (Soldier enemy in soldiers)
        {
            if (IsEnemySoldier(enemy) &&
                GetDistance(enemy.transform) <= handbagRadius)
            {
                enemy.TakeDamage(handbagDamage);
            }
        }

        foreach (Castle castle in castles)
        {
            if (IsEnemyCastle(castle) &&
                GetDistance(castle.transform) <= handbagRadius)
            {
                castle.TakeDamage(handbagDamage);
            }
        }

        float elapsedTime = 0f;
        Quaternion startRotation = transform.rotation;

        while (elapsedTime < handbagDuration)
        {
            if (!soldier.enabled ||
                soldier.currentHealth <= 0)
                break;

            elapsedTime += Time.deltaTime;

            float progress = Mathf.Clamp01(
                elapsedTime / handbagDuration);

            transform.rotation = startRotation *
                Quaternion.Euler(
                    0f, progress * 720f, 0f);

            yield return null;
        }

        transform.rotation = startRotation;

        isUsingSpecial = false;
    }

    // ==============================
    // ALMAN
    // ==============================

    private void TryAlmanSpecial()
    {
        if (soldier.characterData.projectilePrefab == null)
            return;

        if (bottleCount <= 0)
            return;

        List<Soldier> enemies = new List<Soldier>();

        Soldier[] soldiers = FindObjectsOfType<Soldier>();

        foreach (Soldier enemy in soldiers)
        {
            if (IsValidSoldier(enemy))
            {
                enemies.Add(enemy);
            }
        }

        enemies.Sort((a, b) =>
            GetDistance(a.transform).CompareTo(
                GetDistance(b.transform)));

        Castle castleTarget = soldier.targetCastle;

        if (enemies.Count == 0 &&
            !IsValidCastle(castleTarget))
            return;

        StartCooldown();

        Debug.Log("Alman: Dreifacher Bierflaschenwurf!");

        for (int i = 0; i < bottleCount; i++)
        {
            Soldier target = null;
            Castle castle = null;

            if (enemies.Count > 0)
            {
                target = enemies[i % enemies.Count];
            }
            else
            {
                castle = castleTarget;
            }

            Vector3 spawnPosition =
                transform.position +
                Vector3.up * 0.8f +
                transform.right *
                ((i - (bottleCount - 1) / 2f) * 0.3f);

            Projectile projectile = Instantiate(
                soldier.characterData.projectilePrefab,
                spawnPosition,
                Quaternion.identity);

            projectile.Initialize(
                target,
                castle,
                soldier.attackDamage);
        }
    }

    // ==============================
    // GYMBRO
    // ==============================

    private void TryGymbroSpecial()
    {
        bool hasEnemy =
            IsValidSoldier(soldier.targetSoldier) ||
            IsValidCastle(soldier.targetCastle);

        if (!hasEnemy)
            return;

        StartCooldown();

        StartCoroutine(ProteinBoost());
    }

    private IEnumerator ProteinBoost()
    {
        proteinActive = true;

        originalDamage = soldier.attackDamage;

        soldier.attackDamage =
            originalDamage * damageMultiplier;

        Debug.Log("Gymbro: Proteinshake aktiviert! " +
            soldier.attackDamage + " Schaden");

        float elapsedTime = 0f;

        while (elapsedTime < proteinDuration)
        {
            if (!soldier.enabled ||
                soldier.currentHealth <= 0)
                break;

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        ResetProteinBoost();
    }

    private void ResetProteinBoost()
    {
        if (!proteinActive)
            return;

        soldier.attackDamage = originalDamage;

        proteinActive = false;

        Debug.Log("Gymbro: Proteinshake beendet!");
    }

    // ==============================
    // DUBAI-INFLUENCERIN
    // ==============================

    private void TryDubaiSpecial()
    {
        if (soldier.characterData.carPrefab == null)
            return;

        Transform target = null;

        if (IsValidSoldier(soldier.targetSoldier))
        {
            target = soldier.targetSoldier.transform;
        }
        else if (IsValidCastle(soldier.targetCastle))
        {
            target = soldier.targetCastle.transform;
        }

        if (target == null)
            return;

        // Fahrtrichtung berechnen
        Vector3 direction =
            target.position - transform.position;

        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
            return;

        direction.Normalize();

        StartCooldown();

        // Auto vor der Influencerin erzeugen
        Vector3 spawnPosition =
            transform.position + direction * 1.5f;

        spawnPosition.y = 0.5f;

        CarAttack car = Instantiate(
            soldier.characterData.carPrefab,
            spawnPosition,
            Quaternion.LookRotation(direction));

        car.Initialize(
            soldier.ownCastle,
            direction);

        Debug.Log(
            "Dubai-Influencerin: Lamborghini aktiviert!");
    }

    // ==============================
    // GEMEINSAME METHODEN
    // ==============================

    private void StartCooldown()
    {
        nextSpecialTime = Time.time +
            soldier.characterData.specialCooldown;
    }

    private bool IsEnemySoldier(Soldier target)
    {
        return target != null &&
               target != soldier &&
               target.gameObject.activeInHierarchy &&
               target.currentHealth > 0 &&
               target.ownCastle != null &&
               target.ownCastle != soldier.ownCastle;
    }

    private bool IsEnemyCastle(Castle castle)
    {
        return castle != null &&
               castle.gameObject.activeInHierarchy &&
               castle.currentHealth > 0 &&
               castle != soldier.ownCastle;
    }

    private bool IsValidSoldier(Soldier target)
    {
        return IsEnemySoldier(target) &&
               IsInRange(target.transform);
    }

    private bool IsValidCastle(Castle castle)
    {
        return IsEnemyCastle(castle) &&
               IsInRange(castle.transform);
    }

    private bool IsInRange(Transform target)
    {
        return GetDistance(target) <= soldier.attackRange;
    }

    private float GetDistance(Transform target)
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

        return Vector3.Distance(
            transform.position, targetPosition);
    }

    void OnDisable()
    {
        StopAllCoroutines();

        ResetProteinBoost();

        isUsingSpecial = false;
    }
}
