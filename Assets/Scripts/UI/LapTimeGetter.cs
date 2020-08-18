using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LapTimeGetter : MonoBehaviour
{
    [SerializeField]
    private ShipLap shipLap;

    private TextMeshProUGUI _text;
    void Start()
    {
        shipLap = FindObjectOfType<ShipLap>();
        _text = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _text.text = shipLap.currentLapTime.TotalLapTime;
    }
}
