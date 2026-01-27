using Crowsalina;
using NUnit.Framework;
using System.Collections.Generic;
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
        currentOriginProvince.GetComponent<ProvinceButtonManager>().isProvinceClicked = false;
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
        else if (currentDestProvince == null)
        {
            orderManager.AddSupportMoveOrder(currentOriginProvince, currentTargetProvince, province);
            ClearTracking();
        }
    }
    public void TrackProvinceConvoy(GameObject province)
    { 
        
    }
    public void TrackOriginProvince(GameObject province)
    { 
        currentOriginProvince = province;
    }
    public void ClearTracking()
    {
        currentOriginProvince.GetComponent<ProvinceButtonManager>().isProvinceClicked = false;
        currentOriginProvince = null;
        currentDestProvince = null;
        currentTargetProvince = null;
    }
}
