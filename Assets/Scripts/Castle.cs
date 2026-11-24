using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Castle : MonoBehaviour
{
    public int maxHealth = 10000;
    public int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth = Mathf.Max(0, currentHealth - damage);

        if (currentHealth == 0)
        {
            DestroyCastle();
        }
    }

    private void DestroyCastle()
    {
        Debug.Log(gameObject.name + " wurde zerstört!");
        gameObject.SetActive(false);
    }
}