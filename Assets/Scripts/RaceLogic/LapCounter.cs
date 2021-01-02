using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class LapCounter : MonoBehaviour
{
    //Structs
    public struct LapTime
     {
         public float TotalSeconds;
         public int Seconds;
         public int Minutes;
         public int Milliseconds;
         public string TotalLapTime;
     }
    
    //******************************************************************************************************************
    //Dependencies
    
    public RaceHandler raceHandler;
    [SerializeField] private LapTimeGetter raceUI;
    
    //******************************************************************************************************************
    // Data

    public List<LapTime> lapTimes;

    //******************************************************************************************************************
    //State
    
    public int currentLap = 1;
    public LapTime currentLapTime;
    public int checkpointIndex;
    private float _raceTime;


    public event System.Action OnRaceEnd;

    
    void Start()
    {
        currentLap = 1;
        checkpointIndex = 0;

        lapTimes = new List<LapTime>();

        currentLapTime.TotalSeconds = 0;
        currentLapTime.Seconds = 0;
        currentLapTime.Minutes = 0;
        currentLapTime.Milliseconds = 0;
        currentLapTime.TotalLapTime = "";

        _raceTime = 0;

        raceHandler = FindObjectOfType<RaceHandler>();
    }
    private void FixedUpdate()
    {
        SetCurrentLapTime();
    }

    private void SetCurrentLapTime()
    {
        currentLapTime.TotalSeconds = Time.time - _raceTime;
        currentLapTime.Seconds = (int)currentLapTime.TotalSeconds % 60;
        currentLapTime.Minutes = (int)currentLapTime.TotalSeconds / 60;
        currentLapTime.Milliseconds = (int)(100 * (currentLapTime.TotalSeconds % 1));
        currentLapTime.TotalLapTime = (string)(currentLapTime.Minutes + ":" + currentLapTime.Seconds + ":" + currentLapTime.Milliseconds);
    }


    public void OnLapComplete()
    {
        lapTimes.Add(currentLapTime);
        _raceTime += currentLapTime.TotalSeconds;
        raceUI.UpdateCurrentLap();
        if (currentLap > raceHandler.totalLaps)
        {
            OnRaceEnd?.Invoke();
        }
    }

    
}
