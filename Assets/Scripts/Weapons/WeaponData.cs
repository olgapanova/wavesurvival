using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = StringConsts.StudioPrefix + "WeaponsData/New Weapon Data", fileName = "NewWeaponData")]
public class WeaponData : ScriptableObject
{
    [field: SerializeField] public WeaponType WeaponType { get; private set; }
    [field: SerializeField] public GameObject AttackPrefab { get; private set; }
    [field: SerializeField] public Explosion ExplosionData { get; private set; }
    [field: SerializeField] public float AttackPeriod { get; private set; }
    [field: SerializeField] public float AttackDamageValue { get; private set; }
    [field: SerializeField, Range(0,1)] public float AttackDefenceBreakingValue { get; private set; }
    [field: SerializeField] public float MoveSpeed { get; private set; }
    [field: SerializeField] public float DespawnDistance { get; private set; }
    [field: SerializeField] public List<FiringPoint> FiringPoints { get; private set; }
}

[Serializable]
public struct FiringPoint
{
    public Vector3 position;
    public Quaternion rotation;
    
}
