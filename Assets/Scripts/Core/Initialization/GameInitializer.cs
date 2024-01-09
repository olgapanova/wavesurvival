using UnityEngine;
using System.Collections.Generic;

public class GameInitializer : MonoBehaviour
{
    [SerializeField] private UIView uiView;
    [SerializeField] private List<BindableScriptableObject> bindableScriptables;
    
    private List<ITickable> _tickables = new();
    private List<IFixedTickable> _fixedTickables = new();
    
    private void Awake() => GameInitialization();

    private void GameInitialization()
    {
        BindAllScriptables();
        InitializeAllManagersAndServices();
    }

    private void BindAllScriptables()
    {
        foreach (var scriptable in bindableScriptables)
            scriptable.Bind();
    }

    private void InitializeAllManagersAndServices()
    {
        //Creating instances of static managers
        var prefabSpawner = PrefabSpawner.CreateInstance();
        var playerManager = PlayerManager.CreateInstance();
        var enemiesManager = EnemiesManager.CreateInstance();
        var projectileControlSystem = ProjectileControlSystem.CreateInstance();
        
        //Initialize non static managers and services
        var inputServices = new InputSystem(playerManager.InputInfo);
        
        //Register tickables to necessary objects
        RegisterTickable(playerManager);
        RegisterTickable(inputServices);
        RegisterTickable(enemiesManager);
        RegisterTickable(projectileControlSystem);
        
        //Add UIView to managers which influence the ui
        AddUIViewToInfluencer(playerManager);
        AddUIViewToInfluencer(enemiesManager);
    }
    
    private void Update()
    {
        foreach (var tickable in _tickables)
        {
            tickable.Update();
        }
    }

    private void FixedUpdate()
    {
        foreach (var fixedTickable in _fixedTickables)
        {
            fixedTickable.FixedUpdate(Time.fixedDeltaTime);
        }
    }
    
    private void RegisterTickable(object tickableCandidate)
    {
        if (tickableCandidate is ITickable tickable)
            _tickables.Add(tickable);

        if (tickableCandidate is IFixedTickable fixedTickable)
            _fixedTickables.Add(fixedTickable);
    }

    private void AddUIViewToInfluencer(IInfluenceOnUI candidate) => candidate.UIView = uiView;
}
