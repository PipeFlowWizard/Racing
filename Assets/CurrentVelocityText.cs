using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CurrentVelocityText : MonoBehaviour
{
    [SerializeField]
    private ShipMovement velocity;
    
    [SerializeField]
    private TextMeshProUGUI _text;

    // Update is called once per frame
    void Update()
    {
        var currentVelocity = ((int)velocity.CurrentVelocity).ToString();
        _text.text = currentVelocity;
    }
}
