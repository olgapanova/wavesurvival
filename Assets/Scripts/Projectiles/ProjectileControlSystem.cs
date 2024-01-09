using System.Collections.Generic;
using UnityEngine;

public class ProjectileControlSystem : Singleton<ProjectileControlSystem>, ITickable
{
    private List<Projectile> _registeredProjectiles = new(); 

    public void RegisterProjectile(Projectile projectile) => _registeredProjectiles.Add(projectile);

    public void UnregisterAndDespawnProjectile(Projectile projectile)
    {
        _registeredProjectiles.Remove(projectile);
        PrefabSpawner.Instance.Despawn(projectile.ProjectileObject);
    }
    
    public void Update()
    {
        if (_registeredProjectiles.Count == 0) return;

        for (int i = 0; i < _registeredProjectiles.Count; i++)
        {
            var projectile = _registeredProjectiles[i];
            
            //TODO: that is also some todo logic for future for projectiles which follow player and despawn by timer, not only distance
            if (projectile.Target != null)
            {
                if (projectile.CurrentDespawnTimer <= 0)
                {
                    UnregisterAndDespawnProjectile(projectile);
                    continue;
                }

                projectile.CurrentDespawnTimer -= Time.deltaTime;
                var directionToTarget = (projectile.Target.position - projectile.ProjectileObject.transform.position).normalized;
                projectile.ProjectileObject.transform.position += directionToTarget * projectile.MoveSpeed * Time.deltaTime;
            }
            else
                projectile.ProjectileObject.transform.position += projectile.MoveDirection * projectile.MoveSpeed * Time.deltaTime;
            
            if (Vector2.Distance(projectile.StartPoint, projectile.ProjectileObject.transform.position) >= projectile.DespawnDistance)
                UnregisterAndDespawnProjectile(projectile);
        }
    }
}
