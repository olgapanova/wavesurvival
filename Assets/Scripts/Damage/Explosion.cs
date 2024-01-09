using UnityEngine;

public class Explosion
{
    [field: SerializeField] public GameObject ExplosionPrefab { get; private set; }
    [field: SerializeField] public float ExplosionDamageValue { get; private set; }
    [field: SerializeField] public float ExplosionDefenceBreakingValue { get; private set; }
    [field: SerializeField] public float ExplosionRadius { get; private set; }
}
