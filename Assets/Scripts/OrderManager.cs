using Crowsalina;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
public class OrderManager : MonoBehaviour
{
    public List<GameObject> MoveOriginList = new List<GameObject>();
    public List<GameObject> MoveDestList = new List<GameObject>();
    public List<GameObject> HoldList = new List<GameObject>();
    public List<GameObject> SupportMoveOriginList = new List<GameObject>();
    public List<GameObject> SupportMoveTargetList = new List<GameObject>();
    public List<GameObject> SupportOriginList = new List<GameObject>();
    public List<GameObject> SupportTargetList = new List<GameObject>();
    public List<GameObject> SupportMoveDestList = new List<GameObject>();
    public List<GameObject> ConvoyOriginList = new List<GameObject>();
    public List<GameObject> ConvoyTargetList = new List<GameObject>();
    public List<GameObject> ConvoyDestList = new List<GameObject>();
    public void AddHoldOrder(GameObject originProvince)
    { 
        HoldList.Add(originProvince);
        Debug.Log("Successfully added hold order for " + originProvince.GetComponent<ProvinceStats>().provinceData.provinceName.ToString());
    }
    public void AddMoveOrder(GameObject originProvince, GameObject destProvince)
    {
        MoveOriginList.Add(originProvince);
        MoveDestList.Add(destProvince);
        Debug.Log("Successfully added move order between " + originProvince.GetComponent<ProvinceStats>().provinceData.provinceName.ToString() + " + " + destProvince.GetComponent<ProvinceStats>().provinceData.provinceName.ToString());
    }
    public void AddSupportMoveOrder(GameObject originProvince, GameObject targetProvince, GameObject destProvince)
    {
        SupportMoveOriginList.Add(originProvince);
        SupportMoveDestList.Add(destProvince);
        SupportMoveTargetList.Add(targetProvince);
        Debug.Log("Successfully added support move order between " + originProvince.GetComponent<ProvinceStats>().provinceData.provinceName.ToString() + " + " + targetProvince.GetComponent<ProvinceStats>().provinceData.provinceName.ToString() + " to " + destProvince.GetComponent<ProvinceStats>().provinceData.provinceName.ToString());
    }
    public void AddSupportOrder(GameObject originProvince, GameObject targetProvince)
    { 
        SupportOriginList.Add(originProvince);
        SupportTargetList.Add(targetProvince);
        Debug.Log("Successfully added support order between " + originProvince.GetComponent<ProvinceStats>().provinceData.provinceName.ToString() + " + " + targetProvince.GetComponent<ProvinceStats>().provinceData.provinceName.ToString());
    }
    public void AddConvoyOrder(GameObject originProvince, GameObject targetProvince, GameObject destProvince)
    {
        ConvoyOriginList.Add(originProvince);
        ConvoyDestList.Add(destProvince);
        ConvoyTargetList.Add(targetProvince);
        Debug.Log("Successfully added convoy order between " + originProvince.GetComponent<ProvinceStats>().provinceData.provinceName.ToString() + " + " + targetProvince.GetComponent<ProvinceStats>().provinceData.provinceName.ToString() + " to " + destProvince.GetComponent<ProvinceStats>().provinceData.provinceName.ToString());
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
        Debug.Log("Cleared Last Orders");
    }
}
