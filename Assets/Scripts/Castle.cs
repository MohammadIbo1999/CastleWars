
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Castle : MonoBehaviour
{
    public Country country;
    public TMP_Text countryLabel;
    public SpriteRenderer countryFlag;
    public Slider healthBar;
    public TMP_Text healthText;

    public int maxHealth = 10000;
    public int currentHealth;

    void Start()
    {
        ResetCastle();
    }

    // Land zuweisen
    public void SetCountry(Country newCountry)
    {
        country = newCountry;
        countryLabel.text = country.countryName;
        countryFlag.sprite = country.flag;
    }

    // Schaden erhalten
    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0)
            return;

        currentHealth = Mathf.Max(
            0, currentHealth - damage);

        UpdateHealthUI();

        if (currentHealth == 0)
        {
            DestroyCastle();
        }
    }

    // Lebensanzeige aktualisieren
    private void UpdateHealthUI()
    {
        healthBar.value = currentHealth;

        healthText.text =
            currentHealth + " / " + maxHealth + " HP";
    }

    // Burg zerstören
    private void DestroyCastle()
    {
        Debug.Log(gameObject.name + " wurde zerstört!");
        gameObject.SetActive(false);
    }

    // Burg für die nächste Runde zurücksetzen
    public void ResetCastle()
    {
        currentHealth = maxHealth;

        gameObject.SetActive(true);

        healthBar.maxValue = maxHealth;

        UpdateHealthUI();
    }
}
