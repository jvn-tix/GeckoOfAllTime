using UnityEngine;

public enum PowerUpType
{
    Damage,
    FireRate,
    AdditionalProjectile,
    ClusterBullet
}

public enum PowerUpTier { Small, Medium, Large }

[System.Serializable]
public class PowerUpPreset
{
    public PowerUpTier tier = PowerUpTier.Small;
    [Min(1)] public int weight = 10;  
    public float value;               
}

[System.Serializable]
public class PowerUpData
{
    public PowerUpType type;
    public string title;
    [TextArea] public string description;
    public Sprite icon;

    [Header("Static (Cluster & AdditionalProjectile)")]
    public float value;

    [Header("Preset (Damage & FireRate)")]
    public PowerUpPreset[] presets;

    public bool UsesPreset =>
        (type == PowerUpType.Damage || type == PowerUpType.FireRate)
        && presets != null && presets.Length > 0;

    public PowerUpOffer CreateOffer()
    {
        if (!UsesPreset)
            return new PowerUpOffer(this, value, null);

        int total = 0;
        foreach (var p in presets) total += p.weight;

        int roll = Random.Range(0, total);
        int sum = 0;
        foreach (var p in presets)
        {
            sum += p.weight;
            if (roll < sum)
                return new PowerUpOffer(this, p.value, p.tier.ToString());
        }

        return new PowerUpOffer(this, presets[0].value, presets[0].tier.ToString());
    }
}

public class PowerUpOffer
{
    public PowerUpData data;
    public float finalValue;
    public string tierLabel;  

    public PowerUpOffer(PowerUpData data, float finalValue, string tierLabel)
    {
        this.data = data;
        this.finalValue = finalValue;
        this.tierLabel = tierLabel;
    }
}