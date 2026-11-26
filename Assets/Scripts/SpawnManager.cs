
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("References")]
    public Soldier soldierPrefab;
    public Castle[] castles;

    [Header("Characters")]
    public CharacterData[] characters;
    public int selectedCharacterIndex = 0;

    [Header("Spawn Settings")]
    public bool autoSpawn = true;
    public float spawnInterval = 5f;
    public int maxSoldiersPerTeam = 100;

    private float spawnTimer = 0f;

    void Update()
    {
        HandleCharacterSelection();

        if (autoSpawn)
        {
            spawnTimer += Time.deltaTime;

            if (spawnTimer >= spawnInterval)
            {
                SpawnAllTeams();
                spawnTimer = 0f;
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
            SpawnSoldier(castles[0]);

        if (Input.GetKeyDown(KeyCode.Alpha2))
            SpawnSoldier(castles[1]);

        if (Input.GetKeyDown(KeyCode.Alpha3))
            SpawnSoldier(castles[2]);

        if (Input.GetKeyDown(KeyCode.Alpha4))
            SpawnSoldier(castles[3]);
    }

    // Charakter auswählen
    private void HandleCharacterSelection()
    {
        if (Input.GetKeyDown(KeyCode.F1))
            SelectCharacter(0);

        if (Input.GetKeyDown(KeyCode.F2))
            SelectCharacter(1);

        if (Input.GetKeyDown(KeyCode.F3))
            SelectCharacter(2);

        if (Input.GetKeyDown(KeyCode.F4))
            SelectCharacter(3);

        if (Input.GetKeyDown(KeyCode.F5))
            SelectCharacter(4);
    }

    private void SelectCharacter(int index)
    {
        if (index < 0 || index >= characters.Length)
            return;

        if (characters[index] == null)
            return;

        selectedCharacterIndex = index;

        Debug.Log("Selected Character: " +
            characters[index].characterName);
    }

    // Automatische Soldatenerzeugung
    private void SpawnAllTeams()
    {
        foreach (Castle castle in castles)
        {
            SpawnSoldier(castle, characters[0]);
        }
    }

    // Ausgewählten Charakter erzeugen
    public void SpawnSoldier(Castle castle)
    {
        if (selectedCharacterIndex < 0 ||
            selectedCharacterIndex >= characters.Length)
            return;

        SpawnSoldier(castle,
            characters[selectedCharacterIndex]);
    }

    // Bestimmten Charakter erzeugen
    public void SpawnSoldier(
        Castle castle, CharacterData character)
    {
        if (castle == null ||
            !castle.gameObject.activeInHierarchy ||
            character == null ||
            !isActiveAndEnabled)
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
            Quaternion.identity);

        soldier.ownCastle = castle;
        soldier.targetCastle = null;

        // Charakterdaten übernehmen
        soldier.characterData = character;
        soldier.InitializeCharacter();

        // Teamfarbe übernehmen
        soldier.GetComponent<Renderer>().sharedMaterial =
            castle.GetComponent<Renderer>().sharedMaterial;

        // Namen in der Hierarchy setzen
        soldier.gameObject.name =
            character.characterName + "_" +
            castle.country.countryName;
    }

    // Soldaten eines Teams zählen
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

    // Spawn-Timer zurücksetzen
    public void ResetSpawnTimer()
    {
        spawnTimer = 0f;
    }
}
