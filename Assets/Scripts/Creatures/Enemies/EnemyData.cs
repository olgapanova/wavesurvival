using UnityEngine;

[CreateAssetMenu(menuName = StringConsts.StudioPrefix + "CreaturesData/New Enemy Data", fileName = "NewEnemyData")]
public class EnemyData : ParentCreatureData
{
    [field: SerializeField] public WeaponData WeaponData { get; private set; }
    [field: SerializeField] public float ArriveDistanceToTarget { get; private set; }
}
