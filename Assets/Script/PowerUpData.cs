using UnityEngine;

public enum PowerUpType
{
    Damage,
    FireRate,
    AdditionalProjectile,
    MaxHP
}

[System.Serializable]
public class PowerUpData
{
    public PowerUpType type;
    public string title;
    [TextArea] public string description;
    public Sprite icon;
    public float value; // Nilai penambahan (misal: +5 Dmg, +0.2 FireRate, +1 Projectile, +20 HP)
}