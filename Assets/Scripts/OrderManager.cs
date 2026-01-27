using Crowsalina;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
public class OrderManager : MonoBehaviour
{
    List<GameObject> MoveOriginList = new List<GameObject>();
    List<GameObject> MoveDestList = new List<GameObject>();
    List<GameObject> HoldList = new List<GameObject>();
    List<GameObject> SupportMoveOriginList = new List<GameObject>();
    List<GameObject> SupportMoveTargetList = new List<GameObject>();
    List<GameObject> SupportOriginList = new List<GameObject>();
    List<GameObject> SupportTargetList = new List<GameObject>();
    List<GameObject> SupportMoveDestList = new List<GameObject>();
    List<GameObject> ConvoyOriginList = new List<GameObject>();
    List<GameObject> ConvoyTargetList = new List<GameObject>();
    List<GameObject> ConvoyDestList = new List<GameObject>();
    public void AddHoldOrder(GameObject originProvince)
    { 
        HoldList.Add(originProvince);
    }
    public void AddMoveOrder(GameObject originProvince, GameObject destProvince)
    {
        MoveOriginList.Add(originProvince);
        MoveDestList.Add(destProvince);
    }
    public void AddSupportMoveOrder(GameObject originProvince, GameObject targetProvince, GameObject destProvince)
    {
        SupportMoveOriginList.Add(originProvince);
        SupportMoveDestList.Add(destProvince);
        SupportMoveTargetList.Add(targetProvince);
    }
    public void AddSupportOrder(GameObject originProvince, GameObject targetProvince)
    { 
        SupportOriginList.Add(originProvince);
        SupportTargetList.Add(targetProvince);
    }
    public void AddConvoyOrder(GameObject originProvince, GameObject targetProvince, GameObject destProvince)
    {
        ConvoyOriginList.Add(originProvince);
        ConvoyDestList.Add(destProvince);
        ConvoyTargetList.Add(targetProvince);
    }
    public void ClearOrders()
    { 
        MoveDestList.Clear();
        MoveDestList.TrimExcess();
        MoveOriginList.Clear();
        MoveOriginList.TrimExcess();
        HoldList.Clear();
        HoldList.TrimExcess();
        SupportOriginList.Clear();
        SupportOriginList.TrimExcess();
        SupportTargetList.Clear();
        SupportTargetList.TrimExcess();
        SupportMoveDestList.Clear();
        SupportMoveDestList.TrimExcess();
        ConvoyOriginList.Clear();
        ConvoyOriginList.TrimExcess();
        ConvoyTargetList.Clear();
        ConvoyTargetList.TrimExcess();
        ConvoyDestList.Clear();
        ConvoyDestList.TrimExcess();
    }
}
