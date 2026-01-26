using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Crowsalina;
using UnityEngine.UI;
using TMPro;

public class ProvinceStats : MonoBehaviour
{
    public Province provinceData;
    public bool hasArmy, hasFleet;
    public int controllingPower;
    public Color iconColour;
    public Image army, fleet, supply;
    public TextMeshProUGUI nameText;
    void Start()
    {
        switch (provinceData.startingPower)
        {
            case 0:
                iconColour = Color.white;
                break;
            case 1:
                iconColour = Color.blue;
                break;
            case 2:
                iconColour = Color.cyan;
                break;
            case 3:
                iconColour = Color.black;
                break;
            case 4:
                iconColour = Color.green;
                break;
            case 5:
                iconColour = Color.red;
                break;
            case 6:
                iconColour = Color.magenta;
                break;
            case 7:
                iconColour = Color.yellow;
                break;
        }
        UpdateProvinceStats();
    }

    public void UpdateProvinceStats()
    {
        nameText.text = provinceData.provinceName;
        switch (controllingPower)
        {
            case 0:
                iconColour = Color.white;
                break;
            case 1:
                iconColour = Color.blue;
                break;
            case 2:
                iconColour = Color.cyan;
                break;
            case 3:
                iconColour = Color.black;
                break;
            case 4:
                iconColour = Color.green;
                break;
            case 5:
                iconColour = Color.red;
                break;
            case 6:
                iconColour = Color.magenta;
                break;
            case 7:
                iconColour = Color.yellow;
                break;
        }
        army.color = iconColour;
        fleet.color = iconColour;
        supply.color = iconColour;
        if (provinceData.isSupply)
        {
            supply.gameObject.SetActive(true);
        }
        if (hasArmy)
        {
            army.gameObject.SetActive(true);
        }
        if (hasFleet)
        {
            fleet.gameObject.SetActive(true);
        }
    }
}
