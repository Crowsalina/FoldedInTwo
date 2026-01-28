using Crowsalina;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProvinceSpawningManager : MonoBehaviour
{
    List<Province> allProvinces = new List<Province>();
    [SerializeField]
    private GameObject provincePrefab;
    [SerializeField]
    private Transform ProvinceHandler;
    List<GameObject> spawnLocations = new List<GameObject>();
    private void Start()
    {
        Province[] provinces = Resources.LoadAll<Province>("Provinces").OrderBy(go => go.provinceName).ToArray();
        allProvinces.AddRange(provinces);
        GameObject[] spawns = GameObject.FindGameObjectsWithTag("SpawnLocation").OrderBy(go => go.name).ToArray();
        spawnLocations.AddRange(spawns);
    }
    public void SpawnAllProvinces()
    {
        for (int i = 0; i < allProvinces.Count; i++)
        {
            BREAKOUT.Check();
            SpawnProvince(allProvinces[i], spawnLocations[i]);
        }
    }
    public void SpawnProvince(Province provinceData, GameObject spawnLocation)
    {
        GameObject newProvince = Instantiate(provincePrefab, spawnLocation.transform.position, Quaternion.identity, ProvinceHandler);
        newProvince.GetComponent<ProvinceStats>().provinceData = provinceData;
    }
}
