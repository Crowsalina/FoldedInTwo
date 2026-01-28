using Crowsalina;
using UnityEngine;

public class ProvinceButtonManager : MonoBehaviour
{
    private ProvinceStats provinceStats;
    public bool canButtonBePressed, hasRequestedMovement, isCurrentMovementSupported, isProvinceClicked;
    public GameObject destinationProvince, supportingProvince;
    private OrderManager orderManager;
    private LocalOrderHandler localOrderHandler;
    
    private void Awake()
    {
        canButtonBePressed = true;
        provinceStats = gameObject.GetComponent<ProvinceStats>();
        orderManager = FindFirstObjectByType<OrderManager>();
        localOrderHandler = FindFirstObjectByType<LocalOrderHandler>();
        isProvinceClicked = false;
    }
    public void ButtonPressed()
    {
        if (canButtonBePressed)
        {
            if (isProvinceClicked && !localOrderHandler.isSupportOrderActive)
            {
                HoldOrder();
                isProvinceClicked = false;
            }
            else if (isProvinceClicked && localOrderHandler.isSupportOrderActive)
            {
                localOrderHandler.TrackProvinceSupport(this.gameObject);
                isProvinceClicked = false;
            }
            else
            {
                if (localOrderHandler.isMovementOpen && !localOrderHandler.isConvoyOrderActive && !localOrderHandler.isSupportOrderActive)
                {
                    localOrderHandler.TrackProvinceMove(this.gameObject);
                    isProvinceClicked = false;
                }
                else if (localOrderHandler.isMovementOpen && localOrderHandler.isConvoyOrderActive)
                {
                    localOrderHandler.TrackProvinceConvoy(this.gameObject);
                }
                else if (localOrderHandler.isMovementOpen && localOrderHandler.isSupportOrderActive)
                {
                    localOrderHandler.TrackProvinceSupportMove(this.gameObject);
                }
                else
                {
                    isProvinceClicked = true;
                    localOrderHandler.TrackOriginProvince(this.gameObject);
                    OrderOptions();
                }
            }
        }
    }
    public void OrderOptions()
    {
        localOrderHandler.isMovementOpen = true;
        while (isProvinceClicked)
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                SupportOrder();
            }
            if (Input.GetKeyDown(KeyCode.C))
            { 
                ConvoyOrder();
            }
        }
    }
    public void HoldOrder()
    {
        orderManager.AddHoldOrder(this.gameObject);
    }
    public void SupportOrder()
    {
        if (localOrderHandler.isConvoyOrderActive)
        {
            localOrderHandler.isConvoyOrderActive = false;
        }
        localOrderHandler.isSupportOrderActive = true;
    }
    public void ConvoyOrder()
    {
        if (localOrderHandler.isSupportOrderActive)
        {
            localOrderHandler.isSupportOrderActive = false;
        }
        localOrderHandler.isConvoyOrderActive = true;
    }
}
