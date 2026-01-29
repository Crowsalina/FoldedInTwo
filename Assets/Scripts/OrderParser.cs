using Crowsalina;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class OrderParser : MonoBehaviour
{
    private OrderManager orderManager;
    private ProvinceStats currentOriginProvinceStats, currentTargetProvinceStats, currentDestProvinceStats;
    public GameObject dislodgeDest;
    private string currentUnitType;
    private bool hasParsedHolds, hasParsedMoves, hasParsedSupportMoves, hasParsedSupports, hasParsedConvoys, hasChosen, dislodgeInput, disbandInput;
    public bool isDislodgeActive = false;
    List<ProvinceStats> provincesInScene = new List<ProvinceStats>();
    private void Start()
    {
        orderManager = FindFirstObjectByType<OrderManager>();
        hasParsedHolds = false;
        hasParsedMoves = false;
        hasParsedSupportMoves = false;
        hasParsedSupports = false;
        hasParsedConvoys = false;
        hasChosen = true;
    }
    public void StartParsingOrders()
    {
        StartCoroutine(ParseOrders());
    }
    IEnumerator ParseOrders()
    {

        ProvinceStats[] provinces = ProvinceStats.FindObjectsByType<ProvinceStats>(FindObjectsSortMode.None);
        provincesInScene.AddRange(provinces);
        ParseHoldOrders();
        StartCoroutine(ParseMoveOrders());
        while (true)
        {
            if (hasChosen)
            {
                ParseSupportMoveOrders();
                ParseSupportOrders();
                ParseConvoyOrders();
                StartCoroutine(ParsedChecker());
                break;
            }
            yield return new WaitForSeconds(0.1f);
        }
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
    IEnumerator ParseMoveOrders()
    {
        Debug.Log("Move Orders: " + orderManager.MoveOriginList.Count.ToString());
        for (int i = 0; i < orderManager.MoveOriginList.Count; i++)
        {
            BREAKOUT.Check();
            currentOriginProvinceStats = orderManager.MoveOriginList[i].GetComponent<ProvinceStats>();
            currentDestProvinceStats = orderManager.MoveDestList[i].GetComponent<ProvinceStats>();
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
                            }
                            else
                            {
                                currentOriginProvinceStats.hasFleet = false;
                                currentDestProvinceStats.hasFleet = true;
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
                    }
                    else
                    {
                        currentOriginProvinceStats.hasFleet = false;
                        currentDestProvinceStats.hasFleet = true;
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
        if (orderType == 0 || orderType == 2)
        {
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
                for (int i = 0; i < origin.provinceData.coastlineAdjacentProvinces.Count; i++)
                {
                    if (origin.provinceData.coastlineAdjacentProvinces[i].provinceName == target.provinceData.provinceName)
                    {
                        hasFoundAdjacency = true;
                    }
                    else
                    {
                        for (int j = 0; j < origin.provinceData.adjacentProvinces.Count; j++)
                        {
                            if (origin.provinceData.adjacentProvinces[j].provinceName == target.provinceData.provinceName && origin.provinceData.adjacentProvinces[j].isMaritime)
                            {
                                hasFoundAdjacency = true;
                            }
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
        if (!hasFoundAdjacency)
        {
            Debug.Log("Order Failed - missing adjacency");
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
                    Debug.Log("Standoff Detected: checking support");
                    //current origin province = origin
                    //current dest province = dest
                    //current enemy origin province = moveoriginlist[i]
                    for (int j = 0; j < orderManager.SupportMoveTargetList.Count; j++)
                    {
                        if (origin.provinceData.provinceName == orderManager.SupportMoveTargetList[j].GetComponent<ProvinceStats>().provinceData.provinceName && dest.provinceData.provinceName == orderManager.SupportMoveDestList[j].GetComponent<ProvinceStats>().provinceData.provinceName)
                        {
                            Debug.Log("Standoff Detected: This move has support: checking enemy support");
                            //current supporting province = supportmoveoriginlist[j]
                            for (int k = 0; k < orderManager.SupportMoveDestList.Count; k++)
                            {
                                if (orderManager.SupportMoveDestList[k].GetComponent<ProvinceStats>().provinceData.provinceName == orderManager.MoveDestList[i].GetComponent<ProvinceStats>().provinceData.provinceName && orderManager.SupportMoveTargetList[k].GetComponent<ProvinceStats>().provinceData.provinceName == orderManager.MoveOriginList[i].GetComponent<ProvinceStats>().provinceData.provinceName && orderManager.SupportMoveTargetList[k].GetComponent<ProvinceStats>().provinceData.provinceName != origin.provinceData.provinceName)
                                {
                                    Debug.Log("Standoff Detected: enemy has support: checking supports of this moves support");
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
                                                Debug.Log("standoff is equally supported, fuck off");
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
                if (dest.provinceData.provinceName == orderManager.HoldList[i].GetComponent<ProvinceStats>().provinceData.provinceName)
                {
                    Debug.Log("Standoff Hold Detected: checking support");
                    //current origin province = origin
                    //current dest province = dest
                    for (int j = 0; j < orderManager.SupportMoveTargetList.Count; j++)
                    {
                        if (origin.provinceData.provinceName == orderManager.SupportMoveTargetList[j].GetComponent<ProvinceStats>().provinceData.provinceName && dest.provinceData.provinceName == orderManager.SupportMoveDestList[j].GetComponent<ProvinceStats>().provinceData.provinceName)
                        {
                            Debug.Log("Standoff Detected: This move has support: checking enemy support");
                            //current supporting province = supportmoveoriginlist[j]
                            for (int k = 0; k < orderManager.SupportTargetList.Count; k++)
                            {
                                if (orderManager.SupportTargetList[k].GetComponent<ProvinceStats>().provinceData.provinceName == dest.provinceData.provinceName)
                                {
                                    Debug.Log("Standoff Detected: enemy supported hold, fuck off");
                                    //current enemy supporting province = supportmoveoriginlist[k]
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
            Debug.Log("Move Uncontested");
            hasWonStandoff = true;
        }
        return hasWonStandoff;
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
                yield return new WaitForSeconds(0.5f);
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
            yield return new WaitForSeconds(1f);
            if (hasParsedHolds && hasParsedMoves && hasParsedSupportMoves && hasParsedSupports && hasParsedConvoys)
            {
                orderManager.ClearOrders();
                UpdateAllProvinces();
                break;
            }
        }
    }
    public void UpdateAllProvinces()
    {
        for (int i = 0; i < provincesInScene.Count; i++)
        {
            provincesInScene[i].UpdateProvinceStats();
        }
    }
}
