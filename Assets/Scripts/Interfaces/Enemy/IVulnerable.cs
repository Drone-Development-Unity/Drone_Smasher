using UnityEngine;

public interface IVulnerable
{
    void TakeDamage(int damage);
    void Die();
    bool IsAlive { get; set; }
    }
