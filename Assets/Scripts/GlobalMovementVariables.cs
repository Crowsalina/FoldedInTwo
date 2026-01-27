using Crowsalina;
using System.Collections.Generic;
using UnityEngine;
public class GlobalMovementVariables : MonoBehaviour
{
    public bool hasMovementStarted;
    public GameObject OriginProvince;
    List<GameObject> MoveList = new List<GameObject>();
    private void Start()
    {
        hasMovementStarted = false;
    }
    public void RequestMove(GameObject originProvince, int unitType)
    {
        OriginProvince = originProvince;
        AddProvinceToMovelist(originProvince);
    }
    public void CancelMove(GameObject originProvince)
    {
        MoveList.Remove(originProvince);
        MoveList.TrimExcess();
    }
    public void AddProvinceToMovelist(GameObject province)
    {
        MoveList.Add(province);
    }
    public void BeginMovementParsing()
    {
        for (int i = 0; i < MoveList.Count; i++)
        {
            for (int j = 0; j < MoveList.Count; j++)
            {
                if (j != i)
                {
                    if (MoveList[i].GetComponent<ProvinceButtonManager>().destinationProvince == MoveList[j].GetComponent<ProvinceButtonManager>().destinationProvince && MoveList[i].GetComponent<ProvinceButtonManager>().isCurrentMovementSupported)
                    {
                        if (!MoveList[j].GetComponent<ProvinceButtonManager>().isCurrentMovementSupported)
                        {
                            MoveList[i].GetComponent<ProvinceButtonManager>().SendUnit();
                        }
                        else
                        { 
                        
                        }
                    }
                }
            }
        }
    }
}
