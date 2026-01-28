using Crowsalina;
using System.Collections;
using UnityEngine;

public class OrderParser : MonoBehaviour
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
            if (HasUnit(currentOriginProvinceStats))
            {
                Debug.Log(currentUnitType + " " + currentOriginProvinceStats.provinceData.provinceName + " H");
            }
            else
            {
                continue;
            }
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
            if (HasUnit(currentOriginProvinceStats) && AdjacencyCheck(currentOriginProvinceStats,currentTargetProvinceStats,currentDestProvinceStats,0))
            {
                Debug.Log(currentUnitType + " " + currentOriginProvinceStats.provinceData.provinceName + " - " + currentDestProvinceStats.provinceData.provinceName);
            }
            else
            {
                continue;
            }
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
            if (HasUnit(currentOriginProvinceStats))
            {
                Debug.Log(currentUnitType + " " + currentOriginProvinceStats.provinceData.provinceName + " S " + currentTargetProvinceStats.provinceData.provinceName + " - " + currentDestProvinceStats.provinceData.provinceName);
            }
            else
            {
                continue;
            }
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
            if (HasUnit(currentOriginProvinceStats))
            {
                Debug.Log(currentUnitType + " " + currentOriginProvinceStats.provinceData.provinceName + " S " + currentTargetProvinceStats.provinceData.provinceName);
            }
            else
            {
                continue;
            }
        }
        hasParsedSupports = true;
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
            if (HasUnit(currentOriginProvinceStats))
            {
                Debug.Log(currentUnitType + " " + currentOriginProvinceStats.provinceData.provinceName + " C " + currentTargetProvinceStats.provinceData.provinceName + " - " + currentDestProvinceStats.provinceData.provinceName);
            }
            else
            {
                continue;
            }
        }
        hasParsedConvoys = true;
    }
    public bool HasUnit(ProvinceStats current)
    {
        if (current.hasArmy)
        {
            currentUnitType = "A";
            return true;
        }
        else if (current.hasFleet)
        {
            currentUnitType = "F";
            return true;
        }
        else
        {
            Debug.Log("Order Failed - No Unit");
            return false;
        }
    }
    public bool AdjacencyCheck(ProvinceStats origin, ProvinceStats target, ProvinceStats dest, int orderType)
    {
        //move = 0, support move = 1, support = 2, convoy = 3
        bool hasFoundAdjacency = false;
        switch (orderType)
        {
            case 0:
                if (currentUnitType == "A")
                {
                    for (int i = 0; i < origin.provinceData.adjacentProvinces.Count; i++)
                    {
                        if (origin.provinceData.adjacentProvinces[i].provinceName == dest.provinceData.provinceName)
                        {
                            hasFoundAdjacency = true;
                        }
                    }
                }
                else 
                {
                    for (int i = 0; i < origin.provinceData.coastlineAdjacentProvinces.Count; i++)
                    {
                        if (origin.provinceData.coastlineAdjacentProvinces[i].provinceName == dest.provinceData.provinceName)
                        {
                            hasFoundAdjacency = true;
                        }
                    }
                }
                    break;
            case 1:

                break;
            case 2:

                break;
            case 3:

                break;
        }
        return hasFoundAdjacency;
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
