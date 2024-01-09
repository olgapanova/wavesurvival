using UnityEngine;

public class EnemyModel
{
    public float MaxHp { get; set; }
    public float Hp { get; set; }
    public float MaxDefence { get; set; }
    public float Defence { get; set; }
    
    /// <summary>
    /// TODO: maybe in future I'll make the boss enemy who recover defence idk
    /// </summary>
    public float DefenceRecoveryValueInPeriod { get; set; }
    public float DefenceRecoveryPeriod { get; set; }
    public float DefenceRecoveryTimer { get; set; }
    public float Speed { get; set; } 
    public float ArriveDistanceToTarget { get; set; }
    public WeaponData WeaponData { get; set; }
    public DamageReceiver DamageReceiver { get; set; }
    public float AttackDelayTimer { get; set; }
    public Damager MeleeDamager { get; set; }
    public Collider2D MeleeDamagerCollider { get; set; }
}
