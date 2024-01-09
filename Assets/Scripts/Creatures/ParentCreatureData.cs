using UnityEngine;

public class ParentCreatureData : ScriptableObject
{
    [field: SerializeField] public GameObject Prefab { get; private set; }
    [field: SerializeField] public float Speed { get; private set; }
    [field: SerializeField, Range(0,1)] public float Defence { get; private set; }
    [field: SerializeField] public float Hp { get; private set; }
}
