using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ShipLap : MonoBehaviour
{
    public struct LapTime
    {
        public float TotalSeconds;
        public int Seconds;
        public int Minutes;
        public int Milliseconds;
        public string TotalLapTime;
    }

    public LapTime currentLapTime;
  
    public List<LapTime> lapTimes;
    private float _lapTimeSum;

    public int lapNumber;
    public int checkpointIndex;


    

    
    void Start()
    {
        lapNumber = 1;
        checkpointIndex = 0;

        lapTimes = new List<LapTime>();

        currentLapTime.TotalSeconds = 0;
        currentLapTime.Seconds = 0;
        currentLapTime.Minutes = 0;
        currentLapTime.Milliseconds = 0;
        currentLapTime.TotalLapTime = "";

        _lapTimeSum = 0;
    }

    private void FixedUpdate()
    {
        SetCurrentLapTime();
    }

    private void SetCurrentLapTime()
    {
        currentLapTime.TotalSeconds = Time.time - _lapTimeSum;
        currentLapTime.Seconds = (int)currentLapTime.TotalSeconds % 60;
        currentLapTime.Minutes = (int)currentLapTime.TotalSeconds / 60;
        currentLapTime.Milliseconds = (int)(100 * (currentLapTime.TotalSeconds % 1));
        currentLapTime.TotalLapTime = (string)(currentLapTime.Minutes + ":" + currentLapTime.Seconds + ":" + currentLapTime.Milliseconds);
    }


    public void OnLapComplete()
    {
        lapTimes.Add(currentLapTime);
        _lapTimeSum += currentLapTime.TotalSeconds;
    }
}
