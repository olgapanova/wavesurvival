using UnityEngine;

public class Projectile
{
    public GameObject ProjectileObject { get; set; }
    public Explosion ExplosionData { get; set; }
    public Transform Target { get; set; }
    public float DespawnTime { get; set; }
    public float CurrentDespawnTimer { get; set; }
    public Vector3 MoveDirection { get; set; }
    public float MoveSpeed { get; set; }
    public Vector2 StartPoint { get; set; }
    public float DespawnDistance { get; set; }
}
