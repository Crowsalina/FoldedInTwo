using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Crowsalina
{
    [CreateAssetMenu(fileName = "New Province", menuName = "Province")]
    public class Province : ScriptableObject
    {
        public string provinceName;
        public int startingPower;
        public bool isWater, isCoastal, isSupply;
        public List<Province> adjacentProvinces;
    }
}
