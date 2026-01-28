using Crowsalina;
using System.Collections;
using UnityEngine;

public class OrderTextParser : MonoBehaviour
{
    private OrderManager orderManager;
    private ProvinceStats currentOriginProvinceStats, currentTargetProvinceStats, currentDestProvinceStats;
    private string currentUnitType;
    private bool hasParsedHolds, hasParsedMoves, hasParsedSupportMoves, hasParsedSupports, hasParsedConvoys;
    private void Start()
    {
        orderManager = FindFirstObjectByType<OrderManager>();
        hasParsedHolds = false;
        hasParsedMoves = false;
        hasParsedSupportMoves = false;
        hasParsedSupports = false;
        hasParsedConvoys = false;
    }
    public void ParseOrders()
    {
        ParseHoldOrders();
        ParseMoveOrders();
        ParseSupportMoveOrders();
        ParseSupportOrders();
        ParseConvoyOrders();
        StartCoroutine(ParsedChecker());
    }
    public void ParseHoldOrders()
    {
        Debug.Log("Hold Orders: " + orderManager.HoldList.Count.ToString());
        for (int i = 0; i < orderManager.HoldList.Count; i++)
        {
            BREAKOUT.Check();
            currentOriginProvinceStats = orderManager.HoldList[i].GetComponent<ProvinceStats>();
            Debug.Log(currentOriginProvinceStats.provinceData.provinceName.ToString());
            if (currentOriginProvinceStats.hasArmy)
            {
                currentUnitType = "A";
            }
            else if (currentOriginProvinceStats.hasFleet)
            {
                currentUnitType = "F";
            }
            else
            {
                continue;
            }
            Debug.Log(currentUnitType +" "+ currentOriginProvinceStats.provinceData.provinceName + " H");
        }
        hasParsedHolds = true;
    }
    public void ParseMoveOrders()
    {
        Debug.Log("Move Orders: " + orderManager.MoveOriginList.Count.ToString());
        for (int i = 0; i < orderManager.MoveOriginList.Count; i++)
        {
            BREAKOUT.Check();
            currentOriginProvinceStats = orderManager.MoveOriginList[i].GetComponent<ProvinceStats>();
            currentDestProvinceStats = orderManager.MoveDestList[i].GetComponent<ProvinceStats>();
            if (currentOriginProvinceStats.hasArmy)
            {
                currentUnitType = "A";
            }
            else if (currentOriginProvinceStats.hasFleet)
            {
                currentUnitType = "F";
            }
            else
            {
                continue;
            }

            Debug.Log(currentUnitType + " " + currentOriginProvinceStats.provinceData.provinceName + " - " + currentDestProvinceStats.provinceData.provinceName);
        }
        hasParsedMoves = true;
    }
    public void ParseSupportMoveOrders()
    {
        Debug.Log("Support Move Orders: " + orderManager.SupportMoveOriginList.Count.ToString());
        for (int i = 0; i < orderManager.SupportMoveOriginList.Count; i++)
        {
            BREAKOUT.Check();
            currentOriginProvinceStats = orderManager.SupportMoveOriginList[i].GetComponent<ProvinceStats>();
            currentTargetProvinceStats = orderManager.SupportMoveTargetList[i].GetComponent<ProvinceStats>();
            currentDestProvinceStats = orderManager.SupportMoveDestList[i].GetComponent<ProvinceStats>();
            if (currentOriginProvinceStats.hasArmy)
            {
                currentUnitType = "A";
            }
            else if (currentOriginProvinceStats.hasFleet)
            {
                currentUnitType = "F";
            }
            else
            {
                continue;
            }

            Debug.Log(currentUnitType + " " + currentOriginProvinceStats.provinceData.provinceName + " S " + currentTargetProvinceStats.provinceData.provinceName + " - " + currentDestProvinceStats.provinceData.provinceName);
        }
        hasParsedSupportMoves = true;
    }
    public void ParseSupportOrders()
    {
        Debug.Log("Support Orders: " + orderManager.SupportOriginList.Count.ToString());
        for (int i = 0; i < orderManager.SupportOriginList.Count; i++)
        {
            BREAKOUT.Check();
            currentOriginProvinceStats = orderManager.SupportOriginList[i].GetComponent<ProvinceStats>();
            currentTargetProvinceStats = orderManager.SupportTargetList[i].GetComponent<ProvinceStats>();
            if (currentOriginProvinceStats.hasArmy)
            {
                currentUnitType = "A";
            }
            else if (currentOriginProvinceStats.hasFleet)
            {
                currentUnitType = "F";
            }
            else
            {
                continue;
            }

            Debug.Log(currentUnitType + " " + currentOriginProvinceStats.provinceData.provinceName + " S " + currentTargetProvinceStats.provinceData.provinceName);
        }
        hasParsedMoves = true;
    }
    public void ParseConvoyOrders()
    {
        Debug.Log("Convoy Orders: " + orderManager.ConvoyOriginList.Count.ToString());
        for (int i = 0; i < orderManager.ConvoyOriginList.Count; i++)
        {
            BREAKOUT.Check();
            currentOriginProvinceStats = orderManager.ConvoyOriginList[i].GetComponent<ProvinceStats>();
            currentTargetProvinceStats = orderManager.ConvoyTargetList[i].GetComponent<ProvinceStats>();
            currentDestProvinceStats = orderManager.ConvoyDestList[i].GetComponent<ProvinceStats>();
            if (currentOriginProvinceStats.hasArmy)
            {
                currentUnitType = "A";
            }
            else if (currentOriginProvinceStats.hasFleet)
            {
                currentUnitType = "F";
            }
            else
            {
                continue;
            }

            Debug.Log(currentUnitType + " " + currentOriginProvinceStats.provinceData.provinceName + " C " + currentTargetProvinceStats.provinceData.provinceName + " - " + currentDestProvinceStats.provinceData.provinceName);
        }
        hasParsedSupportMoves = true;
    }
    IEnumerator ParsedChecker()
    {
        while (true)
        {
            BREAKOUT.Check();
            yield return new WaitForSeconds(0.1f);
            if (hasParsedHolds && hasParsedMoves && hasParsedSupportMoves && hasParsedSupports && hasParsedConvoys)
            {
                orderManager.ClearOrders();
            }
        }
    }
}
