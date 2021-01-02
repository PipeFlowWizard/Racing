using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LapTimeGetter : MonoBehaviour
{
    [SerializeField]
    private LapCounter lapCounter;
    [SerializeField]
    private TextMeshProUGUI _currentLapString;
    [SerializeField]
    private TextMeshProUGUI _lapTime;

    private RaceHandler raceManager;
    
    void Start()
    {
        lapCounter = FindObjectOfType<LapCounter>();
        raceManager = FindObjectOfType<RaceHandler>();
        UpdateCurrentLap();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _lapTime.text = lapCounter.currentLapTime.TotalLapTime;
    }

    public void UpdateCurrentLap()
    {
        _currentLapString.text = "Lap: " + lapCounter.currentLap + "/" + raceManager.totalLaps;
    }
}
