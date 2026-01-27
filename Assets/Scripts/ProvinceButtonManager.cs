using Crowsalina;
using UnityEngine;

public class ProvinceButtonManager : MonoBehaviour
{
    private ProvinceStats provinceStats;
    private GlobalMovementVariables GMV;
    public bool canButtonBePressed, hasRequestedMovement, isCurrentMovementSupported;
    public GameObject destinationProvince, supportingProvince;
    
    private void Awake()
    {
        provinceStats = gameObject.GetComponent<ProvinceStats>();
        GMV = FindFirstObjectByType<GlobalMovementVariables>();
    }
    public void ButtonPressed()
    {
        if (canButtonBePressed)
        {
            if (GMV.hasMovementStarted)
            {
                GMV.OriginProvince.GetComponent<ProvinceButtonManager>().SetDestinationProvince(this.gameObject);
            }
            else
            {
                if (provinceStats.hasArmy)
                {
                    GMV.RequestMove(this.gameObject, 0);
                    hasRequestedMovement = true;
                }
                else if (provinceStats.hasFleet)
                {
                    GMV.RequestMove(this.gameObject, 1);
                    hasRequestedMovement = true;
                }
            }
            if (hasRequestedMovement)
            {
                GMV.CancelMove(this.gameObject);
                hasRequestedMovement = false;
            }
        }
    }
    public void SetDestinationProvince(GameObject destProvince)
    {
        destinationProvince = destProvince;
    }
    public void SendUnit()
    {
        if (provinceStats.hasArmy)
        {
            destinationProvince.GetComponent<ProvinceStats>().hasArmy = true;
            provinceStats.hasArmy = false;
        }
        else if (provinceStats.hasFleet)
        {
            destinationProvince.GetComponent<ProvinceStats>().hasFleet = true;
            provinceStats.hasFleet = false;
        }
    }
}
