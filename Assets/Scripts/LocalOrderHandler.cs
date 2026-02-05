using Crowsalina;
using NUnit.Framework;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
public class LocalOrderHandler : MonoBehaviour
{
    public GameObject currentOriginProvince, currentDestProvince, currentTargetProvince;
    public bool isMovementOpen, isSupportOrderActive, isConvoyOrderActive;
    private OrderManager orderManager;
    private void Start()
    {
        isMovementOpen = false;
        isSupportOrderActive = false;
        isConvoyOrderActive = false;
        orderManager = FindFirstObjectByType<OrderManager>();
    }
    public void TrackProvinceMove(GameObject province)
    {
        orderManager.AddMoveOrder(currentOriginProvince, province);
        ClearTracking();
    }
    public void TrackProvinceSupport(GameObject province)
    {
        orderManager.AddSupportOrder(currentOriginProvince, province);
        ClearTracking();
    }
    public void TrackProvinceSupportMove(GameObject province)
    {
        if (currentTargetProvince == null)
        {
            currentTargetProvince = province;
        }
        else
        {
            if (province.GetComponent<ProvinceStats>().provinceData.provinceName == currentTargetProvince.GetComponent<ProvinceStats>().provinceData.provinceName)
            {
                TrackProvinceSupport(province);
                ClearTracking();
            }
            else
            {
                orderManager.AddSupportMoveOrder(currentOriginProvince, currentTargetProvince, province);
                ClearTracking();
            }
        }
    }
    public void TrackProvinceConvoy(GameObject province)
    {
        if (currentTargetProvince == null)
        {
            currentTargetProvince = province;
        }
        else
        {
            orderManager.AddConvoyOrder(currentOriginProvince, currentTargetProvince, province);
            ClearTracking();
        }
    }
    public void TrackOriginProvince(GameObject province)
    {
        currentOriginProvince = province;
    }
    public void ClearTracking()
    {
        try
        {
            currentOriginProvince.GetComponent<ProvinceButtonManager>().isProvinceClicked = false;
        }
        catch 
        {

        }
        isConvoyOrderActive = false;
        isSupportOrderActive = false;
        isMovementOpen = false;
        currentOriginProvince = null;
        currentDestProvince = null;
        currentTargetProvince = null;
    }
    public void SupportCall()
    {
        currentOriginProvince.GetComponent<ProvinceButtonManager>().SupportOrder();
    }
    public void ConvoyCall()
    {
        currentOriginProvince.GetComponent<ProvinceButtonManager>().ConvoyOrder();
    }
}
