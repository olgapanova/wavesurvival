using UnityEngine;

[CreateAssetMenu(menuName = StringConsts.StudioPrefix + "CreaturesData/New Enemies Spawner Data", fileName = "NewEnemiesSpawnerData")]
public class EnemiesSpawnerData : ScriptableObject
{
    [field: SerializeField] public GameObject Prefab { get; private set; }
    [field: SerializeField] public int MaxSpawnedEnemiesCount { get; private set; }
    [field: SerializeField] public float AfterSpawnDelaySec { get; private set; }
}
