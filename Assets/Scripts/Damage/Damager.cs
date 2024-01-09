using System;
using UnityEngine;

public class Damager : MonoBehaviour
{
    public DamageInfo DamageInfo { get; set; } = new DamageInfo();
    public Action<DamageInfo, GameObject> DamageApplied;

    private void OnTriggerEnter2D(Collider2D collisionObject)
    {
        if (collisionObject.TryGetComponent<DamageReceiver>(out var damageReceiver))
        {
            DamageApplied?.Invoke(DamageInfo, collisionObject.gameObject);
        }
    }
}
