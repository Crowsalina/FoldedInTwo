using Crowsalina;
using System.Collections.Generic;
using UnityEngine;

public class ProvinceSpawningManager : MonoBehaviour
{
    List<Province> allProvinces = new List<Province>();
    [SerializeField]
    private GameObject provincePrefab;
    [SerializeField]
    private Transform ProvinceHandler;
    [SerializeField]
    List<GameObject> spawnLocations = new List<GameObject>();
    private void Start()
    {
        Province[] provinces = Resources.LoadAll<Province>("Provinces");
        allProvinces.AddRange(provinces);
    }
    public void SpawnAllProvinces()
    {
        for (int i = 0; i < allProvinces.Count; i++)
        {
            SpawnProvince(allProvinces[i], spawnLocations[i]);
        }
    }
    public void SpawnProvince(Province provinceData, GameObject spawnLocation)
    {
        GameObject newProvince = Instantiate(provincePrefab, spawnLocation.transform.position, Quaternion.identity, ProvinceHandler);
        newProvince.GetComponent<ProvinceStats>().provinceData = provinceData;
    }
}
