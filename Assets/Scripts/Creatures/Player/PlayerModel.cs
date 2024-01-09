using System.Collections.Generic;
using UnityEngine;

public class PlayerModel
{
    public float MaxHp { get; set; }
    public float Hp { get; set; }
    public float MaxDefence { get; set; }
    public float Defence { get; set; }
    public float DefenceRecoveryValueInPeriod { get; set; }
    public float DefenceRecoveryPeriod { get; set; }
    public float DefenceRecoveryTimer { get; set; }
    public float Speed { get; set; } 
    public DamageReceiver DamageReceiver { get; set; }
    public List<WeaponData> WeaponsData { get; set; }
    public List<float> WeaponIndexTimers { get; set; }
    public WeaponData ActiveWeaponData { get; set; }
    public Animator Animator { get; set; }
    public Rigidbody2D Rigidbody { get; set; }
    public FiringPointView FiringPoint { get; set; }
}
