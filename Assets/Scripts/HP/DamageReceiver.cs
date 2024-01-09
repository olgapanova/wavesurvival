using System;
using UnityEngine;
using UnityEngine.UI;

public class DamageReceiver : MonoBehaviour
{
    [SerializeField] private Image _hpBar;
    [SerializeField] private Image _defenceBar;

    public Action<GameObject, DamageInfo> DamageReceived;

    private void OnTriggerEnter2D(Collider2D collisionObject)
    {
        if (collisionObject.TryGetComponent<Damager>(out var damager))
            DamageReceived?.Invoke(gameObject, damager.DamageInfo);
    }

    public void SetHpBarValue(float value)
    {
        if (_hpBar != null) 
            ProgressBarFiller.Fill(_hpBar, value);
    }

    public void SetDefenceBarValue(float value)
    {
        if (_hpBar != null) 
            ProgressBarFiller.Fill(_defenceBar, value);
    }
}
