using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastleTest : MonoBehaviour
{
    public Castle castle;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            castle.TakeDamage(1000);
            Debug.Log("Remaining HP: " + castle.currentHealth);
        }
    }
}
