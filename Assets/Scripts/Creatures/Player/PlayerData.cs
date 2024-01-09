using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = StringConsts.StudioPrefix + "CreaturesData/New Player Data", fileName = "NewPlayerData")]
public class PlayerData : ParentCreatureData
{
    [field: SerializeField] public float DefenceRecoveryPeriod { get; private set; }
    [field: SerializeField, Range(0,1)] public float DefenceRecoveryValueInPeriod { get; private set; }
    [field: SerializeField] public List<WeaponData> WeaponData { get; private set; }
}
