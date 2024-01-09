using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>, IFixedTickable, IInfluenceOnUI
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
    
    public InputInfo InputInfo { get; set; } = new InputInfo();
    public bool IsPlayerActive => _player != null && _player.activeSelf;
    public Transform Player => _player?.transform;

    private GameObject _player;
    private PlayerData _playerData;
    private PlayerModel _playerModel;

    public PlayerManager()
    {
        InputInfo.CastSpell += OnCastSpell;
        InputInfo.ChangeSpellPrevious += OnChangeSpellPrevious;
        InputInfo.ChangeSpellNext += OnChangeSpellNext;
    }

    private void OnStartGame()
    {
        if (!IsPlayerActive)
            SpawnPlayer(Vector2.zero);

        SetPlayerActive(true);
        SetPlayerParameters();
    }

    private void SetPlayerActive(bool status) => _player.SetActive(status);

    public void SpawnPlayer(Vector2 position)
    {
        _playerData = GameConfig.Instance.StartGameConfig.BasePlayerData;
        _player = PrefabSpawner.Instance.Spawn(_playerData.Prefab);
        
        SetPlayerParameters();
    }

    private void SetPlayerParameters()
    {
        _playerModel = new PlayerModel()
        {
            MaxHp = _playerData.Hp,
            Hp = _playerData.Hp,
            Speed = _playerData.Speed,
            DamageReceiver = _player.GetComponent<DamageReceiver>(),
            Animator = _player.GetComponent<Animator>(),
            Rigidbody = _player.GetComponent<Rigidbody2D>(),
            FiringPoint = _player.GetComponentInChildren<FiringPointView>(),
            WeaponsData = _playerData.WeaponData,
            WeaponIndexTimers = new List<float>(),
            ActiveWeaponData = _playerData.WeaponData[0],
            Defence = _playerData.Defence,
            MaxDefence = _playerData.Defence,
            DefenceRecoveryPeriod = _playerData.DefenceRecoveryPeriod,
            DefenceRecoveryTimer = _playerData.DefenceRecoveryPeriod,
            DefenceRecoveryValueInPeriod = _playerData.DefenceRecoveryValueInPeriod
        };

        if (_playerModel.DamageReceiver.DamageReceived != null)
            _playerModel.DamageReceiver.DamageReceived -= OnDamageReceived;
        
        _playerModel.DamageReceiver.DamageReceived += OnDamageReceived;
        _playerModel.DamageReceiver.SetHpBarValue(1);
        _playerModel.DamageReceiver.SetDefenceBarValue(1);

        //Timers in twin List which copied the index position of every weapon player have
        for (int i = 0; i < _playerModel.WeaponsData.Count; i++)
            _playerModel.WeaponIndexTimers.Add(0);
    }
    
    public void FixedUpdate(float fixedDeltaTime)
    {
        if (!IsPlayerActive) return;

        for (int i = 0; i < _playerModel.WeaponIndexTimers.Count; i++)
        {
            if (_playerModel.WeaponIndexTimers[i] > 0)
                _playerModel.WeaponIndexTimers[i] -= fixedDeltaTime;
        }

        if (_playerModel.DefenceRecoveryTimer <= 0)
        {
            RecoverDefence();
            _playerModel.DefenceRecoveryTimer = _playerModel.DefenceRecoveryPeriod;
        }
        else
            _playerModel.DefenceRecoveryTimer -= fixedDeltaTime;
        
        if (InputInfo.MoveDirInput != Vector2.zero)
            Move(fixedDeltaTime);
        else
            _playerModel.Animator.SetBool("IsMoving", false);
    }

    private void RecoverDefence()
    {
        _playerModel.Defence += _playerModel.DefenceRecoveryValueInPeriod;
        if (_playerModel.Defence > _playerModel.MaxDefence)
            _playerModel.Defence = _playerModel.MaxDefence;
    }

    private void Move(float fixedDeltaTime)
    {
        _playerModel.Rigidbody.position += InputInfo.MoveDirInput * _playerModel.Speed * fixedDeltaTime;

        _playerModel.Animator.SetFloat("Horizontal", InputInfo.MoveDirInput.x);
        _playerModel.Animator.SetFloat("Vertical", InputInfo.MoveDirInput.y);
        _playerModel.Animator.SetBool("IsMoving", true);

        RotateFiringPoint(InputInfo.MoveDirInput.x, InputInfo.MoveDirInput.y);
    }

    //Because the player isn't actually rotate, I made a firing point which rotates similarly as the animation
    private void RotateFiringPoint(float x, float y)
    {
        if (x == y)
        {
            var angle = y < 0 ? 180 :
                x > 0 ? -90 : 90;
            _playerModel.FiringPoint.transform.localRotation = Quaternion.Euler(0,0,angle);
        }
        else
        {
            if (Mathf.Abs(x) > Mathf.Abs(y))
                _playerModel.FiringPoint.transform.localRotation = Quaternion.Euler(0,0, x > 0 ? -90 : 90);
            else
                _playerModel.FiringPoint.transform.localRotation = Quaternion.Euler(0,0, y > 0 ? 0 : 180);
        }
    }

    private void OnDamageReceived(GameObject damageObject, DamageInfo damageInfo)
    {
        if (damageInfo.DamageInitiator.layer == LayerMask.NameToLayer("Player"))
            return;
        
        _playerModel.Hp -= damageInfo.DamageValue - (damageInfo.DamageValue * _playerModel.Defence);
        _playerModel.DamageReceiver.SetHpBarValue(_playerModel.Hp / _playerModel.MaxHp);
        if (_playerModel.Defence > 0)
        {
            _playerModel.Defence -= damageInfo.DefenceBreakingValue;
            if (_playerModel.Defence < 0)
                _playerModel.Defence = 0;
            
            _playerModel.DamageReceiver.SetDefenceBarValue(_playerModel.Defence / _playerModel.MaxDefence);
        }

        //Player is dead, need to disable him
        if (_playerModel.Hp <= 0)
        {
            SetPlayerActive(false);
            _uiView.SetStartGameCanvasEnabled(true);
        }
    }

    private void OnCastSpell()
    {
        if (!IsPlayerActive) return;
        
        var activeWeapon = _playerModel.ActiveWeaponData;
        var currentWeaponIndex = _playerModel.WeaponsData.IndexOf(activeWeapon);
        if (_playerModel.WeaponIndexTimers[currentWeaponIndex] > 0) return;

        _playerModel.WeaponIndexTimers[currentWeaponIndex] = activeWeapon.AttackPeriod;

        foreach (var firingPoint in activeWeapon.FiringPoints)
        {
            CreateProjectile(firingPoint, out var projectile);

            var damager = projectile.ProjectileObject.GetComponent<Damager>();
            damager.DamageInfo.DamageValue = activeWeapon.AttackDamageValue;
            damager.DamageInfo.DefenceBreakingValue = activeWeapon.AttackDefenceBreakingValue;
            damager.DamageInfo.DamageInitiator = _player;
            damager.DamageInfo.Projectile = projectile;
            
            if (damager.DamageApplied != null)
                damager.DamageApplied -= OnDamageApplied;

            damager.DamageApplied += OnDamageApplied;
        }
    }

    //Creating the projectile in world considering firing point on player's position and rotation 
    private void CreateProjectile(FiringPoint firingPoint, out Projectile projectile)
    {
        var position = _playerModel.FiringPoint.transform.position + firingPoint.position;
        var projectileObject = PrefabSpawner.Instance.Spawn(_playerModel.ActiveWeaponData.AttackPrefab, position, firingPoint.rotation);
        var firingPointTransform = _playerModel.FiringPoint.transform;
        projectileObject.transform.RotateAround(firingPointTransform.position, Vector3.forward, firingPointTransform.rotation.eulerAngles.z);
            
        projectile = new Projectile()
        {
            ProjectileObject = projectileObject,
            DespawnDistance = _playerModel.ActiveWeaponData.DespawnDistance,
            MoveDirection = projectileObject.transform.up,
            MoveSpeed = _playerModel.ActiveWeaponData.MoveSpeed,
            StartPoint = position
        };

        ProjectileControlSystem.Instance.RegisterProjectile(projectile);
    }

    private void OnDamageApplied(DamageInfo damageInfo, GameObject objectToDamage)
    {
        if (objectToDamage.layer == LayerMask.NameToLayer("Player")) return;

        if (damageInfo.Projectile != null)
        {
            //TODO: I want to make explosion damage for future
            if (damageInfo.Projectile.ExplosionData != null)
            {
                //for explosion logic for future
            }
            
            ProjectileControlSystem.Instance.UnregisterAndDespawnProjectile(damageInfo.Projectile);
        }
    }

    private void OnChangeSpellPrevious()
    {
        if (!IsPlayerActive) return;

        var previousIndex = _playerModel.WeaponsData.IndexOf(_playerModel.ActiveWeaponData) - 1;
        if (previousIndex < 0)
            previousIndex = _playerModel.WeaponsData.Count - 1;

        _playerModel.ActiveWeaponData = _playerModel.WeaponsData[previousIndex];
    }

    private void OnChangeSpellNext()
    {
        if (!IsPlayerActive) return;

        var nextIndex = _playerModel.WeaponsData.IndexOf(_playerModel.ActiveWeaponData) + 1;
        if (nextIndex > _playerModel.WeaponsData.Count - 1)
            nextIndex = 0;

        _playerModel.ActiveWeaponData = _playerModel.WeaponsData[nextIndex];
    }
}
