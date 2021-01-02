using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LaptimeDisplay : MonoBehaviour
{
    public TextMeshProUGUI laptimes;
    private LapCounter _lapCounter;

    private void Start()
    {
        _lapCounter = FindObjectOfType<LapCounter>();
    }

   public void DisplayText()
    {
        foreach (var laptime in _lapCounter.lapTimes)
        {
            laptimes.text += laptime.TotalLapTime + "\n";
        }
    }
}
