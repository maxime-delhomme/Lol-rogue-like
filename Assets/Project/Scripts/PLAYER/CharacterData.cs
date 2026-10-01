using UnityEngine;

[CreateAssetMenu(menuName = "Characters/Character Data")]
public class CharacterData : ScriptableObject
{
    [Header("Reference")]

    [Header("Statistiques")]
    public int _maxHealth;
    public int _maxRage;
    public int _maxExperience;
    public int _moveSpeed;
    public float _lookRotationSpeed;

    [Header("Experience")]
    public float _baseExperience;
    public float _experienceExponent;
    public float _experienceLinearBonus;
}
