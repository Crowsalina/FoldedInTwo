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
        public bool isMaritime, isCoastal, isSupply, isFleetOnly;
        public List<Province> adjacentProvinces, childProvinces;
    }
}
