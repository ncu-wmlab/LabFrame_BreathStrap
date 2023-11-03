using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LabFrame2023;
using System.Linq;
using UnityEngine.Events;

public class BreathStrapManager : LabSingleton<BreathStrapManager>, IManager
{
    protected float _breathValue = 0;
    public event UnityAction<float> OnBreathValueUpdated;
    /* -------------------------------------------------------------------------- */    
    public void ManagerInit()
    {
        
    }

    public IEnumerator ManagerDispose()
    {
        Disconnect();
        _breathValue = 0;
        yield break;
    }

    /* -------------------------------------------------------------------------- */

    /// <summary>
    /// 開始連接，請不要持續重複呼叫
    /// </summary>
    public void Connect()
    {
        StopAllCoroutines();
        StartCoroutine(DoReceiveAsync());
    }

    IEnumerator DoReceiveAsync()
    {
        // connect
        Debug.Log("[BreathStrapManager] Start Connect");
#if UNITY_STANDALONE_WIN
        while(!BluetoothManager.Instance.IsConnected())
        {
            BluetoothManager.Instance.ConnectCOM("3");
            yield return new WaitForSeconds(0.1f);
        }
        
#else
        BluetoothManager.Instance.CheckPermission();
        BluetoothManager.Instance.StartDiscovery();

        while(!BluetoothManager.Instance.IsConnected())
        {
            var devices = BluetoothManager.Instance.GetAvailableDevices();
            if(devices.Any(d => d.name == "HC-05"|| d.name == "HC-06"))
            {
                BluetoothManager.Instance.Connect(devices.First(d => d.name == "HC-05"|| d.name == "HC-06").mac);
            }
            yield return new WaitForSeconds(0.1f);
        }
#endif
        Debug.Log("[BreathStrapManager] Connected to "+ BluetoothManager.Instance.GetConnectedDevice());

        // read data
        while(BluetoothManager.Instance.IsConnected())
        {
            string data = BluetoothManager.Instance.ReadLine();
            if(float.TryParse(data, out float breathValue))  // 呼吸綁帶傳遞格式： "Pressure\n\n{VALUE}"，這裡直接 parse 那個 VALUE
            {
                if(breathValue < 900 || breathValue > 1100)
                {
                    Debug.Log("[BreathStrapManager] Illegal breath value " + breathValue);
                    continue;
                }
                _breathValue = breathValue;
                OnBreathValueUpdated?.Invoke(breathValue);
            }
            yield return null;
        }
    }

    public void Disconnect()
    {
#if UNITY_ANDROID
        BluetoothManager.Instance.Stop();
#endif
    }

    /// <summary>
    /// 獲取最後收到的呼吸數值
    /// </summary>
    /// <returns></returns>
    public float GetBreathValue()
    {
        return _breathValue;
    }

    /// <summary>
    /// 是否已連接
    /// </summary>
    /// <returns></returns>
    public bool IsConnected()
    {
        return BluetoothManager.Instance.IsConnected();
    }

    /* -------------------------------------------------------------------------- */
}