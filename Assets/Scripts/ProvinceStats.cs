using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Crowsalina;
using UnityEngine.UI;
using TMPro;

public class ProvinceStats : MonoBehaviour
{
    public Province provinceData;
    public string provinceName;
    public bool hasArmy, hasFleet;
    public int controllingPower;
    void Start()
    {
        UpdateProvinceStats();
    }

    public void UpdateProvinceStats()
    {
        provinceName = provinceData.provinceName;
    }
}
