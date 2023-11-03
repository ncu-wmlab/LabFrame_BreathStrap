using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BreathStrapDemo : MonoBehaviour
{
    [SerializeField] Text _contentText;

    int _count = 0;
    double _total = 0;
    
    /* -------------------------------------------------------------------------- */

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        // calc baseline
        double baseline = _total > 0 ? _total / _count : -1;

        // update ui
        _contentText.text = $"<b>IsConnected:</b> {BreathStrapManager.Instance.IsConnected()}\n"+
                            $"<b>Breath Value:</b> {BreathStrapManager.Instance.GetBreathValue().ToString("0.00")}\n"+
                            $"<b>Count:</b> {_count}\n"+
                            $"<b>Baseline:</b> {baseline.ToString("0.00")}\n";
    }

    /* -------------------------------------------------------------------------- */

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        BreathStrapManager.Instance.OnBreathValueUpdated += OnBreathValueUpdated;        
    }

    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    void OnDisable()
    {
        BreathStrapManager.Instance.OnBreathValueUpdated -= OnBreathValueUpdated;
    }


    private void OnBreathValueUpdated(float breathValue)
    {        
        _count++;
        _total += breathValue;
    }

    /* -------------------------------------------------------------------------- */
    
    public void Connect()
    {
        BreathStrapManager.Instance.Connect();
    }

    public void Disconnect()
    {
        BreathStrapManager.Instance.Disconnect();
    }
}
