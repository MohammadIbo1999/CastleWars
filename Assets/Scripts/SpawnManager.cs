
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public Soldier soldierPrefab;
    public Castle[] castles;

    public void SpawnSoldier(Castle castle)
    {
        if (castle == null ||
            !castle.gameObject.activeInHierarchy)
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

    void Update()
    {
        // Tastatursteuerung zum Testen
        if (Input.GetKeyDown(KeyCode.Alpha1))
            SpawnSoldier(castles[0]);

        if (Input.GetKeyDown(KeyCode.Alpha2))
            SpawnSoldier(castles[1]);

        if (Input.GetKeyDown(KeyCode.Alpha3))
            SpawnSoldier(castles[2]);

        if (Input.GetKeyDown(KeyCode.Alpha4))
            SpawnSoldier(castles[3]);
    }
}
