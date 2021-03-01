using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressScale : MonoBehaviour
{
    public bool useEase = false;

    [Range(0, 1)] public float ease = 0.1f;
    
    public float progressPercent = 0;

    public float targetProgressPercent = 0;
    
    [SerializeField]
    private Image _progressBarFill;

    private void Update()
    {
        SetProgressFill();
    }

    void SetProgressFill()
    {
        progressPercent = useEase ?  Mathf.Lerp(targetProgressPercent, progressPercent, ease) : targetProgressPercent;
        _progressBarFill.fillAmount = progressPercent;
    }
}
