
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RoundManager : MonoBehaviour
{
    public Castle[] castles;
    public SpawnManager spawnManager;
    public CountryManager countryManager;

    // Rundeneinstellungen
    public float roundDuration = 300f;
    public float remainingTime;
    public bool roundFinished = false;
    public int roundNumber = 1;

    // Neustart
    public float restartDelay = 15f;
    private float restartTimer;

    // Countdown
    public TMP_Text timerText;
    public TMP_Text restartText;

    // Gewinneranzeige
    public GameObject winnerPanel;
    public TMP_Text winnerTitle;
    public TMP_Text winnerName;
    public TMP_Text winnerHealth;
    public Image winnerFlag;

    void Start()
    {
        remainingTime = roundDuration;
        winnerPanel.SetActive(false);

        UpdateTimerUI();
    }

    void Update()
    {
        if (roundFinished)
        {
            UpdateRestartTimer();
            return;
        }

        remainingTime = Mathf.Max(
            0f, remainingTime - Time.deltaTime);

        UpdateTimerUI();
        CheckWinner();

        if (!roundFinished && remainingTime <= 0)
        {
            CheckWinnerByHealth();
        }
    }

    private void UpdateTimerUI()
    {
        int totalSeconds = Mathf.CeilToInt(remainingTime);

        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;

        timerText.text = $"{minutes:00}:{seconds:00}";

        timerText.color = remainingTime <= 30
            ? Color.red
            : Color.white;
    }

    private void CheckWinner()
    {
        int aliveCastles = 0;
        Castle winner = null;

        foreach (Castle castle in castles)
        {
            if (castle != null &&
                castle.gameObject.activeInHierarchy &&
                castle.currentHealth > 0)
            {
                aliveCastles++;
                winner = castle;
            }
        }

        if (aliveCastles == 1)
        {
            EndRound(winner);
        }
        else if (aliveCastles == 0)
        {
            EndRound(null);
        }
    }

    private void CheckWinnerByHealth()
    {
        Castle winner = null;
        int highestHealth = 0;
        bool draw = false;

        foreach (Castle castle in castles)
        {
            if (castle == null ||
                !castle.gameObject.activeInHierarchy ||
                castle.currentHealth <= 0)
                continue;

            if (castle.currentHealth > highestHealth)
            {
                highestHealth = castle.currentHealth;
                winner = castle;
                draw = false;
            }
            else if (castle.currentHealth == highestHealth)
            {
                draw = true;
            }
        }

        EndRound(draw ? null : winner);
    }

    // Runde beenden
    private void EndRound(Castle winner)
    {
        roundFinished = true;
        restartTimer = restartDelay;

        spawnManager.autoSpawn = false;
        spawnManager.enabled = false;

        // Soldaten und Spezialfähigkeiten stoppen
        Soldier[] soldiers = FindObjectsOfType<Soldier>();

        foreach (Soldier soldier in soldiers)
        {
            soldier.enabled = false;

            SpecialAbility ability =
                soldier.GetComponent<SpecialAbility>();

            if (ability != null)
            {
                ability.enabled = false;
            }
        }

        ClearAttackObjects();

        ShowWinner(winner);
        UpdateRestartText();
    }

    // Alle Angriffsobjekte entfernen
    private void ClearAttackObjects()
    {
        // Bierflaschen
        Projectile[] projectiles =
            FindObjectsOfType<Projectile>();

        foreach (Projectile projectile in projectiles)
        {
            projectile.gameObject.SetActive(false);
            Destroy(projectile.gameObject);
        }

        // Kamerablitze
        CameraFlash[] flashes =
            FindObjectsOfType<CameraFlash>();

        foreach (CameraFlash flash in flashes)
        {
            flash.gameObject.SetActive(false);
            Destroy(flash.gameObject);
        }

        // Lamborghinis
        CarAttack[] cars =
            FindObjectsOfType<CarAttack>();

        foreach (CarAttack car in cars)
        {
            car.gameObject.SetActive(false);
            Destroy(car.gameObject);
        }
    }

    private void ShowWinner(Castle winner)
    {
        if (winner != null)
        {
            winnerTitle.text = "WINNER";
            winnerName.text = winner.country.countryName;

            winnerHealth.text =
                winner.currentHealth + " / " +
                winner.maxHealth + " HP";

            winnerFlag.sprite = winner.country.flag;
            winnerFlag.gameObject.SetActive(true);

            Debug.Log("Winner: " +
                winner.country.countryName);
        }
        else
        {
            winnerTitle.text = "DRAW";
            winnerName.text = "No Winner";
            winnerHealth.text = "";
            winnerFlag.gameObject.SetActive(false);

            Debug.Log("Draw!");
        }

        winnerPanel.SetActive(true);
    }

    private void UpdateRestartTimer()
    {
        restartTimer = Mathf.Max(
            0f, restartTimer - Time.deltaTime);

        UpdateRestartText();

        if (restartTimer <= 0)
        {
            RestartRound();
        }
    }

    private void UpdateRestartText()
    {
        restartText.text = "Next round in " +
            Mathf.CeilToInt(restartTimer) + "s";
    }

    private void RestartRound()
    {
        // Sicherheitshalber Angriffsobjekte entfernen
        ClearAttackObjects();

        // Alte Soldaten entfernen
        Soldier[] soldiers = FindObjectsOfType<Soldier>();

        foreach (Soldier soldier in soldiers)
        {
            soldier.gameObject.SetActive(false);
            Destroy(soldier.gameObject);
        }

        // Burgen wiederherstellen
        foreach (Castle castle in castles)
        {
            castle.ResetCastle();
        }

        // Neue Länder auswählen
        countryManager.AssignCountries();

        winnerPanel.SetActive(false);

        remainingTime = roundDuration;
        roundNumber++;

        spawnManager.ResetSpawnTimer();
        spawnManager.autoSpawn = true;
        spawnManager.enabled = true;

        roundFinished = false;

        UpdateTimerUI();

        Debug.Log("Round " + roundNumber + " started!");
    }
}
