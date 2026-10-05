using UnityEngine;

public enum LevelUpBonusType
{
    MaxHealth,
    LifeSteal,
    Armor,
    Damage,
    CriticalChance,
    CriticalDamage,
    Range,
    AbilityHaste,
    MoveSpeed,
    Luck,
    xpGain,
}

[CreateAssetMenu(menuName = "Level Up/Bonus Data")]
public class LevelUpBonusData : ScriptableObject
{
    [Header("Bonus")]
    public string _bonusName;
    [TextArea] public string _description;

    public LevelUpBonusType _type;
    public float _value;
}
