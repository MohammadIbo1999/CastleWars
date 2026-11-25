
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
        currentHealth = maxHealth;
        healthBar.maxValue = maxHealth;

        UpdateHealthUI();
    }

    public void SetCountry(Country newCountry)
    {
        country = newCountry;
        countryLabel.text = country.countryName;
        countryFlag.sprite = country.flag;
    }

    public void TakeDamage(int damage)
    {
        currentHealth = Mathf.Max(0, currentHealth - damage);

        UpdateHealthUI();

        if (currentHealth == 0)
        {
            DestroyCastle();
        }
    }

    private void UpdateHealthUI()
    {
        healthBar.value = currentHealth;
        healthText.text = currentHealth + " / " + maxHealth + " HP";
    }

    private void DestroyCastle()
    {
        Debug.Log(gameObject.name + " wurde zerstört!");
        gameObject.SetActive(false);
    }
}
