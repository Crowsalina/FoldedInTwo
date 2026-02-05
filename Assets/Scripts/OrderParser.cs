using Crowsalina;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
public class OrderParser : MonoBehaviour
{
    private OrderManager orderManager;
    private YearManager yearManager;
    private GameManager gameManager;
    private ProvinceStats currentOriginProvinceStats, currentTargetProvinceStats, currentDestProvinceStats;
    public GameObject dislodgeDest;
    private string currentUnitType;
    private bool hasParsedHolds, hasParsedMoves, hasParsedSupportMoves, hasParsedSupports, hasParsedConvoys, hasParsedResupplys, hasChosen, dislodgeInput, disbandInput, fleetInput, armyInput;
    public bool isDislodgeActive = false;
    public bool isUnitChoiceActive = false;
    public List<ProvinceStats> provincesInScene = new List<ProvinceStats>();
    public List<GameObject> redoMoveOriginList = new List<GameObject>();
    public List<GameObject> redoMoveDestList = new List<GameObject>();
    public List<ProvinceStats> resupplyProvinces = new List<ProvinceStats>();
    public List<int> resupplyType = new List<int>();
    #region UnusedSupplies
    public List<ProvinceStats> OwnedSupplies1 = new List<ProvinceStats>();
    public List<ProvinceStats> OwnedSupplies2 = new List<ProvinceStats>();
    public List<ProvinceStats> OwnedSupplies3 = new List<ProvinceStats>();
    public List<ProvinceStats> OwnedSupplies4 = new List<ProvinceStats>();
    public List<ProvinceStats> OwnedSupplies5 = new List<ProvinceStats>();
    public List<ProvinceStats> OwnedSupplies6 = new List<ProvinceStats>();
    public List<ProvinceStats> OwnedSupplies7 = new List<ProvinceStats>();
    public List<ProvinceStats> OwnedUnits1 = new List<ProvinceStats>();
    public List<ProvinceStats> OwnedUnits2 = new List<ProvinceStats>();
    public List<ProvinceStats> OwnedUnits3 = new List<ProvinceStats>();
    public List<ProvinceStats> OwnedUnits4 = new List<ProvinceStats>();
    public List<ProvinceStats> OwnedUnits5 = new List<ProvinceStats>();
    public List<ProvinceStats> OwnedUnits6 = new List<ProvinceStats>();
    public List<ProvinceStats> OwnedUnits7 = new List<ProvinceStats>();
    public int unusedSupplyCount = 0;
    #endregion
    private void Start()
    {
        orderManager = FindFirstObjectByType<OrderManager>();
        yearManager = FindFirstObjectByType<YearManager>();
        gameManager = FindFirstObjectByType<GameManager>();
        hasParsedHolds = false;
        hasParsedMoves = false;
        hasParsedSupportMoves = false;
        hasParsedSupports = false;
        hasParsedConvoys = false;
        hasParsedResupplys = false;
        hasChosen = true;
    }
    public void StartParsingOrders()
    {
        if (yearManager.currentSeason != 2)
        {
            StartCoroutine(ParseOrders());
        }
        else
        {
            ParseResupplyOrders();
            StartCoroutine(ParsedChecker());
        }
    }
    public void ParseResupplyOrders()
    {
        for (int i = 0; i < resupplyProvinces.Count; i++)
        {
            BREAKOUT.Check();
            currentOriginProvinceStats = resupplyProvinces[i].GetComponent<ProvinceStats>();
            if (resupplyType[i] == 3)
            {
                continue;
            }
            else
            {
                if (resupplyType[i] == 0)
                {
                    currentUnitType = "A";
                    currentOriginProvinceStats.hasArmy = true;
                }
                else
                {
                    currentUnitType = "F";
                    currentOriginProvinceStats.hasFleet = true;
                }
                Debug.Log(currentUnitType + " " + currentOriginProvinceStats.provinceData.provinceName);
            }
        }
        hasParsedResupplys = true;
    }
    IEnumerator ParseOrders()
    {
        if (provincesInScene.Count == 0)
        {
            ProvinceStats[] provinces = ProvinceStats.FindObjectsByType<ProvinceStats>(FindObjectsSortMode.None);
            provincesInScene.AddRange(provinces);
        }
        ParseHoldOrders();
        ParseSupportMoveOrders();
        ParseSupportOrders();
        ParseConvoyOrders();
        StartCoroutine(ParseMoveOrders());
        while (true)
        {
            if (hasChosen)
            {
                StartCoroutine(ParsedChecker());
                break;
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
    public void ParseHoldOrders()
    {
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
    IEnumerator ParseMoveOrders()
    {
        for (int i = 0; i < orderManager.MoveOriginList.Count; i++)
        {
            BREAKOUT.Check();
            currentOriginProvinceStats = orderManager.MoveOriginList[i].GetComponent<ProvinceStats>();
            currentDestProvinceStats = orderManager.MoveDestList[i].GetComponent<ProvinceStats>();

            if (HasUnit(currentOriginProvinceStats) && AdjacencyCheck(currentOriginProvinceStats, currentTargetProvinceStats, currentDestProvinceStats, 0) && StandoffChecker(currentOriginProvinceStats, currentDestProvinceStats))
            {
                for (int j = 0; j < provincesInScene.Count; j++)
                {
                    if (currentDestProvinceStats.provinceData.provinceName == orderManager.MoveOriginList[i].GetComponent<ProvinceStats>().provinceData.provinceName)
                    {
                        redoMoveOriginList.Add(orderManager.MoveOriginList[i]);
                        redoMoveDestList.Add(orderManager.MoveDestList[i]);
                        goto Skipped;
                    }
                }
                Debug.Log(currentUnitType + " " + currentOriginProvinceStats.provinceData.provinceName + " - " + currentDestProvinceStats.provinceData.provinceName);
                if (currentDestProvinceStats.hasArmy || currentDestProvinceStats.hasFleet)
                {
                    Debug.Log("D - Dislodge\nX - Disband");
                    hasChosen = false;
                    StartCoroutine(DislodgeOrDisband());
                    while (true)
                    {
                        if (hasChosen)
                        {
                            if (currentUnitType == "A")
                            {
                                currentOriginProvinceStats.hasArmy = false;
                                currentDestProvinceStats.hasArmy = true;
                                if (currentDestProvinceStats.provinceData.childProvinces.Count > 0)
                                {
                                    for (int k = 0; k < currentDestProvinceStats.provinceData.childProvinces.Count; k++)
                                    {
                                        for (int j = 0; j < provincesInScene.Count; j++)
                                        {
                                            if (provincesInScene[j].provinceData.provinceName == currentDestProvinceStats.provinceData.childProvinces[k].provinceName)
                                            {
                                                provincesInScene[j].controllingPower = currentOriginProvinceStats.controllingPower;
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                currentOriginProvinceStats.hasFleet = false;
                                currentDestProvinceStats.hasFleet = true;
                                if (currentDestProvinceStats.provinceData.parentProvince != null)
                                {
                                    for (int j = 0; j < provincesInScene.Count; j++)
                                    {
                                        if (provincesInScene[j].provinceData.provinceName == currentDestProvinceStats.provinceData.parentProvince.provinceName)
                                        {
                                            provincesInScene[j].controllingPower = currentOriginProvinceStats.controllingPower;
                                            for (int k = 0; k < provincesInScene[j].provinceData.childProvinces.Count; k++)
                                            {
                                                for (int l = 0; l < provincesInScene.Count; l++)
                                                {
                                                    if (provincesInScene[l].provinceData.provinceName == provincesInScene[j].provinceData.childProvinces[k].provinceName)
                                                    {
                                                        provincesInScene[l].controllingPower = provincesInScene[j].controllingPower;
                                                    }
                                                }
                                            }
                                        }
                                    }

                                }
                            }
                            currentDestProvinceStats.controllingPower = currentOriginProvinceStats.controllingPower;
                            break;
                        }
                        yield return new WaitForSeconds(0.1f);
                    }
                }
                else
                {
                    if (currentUnitType == "A")
                    {
                        currentOriginProvinceStats.hasArmy = false;
                        currentDestProvinceStats.hasArmy = true;
                        if (currentDestProvinceStats.provinceData.childProvinces.Count > 0)
                        {
                            for (int k = 0; k < currentDestProvinceStats.provinceData.childProvinces.Count; k++)
                            {
                                for (int j = 0; j < provincesInScene.Count; j++)
                                {
                                    if (provincesInScene[j].provinceData.provinceName == currentDestProvinceStats.provinceData.childProvinces[k].provinceName)
                                    {
                                        provincesInScene[j].controllingPower = currentOriginProvinceStats.controllingPower;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        currentOriginProvinceStats.hasFleet = false;
                        currentDestProvinceStats.hasFleet = true;
                        if (currentDestProvinceStats.provinceData.parentProvince != null)
                        {
                            for (int j = 0; j < provincesInScene.Count; j++)
                            {
                                if (provincesInScene[j].provinceData.provinceName == currentDestProvinceStats.provinceData.parentProvince.provinceName)
                                {
                                    provincesInScene[j].controllingPower = currentOriginProvinceStats.controllingPower;
                                    for (int k = 0; k < provincesInScene[j].provinceData.childProvinces.Count; k++)
                                    {
                                        for (int l = 0; l < provincesInScene.Count; l++)
                                        {
                                            if (provincesInScene[l].provinceData.provinceName == provincesInScene[j].provinceData.childProvinces[k].provinceName)
                                            {
                                                provincesInScene[l].controllingPower = provincesInScene[j].controllingPower;
                                            }
                                        }
                                    }
                                }
                            }

                        }
                    }
                    currentDestProvinceStats.controllingPower = currentOriginProvinceStats.controllingPower;
                }
            }
            else
            {
                continue;
            }
        Skipped:
            continue;
        }
        for (int i = 0; i < redoMoveOriginList.Count; i++)
        {
            BREAKOUT.Check();
            currentOriginProvinceStats = redoMoveOriginList[i].GetComponent<ProvinceStats>();
            currentDestProvinceStats = redoMoveDestList[i].GetComponent<ProvinceStats>();

            if (HasUnit(currentOriginProvinceStats) && AdjacencyCheck(currentOriginProvinceStats, currentTargetProvinceStats, currentDestProvinceStats, 0) && StandoffChecker(currentOriginProvinceStats, currentDestProvinceStats))
            {
                Debug.Log(currentUnitType + " " + currentOriginProvinceStats.provinceData.provinceName + " - " + currentDestProvinceStats.provinceData.provinceName);
                if (currentDestProvinceStats.hasArmy || currentDestProvinceStats.hasFleet)
                {
                    Debug.Log("D - Dislodge\nX - Disband");
                    hasChosen = false;
                    StartCoroutine(DislodgeOrDisband());
                    while (true)
                    {
                        if (hasChosen)
                        {
                            if (currentUnitType == "A")
                            {
                                currentOriginProvinceStats.hasArmy = false;
                                currentDestProvinceStats.hasArmy = true;
                                if (currentDestProvinceStats.provinceData.childProvinces.Count > 0)
                                {
                                    for (int k = 0; k < currentDestProvinceStats.provinceData.childProvinces.Count; k++)
                                    {
                                        for (int j = 0; j < provincesInScene.Count; j++)
                                        {
                                            if (provincesInScene[j].provinceData.provinceName == currentDestProvinceStats.provinceData.childProvinces[k].provinceName)
                                            {
                                                provincesInScene[j].controllingPower = currentOriginProvinceStats.controllingPower;
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                currentOriginProvinceStats.hasFleet = false;
                                currentDestProvinceStats.hasFleet = true;
                                if (currentDestProvinceStats.provinceData.parentProvince != null)
                                {
                                    for (int j = 0; j < provincesInScene.Count; j++)
                                    {
                                        if (provincesInScene[j].provinceData.provinceName == currentDestProvinceStats.provinceData.parentProvince.provinceName)
                                        {
                                            provincesInScene[j].controllingPower = currentOriginProvinceStats.controllingPower;
                                            for (int k = 0; k < provincesInScene[j].provinceData.childProvinces.Count; k++)
                                            {
                                                for (int l = 0; l < provincesInScene.Count; l++)
                                                {
                                                    if (provincesInScene[l].provinceData.provinceName == provincesInScene[j].provinceData.childProvinces[k].provinceName)
                                                    {
                                                        provincesInScene[l].controllingPower = provincesInScene[j].controllingPower;
                                                    }
                                                }
                                            }
                                        }
                                    }

                                }
                            }
                            currentDestProvinceStats.controllingPower = currentOriginProvinceStats.controllingPower;
                            break;
                        }
                        yield return new WaitForSeconds(0.1f);
                    }
                }
                else
                {
                    if (currentUnitType == "A")
                    {
                        currentOriginProvinceStats.hasArmy = false;
                        currentDestProvinceStats.hasArmy = true;
                        if (currentDestProvinceStats.provinceData.childProvinces.Count > 0)
                        {
                            for (int k = 0; k < currentDestProvinceStats.provinceData.childProvinces.Count; k++)
                            {
                                for (int j = 0; j < provincesInScene.Count; j++)
                                {
                                    if (provincesInScene[j].provinceData.provinceName == currentDestProvinceStats.provinceData.childProvinces[k].provinceName)
                                    {
                                        provincesInScene[j].controllingPower = currentOriginProvinceStats.controllingPower;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        currentOriginProvinceStats.hasFleet = false;
                        currentDestProvinceStats.hasFleet = true;
                        if (currentDestProvinceStats.provinceData.parentProvince != null)
                        {
                            for (int j = 0; j < provincesInScene.Count; j++)
                            {
                                if (provincesInScene[j].provinceData.provinceName == currentDestProvinceStats.provinceData.parentProvince.provinceName)
                                {
                                    provincesInScene[j].controllingPower = currentOriginProvinceStats.controllingPower;
                                    for (int k = 0; k < provincesInScene[j].provinceData.childProvinces.Count; k++)
                                    {
                                        for (int l = 0; l < provincesInScene.Count; l++)
                                        {
                                            if (provincesInScene[l].provinceData.provinceName == provincesInScene[j].provinceData.childProvinces[k].provinceName)
                                            {
                                                provincesInScene[l].controllingPower = provincesInScene[j].controllingPower;
                                            }
                                        }
                                    }
                                }
                            }

                        }
                    }
                    currentDestProvinceStats.controllingPower = currentOriginProvinceStats.controllingPower;
                }
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
        for (int i = 0; i < orderManager.SupportMoveOriginList.Count; i++)
        {
            BREAKOUT.Check();
            currentOriginProvinceStats = orderManager.SupportMoveOriginList[i].GetComponent<ProvinceStats>();
            currentTargetProvinceStats = orderManager.SupportMoveTargetList[i].GetComponent<ProvinceStats>();
            try
            {
                currentDestProvinceStats = orderManager.SupportMoveDestList[i].GetComponent<ProvinceStats>();
            }
            catch
            {

            }
            if (HasUnit(currentOriginProvinceStats) && AdjacencyCheck(currentOriginProvinceStats, currentTargetProvinceStats, currentDestProvinceStats, 1))
            {
                Debug.Log(currentUnitType + " " + currentOriginProvinceStats.provinceData.provinceName + " S " + currentTargetProvinceStats.provinceData.provinceName + " - " + currentDestProvinceStats.provinceData.provinceName);
            }
            else
            {
                orderManager.SupportMoveOriginList.RemoveAt(i);
                orderManager.SupportMoveTargetList.RemoveAt(i);
                orderManager.SupportMoveDestList.RemoveAt(i);
                continue;
            }
        }
        hasParsedSupportMoves = true;
    }
    public void ParseSupportOrders()
    {
        for (int i = 0; i < orderManager.SupportOriginList.Count; i++)
        {
            BREAKOUT.Check();
            currentOriginProvinceStats = orderManager.SupportOriginList[i].GetComponent<ProvinceStats>();
            currentTargetProvinceStats = orderManager.SupportTargetList[i].GetComponent<ProvinceStats>();
            if (HasUnit(currentOriginProvinceStats) && AdjacencyCheck(currentOriginProvinceStats, currentTargetProvinceStats, currentDestProvinceStats, 2))
            {
                Debug.Log(currentUnitType + " " + currentOriginProvinceStats.provinceData.provinceName + " S " + currentTargetProvinceStats.provinceData.provinceName);
            }
            else
            {
                orderManager.SupportOriginList.RemoveAt(i);
                orderManager.SupportTargetList.RemoveAt(i);
                continue;
            }
        }
        hasParsedSupports = true;
    }
    public void ParseConvoyOrders()
    {
        for (int i = 0; i < orderManager.ConvoyOriginList.Count; i++)
        {
            BREAKOUT.Check();
            currentOriginProvinceStats = orderManager.ConvoyOriginList[i].GetComponent<ProvinceStats>();
            currentTargetProvinceStats = orderManager.ConvoyTargetList[i].GetComponent<ProvinceStats>();
            currentDestProvinceStats = orderManager.ConvoyDestList[i].GetComponent<ProvinceStats>();
            if (HasUnit(currentOriginProvinceStats) && ConvoyChecker(currentOriginProvinceStats, currentTargetProvinceStats, currentDestProvinceStats))
            {
                Debug.Log(currentUnitType + " " + currentOriginProvinceStats.provinceData.provinceName + " C " + currentTargetProvinceStats.provinceData.provinceName + " - " + currentDestProvinceStats.provinceData.provinceName);
            }
            else
            {
                orderManager.ConvoyOriginList.RemoveAt(i);
                orderManager.ConvoyTargetList.RemoveAt(i);
                orderManager.ConvoyDestList.RemoveAt(i);
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
            if (yearManager.currentSeason != 2)
            {
                Debug.Log("Order Failed - No Unit");
            }
            return false;
        }
    }
    public bool AdjacencyCheck(ProvinceStats origin, ProvinceStats target, ProvinceStats dest, int orderType)
    {
        //move = 0, support move = 1, support = 2, convoy = 3
        bool hasFoundAdjacency = false;
        if (orderType == 0 || orderType == 1)
        {
            if (currentUnitType == "A")
            {
                for (int i = 0; i < origin.provinceData.adjacentProvinces.Count; i++)
                {
                    if (origin.provinceData.adjacentProvinces[i].provinceName == dest.provinceData.provinceName && origin.provinceData.adjacentProvinces[i].isMaritime == false && origin.provinceData.adjacentProvinces[i].isFleetOnly == false)
                    {
                        hasFoundAdjacency = true;
                    }
                    else if (orderType == 0 && ConvoyMoveChecker(origin, dest))
                    {
                        hasFoundAdjacency = true;
                    }
                }
            }
            else
            {
                if (origin.provinceData.isMaritime)
                {
                    for (int i = 0; i < origin.provinceData.adjacentProvinces.Count; i++)
                    {
                        if (origin.provinceData.adjacentProvinces[i].provinceName == dest.provinceData.provinceName && dest.provinceData.childProvinces.Count == 0)
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
                        else
                        {
                            for (int j = 0; j < origin.provinceData.adjacentProvinces.Count; j++)
                            {
                                if (origin.provinceData.adjacentProvinces[j].provinceName == dest.provinceData.provinceName && origin.provinceData.adjacentProvinces[j].isMaritime)
                                {
                                    hasFoundAdjacency = true;
                                }
                            }
                        }
                    }
                }
            }
        }
        else if (orderType == 2)
        {
            if (currentUnitType == "A")
            {
                for (int i = 0; i < origin.provinceData.adjacentProvinces.Count; i++)
                {
                    if (origin.provinceData.adjacentProvinces[i].provinceName == target.provinceData.provinceName)
                    {
                        hasFoundAdjacency = true;
                    }
                }
            }
            else
            {
                if (target.provinceData.isMaritime || origin.provinceData.isMaritime)
                {
                    for (int j = 0; j < origin.provinceData.adjacentProvinces.Count; j++)
                    {
                        if (origin.provinceData.adjacentProvinces[j].provinceName == target.provinceData.provinceName)
                        {
                            hasFoundAdjacency = true;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < origin.provinceData.coastlineAdjacentProvinces.Count; i++)
                    {
                        if (origin.provinceData.coastlineAdjacentProvinces[i].provinceName == target.provinceData.provinceName)
                        {
                            hasFoundAdjacency = true;
                        }
                    }
                }
            }
        }
        else if (orderType == 3)
        {
            {
                for (int i = 0; i < origin.provinceData.adjacentProvinces.Count; i++)
                {
                    if (origin.provinceData.adjacentProvinces[i].provinceName == target.provinceData.provinceName)
                    {
                        for (int j = 0; j < origin.provinceData.adjacentProvinces.Count; j++)
                        {
                            if (origin.provinceData.adjacentProvinces[i].provinceName == dest.provinceData.provinceName && target.provinceData.provinceName != dest.provinceData.provinceName)
                            {
                                hasFoundAdjacency = true;
                            }
                        }
                    }
                }
            }
        }
        return hasFoundAdjacency;
    }
    public bool StandoffChecker(ProvinceStats origin, ProvinceStats dest)
    {
        bool hasWonStandoff = false;
        bool hasMoreSupport = true;
        bool noSupportOrders = false;
        bool noHoldOrders = false;
        if (orderManager.SupportMoveOriginList.Count > 0)
        {
            for (int i = 0; i < orderManager.MoveDestList.Count; i++)
            {
                if (dest.provinceData.provinceName == orderManager.MoveDestList[i].GetComponent<ProvinceStats>().provinceData.provinceName && origin.provinceData.provinceName != orderManager.MoveOriginList[i].GetComponent<ProvinceStats>().provinceData.provinceName)
                {
                    //current origin province = origin
                    //current dest province = dest
                    //current enemy origin province = moveoriginlist[i]
                    for (int j = 0; j < orderManager.SupportMoveTargetList.Count; j++)
                    {
                        if (origin.provinceData.provinceName == orderManager.SupportMoveTargetList[j].GetComponent<ProvinceStats>().provinceData.provinceName && dest.provinceData.provinceName == orderManager.SupportMoveDestList[j].GetComponent<ProvinceStats>().provinceData.provinceName)
                        {
                            //current supporting province = supportmoveoriginlist[j]
                            for (int k = 0; k < orderManager.SupportMoveDestList.Count; k++)
                            {
                                if (orderManager.SupportMoveDestList[k].GetComponent<ProvinceStats>().provinceData.provinceName == orderManager.MoveDestList[i].GetComponent<ProvinceStats>().provinceData.provinceName && orderManager.SupportMoveTargetList[k].GetComponent<ProvinceStats>().provinceData.provinceName == orderManager.MoveOriginList[i].GetComponent<ProvinceStats>().provinceData.provinceName && orderManager.SupportMoveTargetList[k].GetComponent<ProvinceStats>().provinceData.provinceName != origin.provinceData.provinceName)
                                {
                                    //current enemy supporting province = supportmoveoriginlist[k]
                                    if (orderManager.SupportTargetList.Count > 0)
                                    {
                                        for (int l = 0; l < orderManager.SupportTargetList.Count; l++)
                                        {
                                            if (orderManager.SupportTargetList[l].GetComponent<ProvinceStats>().provinceData.provinceName == orderManager.SupportMoveOriginList[j].GetComponent<ProvinceStats>().provinceData.provinceName && orderManager.SupportTargetList[l].GetComponent<ProvinceStats>().provinceData.provinceName != orderManager.SupportMoveOriginList[k].GetComponent<ProvinceStats>().provinceData.provinceName)
                                            {
                                                Debug.Log("Standoff Won: outputting winners move order");
                                            }
                                            else
                                            {
                                                return false;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        Debug.Log("Order Failed - Move standoff equal");
                                        return false;
                                    }
                                }
                            }
                            hasWonStandoff = hasMoreSupport;
                        }
                    }
                }
                else if (dest.hasArmy || dest.hasFleet)
                {
                    //current origin province = origin
                    //current dest province = dest
                    //current enemy origin province = moveoriginlist[i]
                    for (int j = 0; j < orderManager.SupportMoveTargetList.Count; j++)
                    {
                        if (origin.provinceData.provinceName == orderManager.SupportMoveTargetList[j].GetComponent<ProvinceStats>().provinceData.provinceName && dest.provinceData.provinceName == orderManager.SupportMoveDestList[j].GetComponent<ProvinceStats>().provinceData.provinceName)
                        {
                            //current supporting province = supportmoveoriginlist[j]
                            for (int k = 0; k < orderManager.SupportMoveDestList.Count; k++)
                            {
                                if (orderManager.SupportMoveDestList[k].GetComponent<ProvinceStats>().provinceData.provinceName == orderManager.MoveDestList[i].GetComponent<ProvinceStats>().provinceData.provinceName && orderManager.SupportMoveTargetList[k].GetComponent<ProvinceStats>().provinceData.provinceName == orderManager.MoveOriginList[i].GetComponent<ProvinceStats>().provinceData.provinceName && orderManager.SupportMoveTargetList[k].GetComponent<ProvinceStats>().provinceData.provinceName != origin.provinceData.provinceName)
                                {
                                    //current enemy supporting province = supportmoveoriginlist[k]
                                    if (orderManager.SupportTargetList.Count > 0)
                                    {
                                        for (int l = 0; l < orderManager.SupportTargetList.Count; l++)
                                        {
                                            if (orderManager.SupportTargetList[l].GetComponent<ProvinceStats>().provinceData.provinceName == orderManager.SupportMoveOriginList[j].GetComponent<ProvinceStats>().provinceData.provinceName && orderManager.SupportTargetList[l].GetComponent<ProvinceStats>().provinceData.provinceName != orderManager.SupportMoveOriginList[k].GetComponent<ProvinceStats>().provinceData.provinceName)
                                            {
                                                Debug.Log("Standoff Won: outputting winners move order");
                                            }
                                            else
                                            {
                                                return false;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        Debug.Log("Order Failed - Move standoff equal");
                                        return false;
                                    }
                                }
                            }
                            hasWonStandoff = hasMoreSupport;
                        }
                    }
                }
                else
                {
                    noSupportOrders = true;
                }
            }
        }
        else
        {
            noSupportOrders = true;
        }
        if (orderManager.HoldList.Count > 0)
        {
            for (int i = 0; i < orderManager.MoveDestList.Count; i++)
            {
                try
                {
                    if (dest.provinceData.provinceName == orderManager.HoldList[i].GetComponent<ProvinceStats>().provinceData.provinceName)
                    {
                        //current origin province = origin
                        //current dest province = dest
                        for (int j = 0; j < orderManager.SupportMoveTargetList.Count; j++)
                        {
                            if (origin.provinceData.provinceName == orderManager.SupportMoveTargetList[j].GetComponent<ProvinceStats>().provinceData.provinceName && dest.provinceData.provinceName == orderManager.SupportMoveDestList[j].GetComponent<ProvinceStats>().provinceData.provinceName)
                            {
                                //current supporting province = supportmoveoriginlist[j]
                                for (int k = 0; k < orderManager.SupportTargetList.Count; k++)
                                {
                                    if (orderManager.SupportTargetList[k].GetComponent<ProvinceStats>().provinceData.provinceName == dest.provinceData.provinceName)
                                    {
                                        //current enemy supporting province = supportoriginlist[k]
                                        return false;
                                    }
                                }
                                hasWonStandoff = hasMoreSupport;
                            }
                        }
                    }
                    else
                    {
                        noHoldOrders = true;
                    }
                }
                catch
                {
                    noHoldOrders = true;
                }
            }
        }
        else
        {
            noHoldOrders = true;
        }
        if (noHoldOrders && noSupportOrders)
        {
            for (int i = 0; i < orderManager.MoveDestList.Count; i++)
            {
                if (dest.provinceData.provinceName == orderManager.MoveDestList[i].GetComponent<ProvinceStats>().provinceData.provinceName && origin.provinceData.provinceName != orderManager.MoveOriginList[i].GetComponent<ProvinceStats>().provinceData.provinceName)
                {
                    Debug.Log("Order Failed - standoff equal");
                    return false;
                }
            }
            if (dest.hasArmy || dest.hasFleet)
            {
                Debug.Log("Order Failed - no support for invasion");
                return false;
            }
            else if (dest.provinceData.childProvinces.Count > 0)
            {
                for (int i = 0; i < provincesInScene.Count; i++)
                {
                    for (int j = 0; j < dest.provinceData.childProvinces.Count; j++)
                    {
                        if (provincesInScene[i].provinceData.provinceName == dest.provinceData.childProvinces[j].provinceName)
                        {
                            if (provincesInScene[i].hasArmy || provincesInScene[i].hasFleet)
                            {
                                Debug.Log("Order Failed - no support for invasion");
                                return false;
                            }
                        }
                    }
                }
            }
            hasWonStandoff = true;
        }
        return hasWonStandoff;
    }
    public bool ConvoyMoveChecker(ProvinceStats origin, ProvinceStats dest)
    {
        if (orderManager.ConvoyOriginList.Count > 0)
        {
            for (int i = 0; i < orderManager.ConvoyOriginList.Count; i++)
            {
                if (orderManager.ConvoyTargetList[i].GetComponent<ProvinceStats>().provinceData.provinceName == origin.provinceData.provinceName && orderManager.ConvoyDestList[i].GetComponent<ProvinceStats>().provinceData.provinceName == dest.provinceData.provinceName)
                {
                    if (ConvoyChecker(orderManager.ConvoyOriginList[i].GetComponent<ProvinceStats>(), orderManager.ConvoyTargetList[i].GetComponent<ProvinceStats>(), orderManager.ConvoyDestList[i].GetComponent<ProvinceStats>()))
                    {
                        if (orderManager.ConvoyOriginList[i].GetComponent<ProvinceStats>().provinceData.isMaritime && orderManager.ConvoyTargetList[i].GetComponent<ProvinceStats>().provinceData.isCoastal && orderManager.ConvoyDestList[i].GetComponent<ProvinceStats>().provinceData.isCoastal)
                        {
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }
    public bool ConvoyChecker(ProvinceStats origin, ProvinceStats target, ProvinceStats dest)
    {
        if (origin.provinceData.isMaritime && target.provinceData.isCoastal && dest.provinceData.isCoastal && AdjacencyCheck(origin, target, dest, 0) && AdjacencyCheck(target, dest, origin, 0))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private void Update()
    {
        if (hasChosen == false)
        {
            if (Input.GetKeyDown(KeyCode.D))
            {
                dislodgeInput = true;
            }
            if (Input.GetKeyDown(KeyCode.X))
            {
                disbandInput = true;
            }
        }
        if (isUnitChoiceActive == true)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                armyInput = true;
                isUnitChoiceActive = false;
            }
            if (Input.GetKeyDown(KeyCode.F))
            {
                fleetInput = true;
                isUnitChoiceActive = false;
            }
        }
    }
    IEnumerator DislodgeOrDisband()
    {
        while (hasChosen == false)
        {
            BREAKOUT.Check();
            if (dislodgeInput)
            {
                Debug.Log("Select Adjacent Province for dislodged unit");
                isDislodgeActive = true;
                dislodgeInput = false;
            }
            if (disbandInput)
            {
                disbandInput = false;
                currentDestProvinceStats.hasArmy = false;
                currentDestProvinceStats.hasFleet = false;
                if (currentUnitType == "A")
                {
                    currentOriginProvinceStats.hasArmy = false;
                    currentDestProvinceStats.hasArmy = true;
                }
                else
                {
                    currentOriginProvinceStats.hasFleet = false;
                    currentDestProvinceStats.hasFleet = true;
                }
                currentDestProvinceStats.controllingPower = currentOriginProvinceStats.controllingPower;
                hasChosen = true;
            }
            while (isDislodgeActive)
            {
                BREAKOUT.Check();
                yield return new WaitForSeconds(0.1f);
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
    public void DislodgeUnit(GameObject dest)
    {
        if (AdjacencyCheck(currentDestProvinceStats, null, dest.GetComponent<ProvinceStats>(), 0))
        {
            currentDestProvinceStats.hasArmy = false;
            currentDestProvinceStats.hasFleet = false;
            if (currentUnitType == "A")
            {
                dest.GetComponent<ProvinceStats>().hasArmy = true;
            }
            else
            {
                dest.GetComponent<ProvinceStats>().hasFleet = true;
            }
            dest.GetComponent<ProvinceStats>().controllingPower = currentDestProvinceStats.controllingPower;
            isDislodgeActive = false;
            hasChosen = true;
        }
    }
    IEnumerator ParsedChecker()
    {
        while (true)
        {
            BREAKOUT.Check();
            yield return new WaitForSeconds(0.1f);
            if (yearManager.currentSeason != 2)
            {
                if (hasParsedHolds && hasParsedMoves && hasParsedSupportMoves && hasParsedSupports && hasParsedConvoys)
                {
                    orderManager.ClearOrders();
                    UpdateAllProvinces();
                    CheckUnusedProvinces();
                    currentDestProvinceStats = null;
                    currentOriginProvinceStats = null;
                    currentTargetProvinceStats = null;
                    redoMoveDestList.Clear();
                    redoMoveDestList.TrimExcess();
                    redoMoveOriginList.Clear();
                    redoMoveOriginList.TrimExcess();
                    yearManager.SetYearText();
                    gameManager.round += 1;
                    break;
                }
            }
            else
            {
                if (hasParsedResupplys)
                {
                    orderManager.ClearOrders();
                    UpdateAllProvinces();
                    CheckUnusedProvinces();
                    currentDestProvinceStats = null;
                    currentOriginProvinceStats = null;
                    currentTargetProvinceStats = null;
                    resupplyProvinces.Clear();
                    resupplyProvinces.TrimExcess();
                    resupplyType.Clear();
                    resupplyType.TrimExcess();
                    redoMoveDestList.Clear();
                    redoMoveDestList.TrimExcess();
                    redoMoveOriginList.Clear();
                    redoMoveOriginList.TrimExcess();
                    yearManager.SetYearText();
                    gameManager.round += 1;
                    break;
                }
            }
        }
    }
    public void CheckUnusedProvinces()
    {
        OwnedSupplies1.Clear();
        OwnedSupplies2.Clear();
        OwnedSupplies3.Clear();
        OwnedSupplies4.Clear();
        OwnedSupplies5.Clear();
        OwnedSupplies6.Clear();
        OwnedSupplies7.Clear();
        OwnedUnits1.Clear();
        OwnedUnits2.Clear();
        OwnedUnits3.Clear();
        OwnedUnits4.Clear();
        OwnedUnits5.Clear();
        OwnedUnits6.Clear();
        OwnedUnits7.Clear();
        for (int i = 0; i < provincesInScene.Count; i++)
        {
            if (provincesInScene[i].provinceData.isSupply && provincesInScene[i].controllingPower != 0)
            {
                switch (provincesInScene[i].controllingPower)
                {
                    case 1:
                        OwnedSupplies1.Add(provincesInScene[i]);
                        break;
                    case 2:
                        OwnedSupplies2.Add(provincesInScene[i]);
                        break;
                    case 3:
                        OwnedSupplies3.Add(provincesInScene[i]);
                        break;
                    case 4:
                        OwnedSupplies4.Add(provincesInScene[i]);
                        break;
                    case 5:
                        OwnedSupplies5.Add(provincesInScene[i]);
                        break;
                    case 6:
                        OwnedSupplies6.Add(provincesInScene[i]);
                        break;
                    case 7:
                        OwnedSupplies7.Add(provincesInScene[i]);
                        break;
                }
            }
        }
        for (int i = 0; i < provincesInScene.Count; i++)
        {
            if (provincesInScene[i].controllingPower != 0)
            {
                if (provincesInScene[i].hasArmy || provincesInScene[i].hasFleet)
                {
                    switch (provincesInScene[i].controllingPower)
                    {
                        case 1:
                            OwnedUnits1.Add(provincesInScene[i]);
                            break;
                        case 2:
                            OwnedUnits2.Add(provincesInScene[i]);
                            break;
                        case 3:
                            OwnedUnits3.Add(provincesInScene[i]);
                            break;
                        case 4:
                            OwnedUnits4.Add(provincesInScene[i]);
                            break;
                        case 5:
                            OwnedUnits5.Add(provincesInScene[i]);
                            break;
                        case 6:
                            OwnedUnits6.Add(provincesInScene[i]);
                            break;
                        case 7:
                            OwnedUnits7.Add(provincesInScene[i]);
                            break;
                    }
                }
            }
        }
        if (OwnedSupplies1.Count > OwnedUnits1.Count)
        {
            unusedSupplyCount++;
        }
        if (OwnedSupplies2.Count > OwnedUnits2.Count)
        {
            unusedSupplyCount++;
        }
        if (OwnedSupplies3.Count > OwnedUnits3.Count)
        {
            unusedSupplyCount++;
        }
        if (OwnedSupplies4.Count > OwnedUnits4.Count)
        {
            unusedSupplyCount++;
        }
        if (OwnedSupplies5.Count > OwnedUnits5.Count)
        {
            unusedSupplyCount++;
        }
        if (OwnedSupplies6.Count > OwnedUnits6.Count)
        {
            unusedSupplyCount++;
        }
        if (OwnedSupplies7.Count > OwnedUnits7.Count)
        {
            unusedSupplyCount++;
        }
    }
    public void PlaceUnits(ProvinceStats placementProvince)
    {
        StartCoroutine(UnitSelection(placementProvince));
    }
    IEnumerator UnitSelection(ProvinceStats placementProvince)
    {
        int resupplyordercount = 0;
        for (int j = 0; j < resupplyProvinces.Count; j++)
        {
            for (int i = 0; i < provincesInScene.Count; i++)
            {
                if (resupplyProvinces[j].provinceData.provinceName == provincesInScene[i].provinceData.provinceName)
                {
                    if (provincesInScene[i].controllingPower == placementProvince.controllingPower && resupplyType[j] != 3)
                    {
                        resupplyordercount++;
                    }
                }
            }
        }
        if (resupplyordercount > 0)
        {
            switch (placementProvince.controllingPower)
            {
                case 1:
                    if (OwnedSupplies1.Count == OwnedUnits1.Count + resupplyordercount)
                    {
                        Debug.Log("Replacing last order for this power");
                        for (int i = (resupplyProvinces.Count - 1); i == 0; i--)
                        {
                            if (resupplyProvinces[i].controllingPower == placementProvince.controllingPower)
                            {
                                Debug.Log("removed " + resupplyType[i] + " resupply to " + resupplyProvinces[i].provinceData.provinceName);
                                resupplyType[i] = 3;
                                break;
                            }
                        }
                    }
                    isUnitChoiceActive = true;
                    Debug.Log("Choose unit type to be placed:");
                    Debug.Log("A - Army");
                    Debug.Log("F - Fleet");
                    while (isUnitChoiceActive)
                    {
                        yield return new WaitForSeconds(0.1f);
                    }
                    if (armyInput)
                    {
                        resupplyProvinces.Add(placementProvince);
                        resupplyType.Add(0);
                        Debug.Log("Added army resupply to " + placementProvince.provinceData.provinceName);
                        armyInput = false;
                    }
                    else if (fleetInput && placementProvince.provinceData.isCoastal == false)
                    {
                        Debug.Log("Cannot place Fleet inland, place an army or select a coastal supply");
                        fleetInput = false;
                    }
                    else if (fleetInput && placementProvince.provinceData.isCoastal)
                    {
                        resupplyProvinces.Add(placementProvince);
                        resupplyType.Add(1);
                        Debug.Log("Added fleet resupply to " + placementProvince.provinceData.provinceName);
                        fleetInput = false;
                    }
                    break;
                case 2:
                    if (OwnedSupplies2.Count == OwnedUnits2.Count + resupplyordercount)
                    {
                        Debug.Log("Replacing last order for this power");
                        for (int i = (resupplyProvinces.Count - 1); i == 0; i--)
                        {
                            if (resupplyProvinces[i].controllingPower == placementProvince.controllingPower)
                            {
                                Debug.Log("removed " + resupplyType[i] + " resupply to " + resupplyProvinces[i].provinceData.provinceName);
                                resupplyType[i] = 3;
                                break;
                            }
                        }
                    }
                    isUnitChoiceActive = true;
                    Debug.Log("Choose unit type to be placed:");
                    Debug.Log("A - Army");
                    Debug.Log("F - Fleet");
                    while (isUnitChoiceActive)
                    {
                        yield return new WaitForSeconds(0.1f);
                    }
                    if (armyInput)
                    {
                        resupplyProvinces.Add(placementProvince);
                        resupplyType.Add(0);
                        Debug.Log("Added army resupply to " + placementProvince.provinceData.provinceName);
                        armyInput = false;
                    }
                    else if (fleetInput && placementProvince.provinceData.isCoastal == false)
                    {
                        Debug.Log("Cannot place Fleet inland, place an army or select a coastal supply");
                        fleetInput = false;
                    }
                    else if (fleetInput && placementProvince.provinceData.isCoastal)
                    {
                        resupplyProvinces.Add(placementProvince);
                        resupplyType.Add(1);
                        Debug.Log("Added fleet resupply to " + placementProvince.provinceData.provinceName);
                        fleetInput = false;
                    }
                    break;
                case 3:
                    if (OwnedSupplies3.Count == OwnedUnits3.Count + resupplyordercount)
                    {
                        Debug.Log("Replacing last order for this power");
                        for (int i = (resupplyProvinces.Count - 1); i == 0; i--)
                        {
                            if (resupplyProvinces[i].controllingPower == placementProvince.controllingPower)
                            {
                                Debug.Log("removed " + resupplyType[i] + " resupply to " + resupplyProvinces[i].provinceData.provinceName);
                                resupplyType[i] = 3;
                                break;
                            }
                        }
                    }
                    isUnitChoiceActive = true;
                    Debug.Log("Choose unit type to be placed:");
                    Debug.Log("A - Army");
                    Debug.Log("F - Fleet");
                    while (isUnitChoiceActive)
                    {
                        yield return new WaitForSeconds(0.1f);
                    }
                    if (armyInput)
                    {
                        resupplyProvinces.Add(placementProvince);
                        resupplyType.Add(0);
                        Debug.Log("Added army resupply to " + placementProvince.provinceData.provinceName);
                        armyInput = false;
                    }
                    else if (fleetInput && placementProvince.provinceData.isCoastal == false)
                    {
                        Debug.Log("Cannot place Fleet inland, place an army or select a coastal supply");
                        fleetInput = false;
                    }
                    else if (fleetInput && placementProvince.provinceData.isCoastal)
                    {
                        resupplyProvinces.Add(placementProvince);
                        resupplyType.Add(1);
                        Debug.Log("Added fleet resupply to " + placementProvince.provinceData.provinceName);
                        fleetInput = false;
                    }
                    break;
                case 4:
                    if (OwnedSupplies4.Count == OwnedUnits4.Count + resupplyordercount)
                    {
                        Debug.Log("Replacing last order for this power");
                        for (int i = (resupplyProvinces.Count - 1); i == 0; i--)
                        {
                            if (resupplyProvinces[i].controllingPower == placementProvince.controllingPower)
                            {
                                Debug.Log("removed " + resupplyType[i] + " resupply to " + resupplyProvinces[i].provinceData.provinceName);
                                resupplyType[i] = 3;
                                break;
                            }
                        }
                    }
                    isUnitChoiceActive = true;
                    Debug.Log("Choose unit type to be placed:");
                    Debug.Log("A - Army");
                    Debug.Log("F - Fleet");
                    while (isUnitChoiceActive)
                    {
                        yield return new WaitForSeconds(0.1f);
                    }
                    if (armyInput)
                    {
                        resupplyProvinces.Add(placementProvince);
                        resupplyType.Add(0);
                        Debug.Log("Added army resupply to " + placementProvince.provinceData.provinceName);
                        armyInput = false;
                    }
                    else if (fleetInput && placementProvince.provinceData.isCoastal == false)
                    {
                        Debug.Log("Cannot place Fleet inland, place an army or select a coastal supply");
                        fleetInput = false;
                    }
                    else if (fleetInput && placementProvince.provinceData.isCoastal)
                    {
                        resupplyProvinces.Add(placementProvince);
                        resupplyType.Add(1);
                        Debug.Log("Added fleet resupply to " + placementProvince.provinceData.provinceName);
                        fleetInput = false;
                    }
                    break;
                case 5:
                    if (OwnedSupplies5.Count == OwnedUnits5.Count + resupplyordercount)
                    {
                        Debug.Log("Replacing last order for this power");
                        for (int i = (resupplyProvinces.Count - 1); i == 0; i--)
                        {
                            if (resupplyProvinces[i].controllingPower == placementProvince.controllingPower)
                            {
                                Debug.Log("removed " + resupplyType[i] + " resupply to " + resupplyProvinces[i].provinceData.provinceName);
                                resupplyType[i] = 3;
                                break;
                            }
                        }
                    }
                    isUnitChoiceActive = true;
                    Debug.Log("Choose unit type to be placed:");
                    Debug.Log("A - Army");
                    Debug.Log("F - Fleet");
                    while (isUnitChoiceActive)
                    {
                        yield return new WaitForSeconds(0.1f);
                    }
                    if (armyInput)
                    {
                        resupplyProvinces.Add(placementProvince);
                        resupplyType.Add(0);
                        Debug.Log("Added army resupply to " + placementProvince.provinceData.provinceName);
                        armyInput = false;
                    }
                    else if (fleetInput && placementProvince.provinceData.isCoastal == false)
                    {
                        Debug.Log("Cannot place Fleet inland, place an army or select a coastal supply");
                        fleetInput = false;
                    }
                    else if (fleetInput && placementProvince.provinceData.isCoastal)
                    {
                        resupplyProvinces.Add(placementProvince);
                        resupplyType.Add(1);
                        Debug.Log("Added fleet resupply to " + placementProvince.provinceData.provinceName);
                        fleetInput = false;
                    }
                    break;
                case 6:
                    if (OwnedSupplies6.Count == OwnedUnits6.Count + resupplyordercount)
                    {
                        Debug.Log("Replacing last order for this power");
                        for (int i = (resupplyProvinces.Count - 1); i == 0; i--)
                        {
                            if (resupplyProvinces[i].controllingPower == placementProvince.controllingPower)
                            {
                                Debug.Log("removed " + resupplyType[i] + " resupply to " + resupplyProvinces[i].provinceData.provinceName);
                                resupplyType[i] = 3;
                                break;
                            }
                        }
                    }
                    isUnitChoiceActive = true;
                    Debug.Log("Choose unit type to be placed:");
                    Debug.Log("A - Army");
                    Debug.Log("F - Fleet");
                    while (isUnitChoiceActive)
                    {
                        yield return new WaitForSeconds(0.1f);
                    }
                    if (armyInput)
                    {
                        resupplyProvinces.Add(placementProvince);
                        resupplyType.Add(0);
                        Debug.Log("Added army resupply to " + placementProvince.provinceData.provinceName);
                        armyInput = false;
                    }
                    else if (fleetInput && placementProvince.provinceData.isCoastal == false)
                    {
                        Debug.Log("Cannot place Fleet inland, place an army or select a coastal supply");
                        fleetInput = false;
                    }
                    else if (fleetInput && placementProvince.provinceData.isCoastal)
                    {
                        resupplyProvinces.Add(placementProvince);
                        resupplyType.Add(1);
                        Debug.Log("Added fleet resupply to " + placementProvince.provinceData.provinceName);
                        fleetInput = false;
                    }
                    break;
                case 7:
                    if (OwnedSupplies7.Count == OwnedUnits7.Count + resupplyordercount)
                    {
                        Debug.Log("Replacing last order for this power");
                        for (int i = (resupplyProvinces.Count - 1); i == 0; i--)
                        {
                            if (resupplyProvinces[i].controllingPower == placementProvince.controllingPower)
                            {
                                Debug.Log("removed " + resupplyType[i] + " resupply to " + resupplyProvinces[i].provinceData.provinceName);
                                resupplyType[i] = 3;
                                break;
                            }
                        }
                    }
                    isUnitChoiceActive = true;
                    Debug.Log("Choose unit type to be placed:");
                    Debug.Log("A - Army");
                    Debug.Log("F - Fleet");
                    while (isUnitChoiceActive)
                    {
                        yield return new WaitForSeconds(0.1f);
                    }
                    if (armyInput)
                    {
                        resupplyProvinces.Add(placementProvince);
                        resupplyType.Add(0);
                        Debug.Log("Added army resupply to " + placementProvince.provinceData.provinceName);
                        armyInput = false;
                    }
                    else if (fleetInput && placementProvince.provinceData.isCoastal == false)
                    {
                        Debug.Log("Cannot place Fleet inland, place an army or select a coastal supply");
                        fleetInput = false;
                    }
                    else if (fleetInput && placementProvince.provinceData.isCoastal)
                    {
                        resupplyProvinces.Add(placementProvince);
                        resupplyType.Add(1);
                        Debug.Log("Added fleet resupply to " + placementProvince.provinceData.provinceName);
                        fleetInput = false;
                    }
                    break;
            }
        }
        else
        {
            switch (placementProvince.controllingPower)
            {
                case 1:
                    if (OwnedSupplies1.Count == OwnedUnits1.Count)
                    {
                        Debug.Log("This power has maximum units already");
                        yield break;
                    }
                    break;
                case 2:
                    if (OwnedSupplies2.Count == OwnedUnits2.Count)
                    {
                        Debug.Log("This power has maximum units already");
                        yield break;
                    }
                    break;
                case 3:
                    if (OwnedSupplies3.Count == OwnedUnits3.Count)
                    {
                        Debug.Log("This power has maximum units already");
                        yield break;
                    }
                    break;
                case 4:
                    if (OwnedSupplies4.Count == OwnedUnits4.Count)
                    {
                        Debug.Log("This power has maximum units already");
                        yield break;
                    }
                    break;
                case 5:
                    if (OwnedSupplies5.Count == OwnedUnits5.Count)
                    {
                        Debug.Log("This power has maximum units already");
                        yield break;
                    }
                    break;
                case 6:
                    if (OwnedSupplies6.Count == OwnedUnits6.Count)
                    {
                        Debug.Log("This power has maximum units already");
                        yield break;
                    }
                    break;
                case 7:
                    if (OwnedSupplies7.Count == OwnedUnits7.Count)
                    {
                        Debug.Log("This power has maximum units already");
                        yield break;
                    }
                    break;
            }
            isUnitChoiceActive = true;
            Debug.Log("Choose unit type to be placed:");
            Debug.Log("A - Army");
            Debug.Log("F - Fleet");
            while (isUnitChoiceActive)
            {
                yield return new WaitForSeconds(0.1f);
            }
            if (armyInput)
            {
                resupplyProvinces.Add(placementProvince);
                resupplyType.Add(0);
                Debug.Log("Added army resupply to " + placementProvince.provinceData.provinceName);
                armyInput = false;
            }
            else if (fleetInput && placementProvince.provinceData.isCoastal == false)
            {
                Debug.Log("Cannot place Fleet inland, place an army or select a coastal supply");
                fleetInput = false;
            }
            else if (fleetInput && placementProvince.provinceData.isCoastal)
            {
                resupplyProvinces.Add(placementProvince);
                resupplyType.Add(1);
                Debug.Log("Added fleet resupply to " + placementProvince.provinceData.provinceName);
                fleetInput = false;
            }
        }
        UpdateAllProvinces();
    }
    public void UpdateAllProvinces()
    {
        for (int i = 0; i < provincesInScene.Count; i++)
        {
            provincesInScene[i].UpdateProvinceStats();
        }
    }
}
