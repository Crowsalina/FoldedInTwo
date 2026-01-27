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
        for (int i = 0; i < orderManager.HoldList.Count; i++)
        {
            currentOriginProvinceStats = orderManager.HoldList[i].GetComponent<ProvinceStats>();
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
                return;
            }
            Debug.Log(currentUnitType +" "+ currentOriginProvinceStats.provinceData.provinceName + " H");
        }
        hasParsedHolds = true;
    }
    public void ParseMoveOrders()
    {
        for (int i = 0; i < orderManager.MoveOriginList.Count; i++)
        {
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
                return;
            }

            Debug.Log(currentUnitType + " " + currentOriginProvinceStats.provinceData.provinceName + " - " + currentDestProvinceStats.provinceData.provinceName);
        }
        hasParsedMoves = true;
    }
    public void ParseSupportMoveOrders()
    {
        for (int i = 0; i < orderManager.SupportMoveOriginList.Count; i++)
        {
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
                return;
            }

            Debug.Log(currentUnitType + " " + currentOriginProvinceStats.provinceData.provinceName + " S " + currentTargetProvinceStats.provinceData.provinceName + " - " + currentDestProvinceStats.provinceData.provinceName);
        }
        hasParsedSupportMoves = true;
    }
    public void ParseSupportOrders()
    {
        for (int i = 0; i < orderManager.SupportOriginList.Count; i++)
        {
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
                return;
            }

            Debug.Log(currentUnitType + " " + currentOriginProvinceStats.provinceData.provinceName + " S " + currentTargetProvinceStats.provinceData.provinceName);
        }
        hasParsedMoves = true;
    }
    public void ParseConvoyOrders()
    {
        for (int i = 0; i < orderManager.ConvoyOriginList.Count; i++)
        {
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
                return;
            }

            Debug.Log(currentUnitType + " " + currentOriginProvinceStats.provinceData.provinceName + " C " + currentTargetProvinceStats.provinceData.provinceName + " - " + currentDestProvinceStats.provinceData.provinceName);
        }
        hasParsedSupportMoves = true;
    }
    IEnumerator ParsedChecker()
    { 
        yield return new WaitForSeconds(0.1f);
        if (hasParsedHolds && hasParsedMoves && hasParsedSupportMoves && hasParsedSupports && hasParsedConvoys)
        {
            orderManager.ClearOrders();
        }
    }
}
