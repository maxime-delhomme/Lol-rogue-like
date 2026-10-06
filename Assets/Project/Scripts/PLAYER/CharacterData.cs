using UnityEngine;

[CreateAssetMenu(menuName = "Characters/Character Data")]
public class CharacterData : ScriptableObject
{
    [Header("Reference")]


    [Header("InGameStats")]
    public int _maxHealth;
    public float _lifeSteal;
    public int _armor;
    public int _damage;
    public float _critChance;
    public float _critDamage;
    public int _range;
    public float _abilityHaste;
    public float _moveSpeed;
    public float _luck;
    public float _xpGain;
    public int _attractionRange;


    [Header("OffStats")]
    public int _maxRage;
    public int _maxExperience;
    public float _lookRotationSpeed;


    [Header("Experience")]
    public float _baseExperience;
    public float _experienceExponent;
    public float _experienceLinearBonus;
}
