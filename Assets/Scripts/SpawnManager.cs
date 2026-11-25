
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public Soldier soldierPrefab;
    public Castle[] castles;

    // Automatische Soldatenerzeugung
    public bool autoSpawn = true;
    public float spawnInterval = 5f;
    public int maxSoldiersPerTeam = 100;

    private float spawnTimer = 0f;

    void Update()
    {
        // Automatische Erzeugung
        if (autoSpawn)
        {
            spawnTimer += Time.deltaTime;

            if (spawnTimer >= spawnInterval)
            {
                SpawnAllTeams();
                spawnTimer = 0f;
            }
        }

        // Manuelle Erzeugung zum Testen
        if (Input.GetKeyDown(KeyCode.Alpha1))
            SpawnSoldier(castles[0]);

        if (Input.GetKeyDown(KeyCode.Alpha2))
            SpawnSoldier(castles[1]);

        if (Input.GetKeyDown(KeyCode.Alpha3))
            SpawnSoldier(castles[2]);

        if (Input.GetKeyDown(KeyCode.Alpha4))
            SpawnSoldier(castles[3]);
    }

    // Soldaten für alle aktiven Teams erzeugen
    private void SpawnAllTeams()
    {
        foreach (Castle castle in castles)
        {
            SpawnSoldier(castle);
        }
    }

    // Einzelnen Soldaten erzeugen
    public void SpawnSoldier(Castle castle)
    {
        if (castle == null ||
            !castle.gameObject.activeInHierarchy)
            return;

        if (CountSoldiers(castle) >= maxSoldiersPerTeam)
            return;

        Transform spawnPoint =
            castle.transform.Find("SpawnPoint");

        if (spawnPoint == null)
        {
            Debug.LogWarning("SpawnPoint fehlt!");
            return;
        }

        Soldier soldier = Instantiate(
            soldierPrefab,
            spawnPoint.position,
            Quaternion.identity
        );

        // Burg zuweisen
        soldier.ownCastle = castle;
        soldier.targetCastle = null;

        // Teamfarbe übernehmen
        soldier.GetComponent<Renderer>().sharedMaterial =
            castle.GetComponent<Renderer>().sharedMaterial;
    }

    // Lebende Soldaten eines Teams zählen
    private int CountSoldiers(Castle castle)
    {
        int count = 0;

        Soldier[] soldiers = FindObjectsOfType<Soldier>();

        foreach (Soldier soldier in soldiers)
        {
            if (soldier.ownCastle == castle &&
                soldier.currentHealth > 0)
            {
                count++;
            }
        }

        return count;
    }

    public void ResetSpawnTimer()
    {
        spawnTimer = 0f;
    }
}
