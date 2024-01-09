using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = StringConsts.StudioPrefix + "DataDictionaties/New Enemies Data List", fileName = "EnemiesDataList")]
public class EnemiesDataList : BindableScriptableObject
{
    [field: SerializeField] public List<EnemyData> EnemiesData { get; private set; }
    
    public static EnemiesDataList Instance { get; private set; }
    public override void Bind() => Instance = this;
}
