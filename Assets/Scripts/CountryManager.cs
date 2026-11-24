
using System.Collections.Generic;
using UnityEngine;

public class CountryManager : MonoBehaviour
{
    public List<Country> availableCountries = new List<Country>();
    public Castle[] castles;

    void Start()
    {
        AssignCountries();
    }

    public void AssignCountries()
    {
        List<Country> remainingCountries =
            new List<Country>(availableCountries);

        foreach (Castle castle in castles)
        {
            if (remainingCountries.Count == 0)
                break;

            int index = Random.Range(0, remainingCountries.Count);

            Country country = remainingCountries[index];

            castle.SetCountry(country);

            remainingCountries.RemoveAt(index);

            Debug.Log(castle.name + ": " + country.countryName);
        }
    }
}
