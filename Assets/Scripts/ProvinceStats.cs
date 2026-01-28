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
    public Color iconColour, buttonColour;
    public Image army, fleet, supply, buttonImage;
    public TextMeshProUGUI nameText;
    void Start()
    {
        controllingPower = provinceData.startingPower;
        UpdateProvinceStats();
    }
    public void UpdateProvinceStats()
    {
        nameText.text = provinceData.provinceName;
        switch (controllingPower)
        {
            case 0:
                if (provinceData.isMaritime)
                {
                    buttonColour = new Color(0.803f, 0.941f, 0.962f, 1);
                }
                else
                {
                    iconColour = Color.white;
                    buttonColour = new Color(0.76f, 0.76f, 0.76f, 1);
                }
                break;
            case 1:
                iconColour = Color.blue;
                buttonColour = new Color(0.463f, 0.435f, 0.698f, 1);
                break;
            case 2:
                iconColour = Color.cyan;
                buttonColour = new Color(0.549f, 0.780f, 0.925f, 1);
                break;
            case 3:
                iconColour = Color.black;
                buttonColour = new Color(0.43f, 0.43f, 0.43f, 1);
                break;
            case 4:
                iconColour = Color.green;
                buttonColour = new Color(0.42f, 0.573f, 0.404f, 1);
                break;
            case 5:
                iconColour = Color.red;
                buttonColour = new Color(0.678f, 0.295f, 0.223f, 1);
                break;
            case 6:
                iconColour = Color.magenta;
                buttonColour = new Color(0.678f, 0.223f, 0.598f, 1);
                break;
            case 7:
                iconColour = Color.yellow;
                buttonColour = new Color(1, 0.969f, 0.518f, 1);
                break;
        }

        buttonImage.color = buttonColour;
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
