using UnityEngine;

[CreateAssetMenu(menuName = StringConsts.StudioPrefix + StringConsts.GameConfig + "GameConfig", fileName = "GameConfig")]
public class GameConfig : BindableScriptableObject
{
    [field: SerializeField] public StartGameConfig StartGameConfig { get; private set; }
    
    public static GameConfig Instance { get; private set; }
    public override void Bind() => Instance = this;
}
