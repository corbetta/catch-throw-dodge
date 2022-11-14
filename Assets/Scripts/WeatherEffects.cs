using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherEffects : MonoBehaviour
{
    [SerializeField] private GameObject[] weatherObjects;

    public void SetWeather(Weather weather) {
        //find the count of the weather object we will keep
        int elementToKeep = -1;
        switch (weather) {
            case Weather.Cloudy:
                elementToKeep = 0;
                break;
            case Weather.Raining:
                elementToKeep = 1;
                break;
            case Weather.Snowing:
                elementToKeep = 2;
                break;
            case Weather.Sunny:
                elementToKeep = -1; //if it's sunny, remove all elements
                break;
        }

        //instantiate through the weather objects and turn off all weather objects that shouldn't be kept
        for (int i = 0; i < weatherObjects.Length; i++) {
            if (i == elementToKeep) {
                weatherObjects[i].SetActive(true);
            }
            else {
                weatherObjects[i].SetActive(false);
            }
        }
    }
}
