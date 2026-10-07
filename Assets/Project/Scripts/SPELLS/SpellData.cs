using UnityEngine;

[CreateAssetMenu(menuName = "Spells/Spell Data")]
public class SpellData : ScriptableObject
{
    public GameObject _prefab;
    public GameObject _projectilePrefab;


    [Header("Stats")]
    public int _range;
    public int _damage;
    public float _impactRadius;
    public float _speed;
    public float _cooldown;
    public int _maxCharges;
    public float _projectileCount;
    public float _projectileDelay;


    [Header("Improved")]
    public SpellData _improvedSpell;
}
