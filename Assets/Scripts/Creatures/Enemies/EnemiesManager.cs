using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemiesManager : Singleton<EnemiesManager>, IFixedTickable, IInfluenceOnUI
{
    private UIView _uiView;
    public UIView UIView
    {
        get { return _uiView; }
        set
        {
            _uiView = value;
            UIView.StartGamePressed += OnStartGame;
        } 
    }
    
    private EnemiesSpawnerData _spawnerData;
    private float _spawnEnemiesDelayTimer;
    private bool _canSpawnEnemies;
    private Collider2D _spawnerCollider;
    
    private Dictionary<GameObject, EnemyModel> _registeredEnemies = new ();

    public EnemiesManager()
    {
        _spawnerData = GameConfig.Instance.StartGameConfig.EnemiesSpawnerData;
        _canSpawnEnemies = false;
        _spawnEnemiesDelayTimer = 0;

        var spawner = PrefabSpawner.Instance.Spawn(_spawnerData.Prefab);
        _spawnerCollider = spawner.GetComponent<Collider2D>();
    }

    private void OnStartGame()
    {
        CanSpawnEnemies(true);
        UIView.ClearEnemiesCounter();
        DespawnAll();
    }
    
    #region ParametersSetters

    private void SetEnemyParameters(EnemyData enemyData, GameObject enemyObject, out EnemyModel model)
    {
        model = new EnemyModel()
        {
            MaxHp = enemyData.Hp,
            Hp = enemyData.Hp,
            Defence = enemyData.Defence,
            MaxDefence = enemyData.Defence,
            Speed = enemyData.Speed,
            WeaponData = enemyData.WeaponData,
            ArriveDistanceToTarget = enemyData.ArriveDistanceToTarget,
            AttackDelayTimer = 0,
            DamageReceiver = enemyObject.GetComponent<DamageReceiver>()
        };

        if (model.DamageReceiver.DamageReceived != null)
            model.DamageReceiver.DamageReceived -= OnDamageReceived;
        
        model.DamageReceiver.DamageReceived += OnDamageReceived;
        model.DamageReceiver.SetHpBarValue(1);
        model.DamageReceiver.SetDefenceBarValue(1);

        //In current state all enemies are melee, but maybe for future issues I made this "if" to set melee damager
        if (model.WeaponData.WeaponType == WeaponType.Melee)
            SetMeleeDamager(model, enemyObject);
    }

    private void SetMeleeDamager(EnemyModel model, GameObject enemyObject)
    {
        var damagerObject = PrefabSpawner.Instance.Spawn(model.WeaponData.AttackPrefab, position: enemyObject.transform.position, parent: enemyObject.transform);
        model.MeleeDamager = damagerObject.GetComponent<Damager>();
        model.MeleeDamager.DamageInfo.DamageInitiator = enemyObject;
        model.MeleeDamager.DamageInfo.DamageValue = model.WeaponData.AttackDamageValue;
        model.MeleeDamager.DamageInfo.DefenceBreakingValue = model.WeaponData.AttackDefenceBreakingValue;
        model.MeleeDamager.DamageInfo.Projectile = null;
            
        if (model.MeleeDamager.DamageApplied != null)
            model.MeleeDamager.DamageApplied -= OnDamageApplied;
            
        model.MeleeDamager.DamageApplied += OnDamageApplied;
            
        model.MeleeDamagerCollider = model.MeleeDamager.GetComponent<Collider2D>();
        model.MeleeDamagerCollider.enabled = true;
    }

    #endregion

    #region Random Spawn and Despawn Logic
    
    public void CanSpawnEnemies(bool status) => _canSpawnEnemies = status;

    private void SpawnEnemy()
    {
        var randomEnemy = Random.Range(0, EnemiesDataList.Instance.EnemiesData.Count);
        var enemyData = EnemiesDataList.Instance.EnemiesData[randomEnemy];
        var spawnedEnemy = PrefabSpawner.Instance.Spawn(enemyData.Prefab, GenerateRandomSpawnPoint());

        SetEnemyParameters(enemyData, spawnedEnemy, out var enemyModel);
        
        _registeredEnemies.Add(spawnedEnemy, enemyModel);
        _spawnEnemiesDelayTimer = _spawnerData.AfterSpawnDelaySec;
    }
    
    private Vector2 GenerateRandomSpawnPoint()
    {
        //Make a closest point on bounds at Physics2D is a bit problematic, 'cause if point is inside the area ClosestPoint returns the incoming value
        //I don't want hardcode so we'll make it this way
        //First of all, we search for a random point in certain rectangle area. And then we'll find closest distance point at bounds
        //between X bounds and randomPointX value and same for the Y value.
        var bounds = _spawnerCollider.bounds;
        
        var randomPointX = Random.Range(bounds.min.x, bounds.max.x);
        CalculateMinDistancePoint(new Vector2(bounds.min.x, bounds.max.x), randomPointX, out var minDistancePointX, out var minDistanceX);
        
        var randomPointY = Random.Range(bounds.min.y, bounds.max.y);
        CalculateMinDistancePoint(new Vector2(bounds.min.y, bounds.max.y), randomPointY, out var minDistancePointY, out var minDistanceY);
        
        //Then when all closest distance point were founded we can find the closest in between these two and change X or Y position
        //to move our's random point to correct bound border
        Vector2 closestPoint = new Vector2(randomPointX, randomPointY);
        if (minDistanceX < minDistanceY)
            closestPoint.x = minDistancePointX;
        else
            closestPoint.y = minDistancePointY;

        return closestPoint;
    }

    private void CalculateMinDistancePoint(Vector2 minMaxValues, float searchValue, out float minDistancePoint, out float minDistance)
    {
        float distanceToMinXPoint = Mathf.Abs(minMaxValues.x - searchValue);
        float distanceToMaxXPoint = Mathf.Abs(minMaxValues.y - searchValue);
        if (distanceToMinXPoint < distanceToMaxXPoint)
        {
            minDistancePoint = minMaxValues.x;
            minDistance = distanceToMinXPoint;
        }
        else
        {
            minDistancePoint = minMaxValues.y;
            minDistance = distanceToMaxXPoint;
        }
    }
    
    private void DespawnEnemy(GameObject enemy, EnemyModel model)
    {
        if (model.WeaponData.WeaponType == WeaponType.Melee)
            PrefabSpawner.Instance.Despawn(enemy.GetComponentInChildren<Damager>().gameObject);
        
        _registeredEnemies.Remove(enemy);
        PrefabSpawner.Instance.Despawn(enemy);
    }

    private void DespawnAll()
    {
        foreach (var enemy in _registeredEnemies)
        {
            if (enemy.Value.WeaponData.WeaponType == WeaponType.Melee)
                PrefabSpawner.Instance.Despawn(enemy.Key.GetComponentInChildren<Damager>().gameObject);
            PrefabSpawner.Instance.Despawn(enemy.Key);
        }
        
        _registeredEnemies.Clear();
    }

    #endregion
    
    private void OnDamageApplied(DamageInfo enemyDamageInfo, GameObject objectToDamage)
    {
        if (objectToDamage.layer == LayerMask.NameToLayer("Enemies"))
            return;

        var model = _registeredEnemies[enemyDamageInfo.DamageInitiator];
        if (model.AttackDelayTimer != 0)
            return;
        
        model.AttackDelayTimer = model.WeaponData.AttackPeriod;
        model.MeleeDamagerCollider.enabled = false;
    }

    private void OnDamageReceived(GameObject damagedObject, DamageInfo damageInfo)
    {
        if (damageInfo.DamageInitiator.layer == LayerMask.NameToLayer("Enemies")) return;
        if (!_registeredEnemies.ContainsKey(damagedObject)) return;
        
        var model = _registeredEnemies[damagedObject];
        model.Hp -= damageInfo.DamageValue - (damageInfo.DamageValue * model.Defence);
        model.DamageReceiver.SetHpBarValue(model.Hp / model.MaxHp);
        
        if (model.Defence > 0)
        {
            model.Defence -= damageInfo.DefenceBreakingValue;
            if (model.Defence < 0)
                model.Defence = 0;
            
            model.DamageReceiver.SetDefenceBarValue(model.Defence / model.MaxDefence);
        }
        
        //Enemy is dead, need to unregister him
        if (model.Hp <= 0)
        {
            DespawnEnemy(damagedObject, model);
            UIView.AddEnemyToCounter(1);
        }
    }

    public void FixedUpdate(float fixedDeltaTime)
    {
        if (_canSpawnEnemies && _spawnEnemiesDelayTimer == 0 && _registeredEnemies.Count < _spawnerData.MaxSpawnedEnemiesCount)
            SpawnEnemy();
        
        if (_spawnEnemiesDelayTimer != 0)
        {
            _spawnEnemiesDelayTimer -= fixedDeltaTime;
            if (_spawnEnemiesDelayTimer < 0)
                _spawnEnemiesDelayTimer = 0;
        }
        
        foreach (var enemy in _registeredEnemies)
        {
            if (enemy.Value.AttackDelayTimer != 0)
            {
                if (enemy.Value.AttackDelayTimer <= 0)
                {
                    enemy.Value.AttackDelayTimer = 0;
                    if (enemy.Value.WeaponData.WeaponType == WeaponType.Melee)
                        enemy.Value.MeleeDamagerCollider.enabled = true;
                }
                else
                    enemy.Value.AttackDelayTimer -= fixedDeltaTime;
            }
            
            if (!PlayerManager.Instance.IsPlayerActive) continue;
            
            //Move Enemy logic - it stops before player
            if (Vector2.Distance(PlayerManager.Instance.Player.position, enemy.Key.transform.position) > enemy.Value.ArriveDistanceToTarget)
            {
                var direction = (PlayerManager.Instance.Player.position - enemy.Key.transform.position).normalized;
                enemy.Key.transform.position += direction * enemy.Value.Speed * fixedDeltaTime;   
            }
        }
    }
}
