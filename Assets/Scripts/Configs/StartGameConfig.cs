using UnityEngine;

[CreateAssetMenu(menuName = StringConsts.StudioPrefix + StringConsts.GameConfig + "StartGameConfig", fileName = "StartGameConfig")]
public class StartGameConfig : ScriptableObject
{
    [field: SerializeField] public PlayerData BasePlayerData { get; private set; }
    [field: SerializeField] public EnemiesSpawnerData EnemiesSpawnerData { get; private set; }
}
